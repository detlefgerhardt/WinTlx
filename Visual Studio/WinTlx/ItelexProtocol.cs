using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Languages;

namespace WinTlx
{
	public enum ItelexCommands
	{
		Heartbeat = 0x00,
		DirectDial = 0x01,
		BaudotData = 0x02,
		End = 0x03,
		Reject = 0x04,
		Ack = 0x06,
		ProtocolVersion = 0x07,
		SelfTest = 0x08,
		RemoteConfig = 0x09,
		ConnectRemote = 0x81,
		RemoteConfirm = 0x82,
		RemoteCall = 0x83,
		AcceptCallRemote = 0x84,
	}

	class ItelexProtocol
	{
		private const string TAG = nameof(ItelexProtocol);

		public enum ConnectionStates
		{
			Disconnected,
			TcpConnected, // after TCP connect and before first data (texting mode unknown)
			Connected, // after direct dial cmd was received or direct dial = 0
			AsciiTexting,
			ItelexTexting, // = baudot texting
			ItelexDisconnected // waiting for reconnect
		}

		public enum ConnectionDirections
		{
			In,
			Out
		}

		public enum CentralexStates
		{
			None,
			CentralexConnect,
			CentralexConnected,
			CentralexRejected,
		}

		public enum CentralexConnectResults
		{
			Ok,
			TcpError,
			AuthError,
			TimeoutError,
			OtherError
		}

		public delegate void ConnectEventHandler();
		public event ConnectEventHandler Connected;

		public delegate void DroppedEventHandler();
		public event DroppedEventHandler Dropped;

		public delegate void ReceivedEventHandler(string asciiText);
		public event ReceivedEventHandler Received;

		public delegate void UpdateEventHandler();
		public event UpdateEventHandler Update;

		public delegate void SendEventHandler(string asciiText);
		public event SendEventHandler Send;

		public delegate void BaudotSendRecvEventHandler(byte[] code);
		public event BaudotSendRecvEventHandler BaudotSendRecv;

		public delegate void MessageEventHandler(string asciiText);
		public event MessageEventHandler Message;

		private const int RECV_BUFFERSIZE = 2048;

		private TcpListener _tcpListener;

		private object DisconnectLock = new object();

		private TcpClient _client;

		private System.Timers.Timer _sendTimer;
		private bool _sendTimerActive;

		private System.Timers.Timer _centralexReconnectTimer;

		private System.Timers.Timer _ackTimer;
		private bool _ackTimerActive;
		private long _lastAckReceived;
		private int _lastRecvAckValue;
		private bool _ackRecvFlag;

		private long _lastSendRecvIdleMs;
		private long _lastSentMs;
		public int IdleTimerMs
		{
			get
			{
				if (!IsConnected)
				{
					return 0;
				}
				else
				{
					return (int)(Helper.GetTicksMs() - _lastSendRecvIdleMs);
				}
			}
		}

		private byte[] _itelixSendBuffer = new byte[Constants.ITELIX_SENDBUFFER_SIZE];
		private int _itelixSendCount;

		private DateTime? _connStartTime = null;
		public int ConnTimeMin
		{
			get
			{
				if (_connStartTime == null || !IsConnected)
				{
					return 0;
				}
				else
				{
					return (int)(DateTime.Now.Subtract(_connStartTime.Value).Ticks / (10000000 * 60));
				}
			}
		}

		public string ConnTimeMinSek
		{
			get
			{
				if (_connStartTime == null || !IsConnected)
				{
					return "";
				}
				else
				{
					int secs = (int)(DateTime.Now.Subtract(_connStartTime.Value).Ticks / 10000000);
					if (secs<120)
					{
						return $"{secs} sec";
					}
					else
					{
						return $"{secs / 60} min";
					}
				}
			}
		}

		private ConfigData _config => ConfigManager.Instance.Config;

		private EyeballChar _eyeballChar;
		public bool EyeballCharActive { get; set; }

		private ShiftStates _shiftState;
		public ShiftStates ShiftState
		{
			get { return _shiftState; }
			set { _shiftState = value; }
		}

		private int _n_recv;
		private int _n_trans;
		private int _n_ack;
		private ConcurrentQueue<byte> _sendBuffer;
		private object _sendLock = new object();

		public int CharsToSendCount
		{
			get
			{
				return _sendBuffer != null ? _sendBuffer.Count : 0;
			}
		}

		public int CharsAckCount
		{
			get
			{
				lock (_sendLock)
				{

					int trans = _n_trans;
					if (_n_ack > trans)
						trans += 256;
					return trans - _n_ack;
				}
			}
		}

		public bool Local { get; set; } = true;

		public bool IsConnected => ConnectionState != ConnectionStates.Disconnected && ConnectionState != ConnectionStates.ItelexDisconnected;

		public bool RecvOn { get; set; }

		public ConnectionStates ConnectionState { get; set; }

		private ConnectionDirections _connectionDirection;

		private CentralexStates CentralexState { get; set; }
		private string _centralexHost;
		private int _centralexPort;

		public string ConnectionStateString
		{
			get
			{
				switch (ConnectionState)
				{
					case ConnectionStates.Disconnected:
						return "Disconnected";
					case ConnectionStates.TcpConnected:
						return "TCP connection";
					case ConnectionStates.Connected:
						if (_connectionDirection == ConnectionDirections.In)
						{
							return $"Connected # {_config.IncomingExtensionNumber}";
						}
						else
						{
							return $"Connected";
						}
					case ConnectionStates.AsciiTexting:
						return "Ascii";
					case ConnectionStates.ItelexTexting:
						return "i-Telex";
					case ConnectionStates.ItelexDisconnected:
						return "Pause";
					default:
						return "???";
				}
			}
		}

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static ItelexProtocol instance;

		public static ItelexProtocol Instance => instance ?? (instance = new ItelexProtocol());

		/// <summary>
		/// constructor
		/// </summary>
		private ItelexProtocol()
		{
			_eyeballChar = EyeballChar.Instance;
			CentralexState = CentralexStates.None;

			_ackTimer = new System.Timers.Timer();
			_ackTimer.Elapsed += AckTimer_Elapsed;

			_sendTimer = new System.Timers.Timer();
			_sendTimer.Elapsed += SendTimer_Elapsed;

			_centralexReconnectTimer = new System.Timers.Timer();
			_centralexReconnectTimer.Elapsed += CentralexReconnectTimer_Elapsed;
		}

		public bool SetRecvOn(int port)
		{
			Logging.Instance.Debug(TAG, nameof(SetRecvOn), "");
			try
			{
				_tcpListener = new TcpListener(IPAddress.Any, port);
				_tcpListener.Start();

				// start listener task for incoming connections
				Task _listenerTask = Task.Run(() => Listener());
				RecvOn = true;
				Update?.Invoke();
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SetRecvOn), "", ex);
				return false;
			}
		}

		public bool SetRecvOff()
		{
			Logging.Instance.Debug(TAG, nameof(SetRecvOff), "");
			_tcpListener.Stop();
			RecvOn = false;
			Update?.Invoke();
			return true;
		}

		private void Listener()
		{
			while (true)
			{
				try
				{
					if (!RecvOn)
					{
						return;
					}

					// wait for connection
					_client = _tcpListener.AcceptTcpClient();
					if (_config.IncomingExtensionNumber != 0)
					{
						// set to connected and wait for direct dial command
						ConnectionState = ConnectionStates.TcpConnected;
					}
					else
					{
						// no direct dial command neccessary
						ConnectInit();
					}
					_connectionDirection = ConnectionDirections.In;
					StartReceive();

					IPAddress remoteAddr = ((IPEndPoint)_client.Client.RemoteEndPoint).Address;
					Message?.Invoke($"{LngText(LngKeys.Message_IncomingConnection)} {remoteAddr}");
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(Listener), $"incoming connection from {remoteAddr}");
					Update?.Invoke();

					while (true)
					{
						if (_client == null)
							return;
						if (!_client.Connected)
							break;
					}

					Disconnect();
					_connectionDirection = ConnectionDirections.Out;
				}
				catch (Exception ex)
				{
					if (ex.HResult != -2147467259)
					{
						Logging.Instance.Error(TAG, nameof(Listener), "", ex);
					}
					Disconnect();
				}
			}
		}

		public async Task<bool> ConnectOut(string host, int port, int extensionNumber, bool asciiMode = false)
		{
			bool status = false;
			await Task.Run(() =>
			{
				try
				{
					_client = new TcpClient(host, port);
					if (_client == null || !_client.Connected)
					{
						Logging.Instance.Log(LogTypes.Info, TAG, nameof(ConnectOut), $"outgoing connection {host}:{port} failed");
						return false;
					}
				}
				catch (Exception ex)
				{
					Logging.Instance.Error(TAG, nameof(ConnectOut), "", ex);
					return false;
				}

				ConnectInit();
				ConnectionState = ConnectionStates.TcpConnected;
				_connectionDirection = ConnectionDirections.Out;

				StartReceive();

				// wait for cr/lf
				Stopwatch sw = new Stopwatch();
				sw.Start();
				while(sw.ElapsedMilliseconds<2000 && ConnectionState==ConnectionStates.TcpConnected)
				{
					Thread.Sleep(100);
				}
				sw.Stop();

				if (!asciiMode)
				{
					SendVersionCodeCmd();
				}

				Update?.Invoke();

				if (!asciiMode)
				{
					SendDirectDialCmd(extensionNumber);
				}

				if (ConnectionState==ConnectionStates.Disconnected)
				{
					status = true;
					return true;
				}

				ConnectionState = ConnectionStates.Connected;
				Logging.Instance.Info(TAG, nameof(ConnectOut), $"outgoing connection {host}:{port}");

				if (asciiMode)
				{
					ConnectionState = ConnectionStates.AsciiTexting;
					SendAsciiText("\n\r");
				}
				else
				{
					SendCode(CodeManager.BAU_LTRS, ref _shiftState);
				}
				Update?.Invoke();

				status = true;
				return true;
			});

			return status;
		}

		public async Task<bool> CentralexConnectAsync(string host, int port)
		{
			_centralexHost = host;
			_centralexPort = port;

			CentralexConnectResults result = await Task.Run(() => CentralexConnect());

			bool status = false;
			switch (result)
			{
				case CentralexConnectResults.Ok:
					Logging.Instance.Info(TAG, nameof(CentralexConnectAsync), $"connected {host}:{port}");
					Message?.Invoke($"connected to centralex {host}:{port}");
					RecvOn = true;
					status = true;
					break;
				case CentralexConnectResults.TcpError:
					Logging.Instance.Info(TAG, nameof(CentralexConnectAsync), $"tcp connection failed {host}:{port}");
					Message?.Invoke($"centralex tcp connection failed {host}:{port}");
					status = false;
					break;
				case CentralexConnectResults.TimeoutError:
					Logging.Instance.Info(TAG, nameof(CentralexConnectAsync), $"connection timeout {host}:{port}");
					Message?.Invoke($"centralex connection timeout {host}:{port}");
					status = false;
					break;
				case CentralexConnectResults.AuthError:
					Logging.Instance.Info(TAG, nameof(CentralexConnectAsync), $"authentication error {host}:{port}");
					Message?.Invoke($"centralex authentication timeout {host}:{port}");
					status = false;
					break;
				case CentralexConnectResults.OtherError:
					Logging.Instance.Info(TAG, nameof(CentralexConnectAsync), $"error {host}:{port}");
					Message?.Invoke($"centralex error {host}:{port}");
					status = false;
					break;
				default:
					status = false;
					break;
			}

			Update?.Invoke();
			return status;
		}

		private CentralexConnectResults CentralexConnect()
		{
			try
			{
				_client = new TcpClient(_centralexHost, _centralexPort);
				if (_client == null || !_client.Connected)
				{
					return CentralexConnectResults.TcpError;
				}
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(CentralexConnectAsync), $"connected {_centralexHost}:{_centralexPort}", ex);
				return CentralexConnectResults.TcpError;
			}

			CentralexState = CentralexStates.CentralexConnect;
			StartReceive();

			SendConnectRemoteCmd(_config.OwnNumber, _config.SubscribeServerUpdatePin);

			Stopwatch sw = new Stopwatch();
			sw.Start();
			while (true)
			{
				if (CentralexState != CentralexStates.CentralexConnect)
				{
					break;
				}
				if (sw.ElapsedMilliseconds > 5000)
				{
					Disconnect();
					return CentralexConnectResults.TimeoutError;
				}
			}

			CentralexInit();
			if (CentralexState == CentralexStates.CentralexRejected)
			{
				return CentralexConnectResults.AuthError;
			}
			if (CentralexState != CentralexStates.CentralexConnected)
			{
				return CentralexConnectResults.OtherError;
			}

			return CentralexConnectResults.Ok;
		}

		public void CentralexDisconnect()
		{
			Disconnect();
			CentralexState = CentralexStates.None;
			RecvOn = false;
		}

		private void CentralexInit()
		{
			_ackTimerActive = false;
			_ackTimer.Stop();
			_ackTimer.Interval = 15000;
			_ackTimer.Start();
		}

		private async void CentralexReconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			_centralexReconnectTimer.Stop();
			await CentralexConnectAsync(_centralexHost, _centralexPort);
		}

		private void ConnectInit()
		{
			Debug.WriteLine($"{nameof(ConnectInit)} start");

			_n_recv = 0;
			_n_trans = 0;
			_n_ack = 0;
			_sendBuffer = new ConcurrentQueue<byte>();
			_ackRecvFlag = false;
			_ackTimerActive = false;
			_ackTimer.Stop();
			_ackTimer.Interval = 2000;
			_ackTimer.Start();

			_sendTimerActive = false;
			_sendTimer.Stop();
			_sendTimer.Interval = 100;
			_sendTimer.Start();

			EyeballCharActive = false;

			_shiftState = ShiftStates.Unknown;
			_lastSendRecvIdleMs = Helper.GetTicksMs();

			Local = false;

			_connStartTime = DateTime.Now;

			ConnectionState = ConnectionStates.Connected;

			Update?.Invoke();
			Connected?.Invoke();

			Debug.WriteLine($"{nameof(ConnectInit)} end");
		}

		public void Disconnect()
		{
			lock (DisconnectLock)
			{
				if (ConnectionState == ConnectionStates.Disconnected)
				{
					return;
				}

				_ackTimer?.Stop();
				_sendTimer?.Stop();

				ConnectionState = ConnectionStates.Disconnected;
				Debug.WriteLine($"Disconnect() ConnectionState={ConnectionState}");

				_client?.Close();

				Local = false;

				Logging.Instance.Log(LogTypes.Info, TAG, nameof(Disconnect), $"connection dropped");

				Dropped?.Invoke();
				Update?.Invoke();

				if (CentralexState==CentralexStates.CentralexConnected)
				{
					_centralexReconnectTimer.Interval = 5000;
					_centralexReconnectTimer.Start();
				}
			}
		}

		private void SendTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			//Debug.WriteLine("SendTimer_Elapsed");
			if (!IsConnected || _ackTimerActive || _sendTimerActive)
			{
				//Debug.WriteLine($"SendTimer_Elapsed: !IsConnected || _ackSendTimerActive || _sendTimerActive");
				return;
			}

			try
			{
				_sendTimerActive = true;

				//Debug.WriteLine($"SendTimer_Elapsed");
				if (_client != null && !_client.Connected)
				{
					Logging.Instance.Info(TAG, nameof(SendTimer_Elapsed), $"!_client.Connected");
					Disconnect();
					return;
				}
				SendTimer();
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendTimer_Elapsed), "", ex);
			}
			finally
			{
				_sendTimerActive = false;
			}
		}

		private void SendTimer()
		{
			//Debug.WriteLine($"SendTimer: {_sendBuffer.Count} {_itelixSendCount} {CharsAckCount} {_n_trans}/{_n_ack}={CharsAckCount}");
			//Logging.Instance.Log(LogTypes.Info, TAG, nameof(SendTimer), $"SendTimer: {_sendBuffer.Count} {_itelixSendCount} {CharsAckCount} {_n_trans}/{_n_ack}");
			//Debug.WriteLine($"{CharsAckCount}");
			lock (_sendLock)
			{
				int ackCount = CharsAckCount;
				for (int i = 0; !_sendBuffer.IsEmpty && _itelixSendCount < Constants.ITELIX_SENDBUFFER_SIZE && ackCount < Constants.ITELIX_REMOTEBUFFER_SIZE;
					i++)
				{
					if (!_sendBuffer.TryDequeue(out byte baudotCode))
					{
						break;
					}

					_itelixSendBuffer[_itelixSendCount++] = baudotCode;
					///Debug.WriteLine($"dequeue {asciiChr}");
					//Logging.Instance.Debug(TAG, nameof(SendTimer), $"dequeue baudotCode={baudotCode:X02}, _itelixSendCount={_itelixSendCount}, ackCount={ackCount}");
					ackCount++;
				}
			}

			//Debug.WriteLine($"_itelixSendCount={_itelixSendCount}");

			if (_itelixSendCount == 0)
			{
				//Debug.WriteLine($"#2 _itelixSendCount={_itelixSendCount}");
				//Logging.Instance.Log(LogTypes.Info, TAG, nameof(SendTimer), $"#2 {_itelixSendCount} == 0");
				return;
			}

			// if character count in itelex send buffer is < ITELIX_SENDBUFFER_SIZE wait for WAIT_BEFORE_SEND_MSEC before sending them
			if (_itelixSendCount < Constants.ITELIX_SENDBUFFER_SIZE &&
				Helper.GetTicksMs() - _lastSentMs < Constants.WAIT_BEFORE_SEND_MSEC)
			{
				//Debug.WriteLine($"#1 {_itelixSendCount} < {Constants.ITELIX_SENDBUFFER_SIZE} && {Helper.GetTicksMs() - _lastSentMs} < {Constants.WAIT_BEFORE_SEND_MSEC}");
				//Logging.Instance.Log(LogTypes.Info, TAG, nameof(SendTimer), $"#1 {_itelixSendCount} < {Constants.ITELIX_SENDBUFFER_SIZE} && {Helper.GetTicksMs() - _lastSentMs} < {Constants.WAIT_BEFORE_SEND_MSEC}");
				Debug.WriteLine($"lastsend={Helper.GetTicksMs() - _lastSentMs}");
				return;
			}

			//Debug.WriteLine($"_itelixSendCount={_itelixSendCount}");

			byte[] baudotData = new byte[_itelixSendCount];
			Buffer.BlockCopy(_itelixSendBuffer, 0, baudotData, 0, _itelixSendCount);
			SendCmd(ItelexCommands.BaudotData, baudotData);
			_itelixSendCount = 0;
			_lastSentMs = Helper.GetTicksMs();
			_lastSendRecvIdleMs = Helper.GetTicksMs();
		}

		private void AckTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_ackTimerActive)
			{
				return;
			}
			_ackTimerActive = true;

			if (CentralexState == CentralexStates.CentralexConnected && !IsConnected)
			{
				SendHeartbeatCmd();
				_ackTimerActive = false;
				return;
			}

			if (!IsConnected || ConnectionState == ConnectionStates.AsciiTexting)
			{
				_ackTimerActive = false;
				return;
			}

			// check received acks

			lock (_sendLock)
			{
				//Debug.WriteLine(CharsAckCount);
				if (CharsAckCount > 0)
				{
					string timeout = null;
					if (_ackRecvFlag && _lastRecvAckValue == _n_ack)
					{
						// if ack was received and ack did'n change since the last AckTimer_Elapsed (2 sec)
						Logging.Instance.Warn(TAG, nameof(AckTimer_Elapsed), $"recv ack value changed timeout");
						timeout = "nochange";
					}
					_ackRecvFlag = false;

					if (Helper.GetTicksMs() - _lastAckReceived > 5000)
					{
						Logging.Instance.Warn(TAG, nameof(AckTimer_Elapsed), $"ack recv timeout");
						timeout = "timeout";
					}
					if (timeout!=null)
					{
						_n_trans = _n_ack;
					}
				}
				_lastRecvAckValue = _n_ack;
			}

			SendAckCmd(_n_recv);

			_ackTimerActive = false;
		}

		public int GetSendBufferCount()
		{
			return _sendBuffer != null ? _sendBuffer.Count : 0;
		}

		public void SendAsciiChar(char asciiChr)
		{
			SendAsciiText(asciiChr.ToString());
		}

		public void SendAsciiText(string asciiStr)
		{
			if (string.IsNullOrEmpty(asciiStr))
			{
				return;
			}

			//_lastSendRecvIdleMs = Helper.GetTicksMs();

			string telexData = CodeManager.AsciiStringToTelex(asciiStr, _config.CodeSet);
			Send?.Invoke(telexData);

			if (ConnectionState == ConnectionStates.AsciiTexting)
			{
				if (_client.Client==null)
				{
					Disconnect();
					return;
				}
				byte[] data = Encoding.ASCII.GetBytes(asciiStr);
				_client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, EndSend, null);
			}
			else
			{
				for (int c = 0; c < asciiStr.Length; c++)
				{
					byte[] baudotData = CodeManager.AsciiStringToBaudot(asciiStr[c].ToString(), ref _shiftState, _config.CodeSet);
					for (int i = 0; i < baudotData.Length; i++)
					{
						byte chr = baudotData[i];
						SendCode(baudotData[i], ref _shiftState);
					}
				}
				Update?.Invoke();
			}
		}

		public void SendDirectDialCmd(int code)
		{
			Logging.Instance.Info(TAG, nameof(SendDirectDialCmd), $"code={code}");
			byte[] data = new byte[] { (byte)code };
			SendCmd(ItelexCommands.DirectDial, data);
		}

		public void SendAckCmd(int ackVal)
		{
			//Debug.WriteLine($"Send Ack {ackVal}");
			byte[] data = new byte[] { (byte)ackVal };
			SendCmd(ItelexCommands.Ack, data);
		}

		public void SendHeartbeatCmd()
		{
			SendCmd(ItelexCommands.Heartbeat, new byte[0]);
		}

		public void SendRejectCmd(string reason)
		{
			Logging.Instance.Info(TAG, nameof(SendRejectCmd), $"reason={reason}");
			reason += '\0';
			byte[] data = Encoding.ASCII.GetBytes(reason);
			SendCmd(ItelexCommands.Reject, data);
		}

		public void SendVersionCodeCmd()
		{
			string version = Helper.GetVersionCode();
			byte[] versionData = Encoding.ASCII.GetBytes(version);
			Logging.Instance.Info(TAG, nameof(SendVersionCodeCmd), $"version={version}");

			byte[] data = new byte[versionData.Length + 2];
			data[0] = 1;    // version 1
			Buffer.BlockCopy(versionData, 0, data, 1, versionData.Length);
			data[data.Length - 1] = 0;

			/*
			data = new byte[5];
			data[0] = 1;
			data[1] = 56;
			data[2] = 51;
			data[3] = 55;
			data[4] = 0;
			*/

			SendCmd(ItelexCommands.ProtocolVersion, data);
		}

		public void SendEndCmd()
		{
			SendCmd(ItelexCommands.End);
		}

		public void SendConnectRemoteCmd(int number, int pin)
		{
			byte[] data = new byte[6];
			byte[] num = BitConverter.GetBytes((UInt32)number);
			Buffer.BlockCopy(num, 0, data, 0, 4);
			num = BitConverter.GetBytes((UInt16)pin);
			Buffer.BlockCopy(num, 0, data, 4, 2);
			SendCmd(ItelexCommands.ConnectRemote, data);
		}

		public void SendAcceptCallRemoteCmd()
		{
			if (CentralexState==CentralexStates.CentralexConnected)
			{
				SendCmd(ItelexCommands.AcceptCallRemote);
			}
		}

		private void SendCode(byte baudotCode, ref ShiftStates shiftState)
		{
			byte[] codes = CodeManager.BaudotCodeToBaudotWithShift(baudotCode, shiftState, ref shiftState);

			if (EyeballCharActive)
			{
				byte[] buffer = new byte[0];
				for (int i=0; i<codes.Length; i++)
				{
					byte[] newCodes = _eyeballChar.GetPunchCodes(codes[i], shiftState);
					buffer = Helper.AddBytes(buffer, newCodes);
				}
				codes = buffer;
			}

			BaudotSendRecv?.Invoke(codes);

			if (Local)
			{
				Update?.Invoke();
				return;
			}

			if (!IsConnected)
			{
				return;
			}

			if (IdleTimerMs >= 25000)
			{   // wake up remote machine with LTRS
				EnqueueSend(new byte[] { CodeManager.BAU_LTRS });
				EnqueueSend(new byte[] { CodeManager.BAU_LTRS });
				EnqueueSend(new byte[] { CodeManager.BAU_LTRS });
				if (shiftState == ShiftStates.Figs)
				{
					EnqueueSend(new byte[] { CodeManager.BAU_FIGS });
				}
			}

			EnqueueSend(codes );
			Update?.Invoke();
		}

		public void SendBaudotCode(byte code)
		{
			string asciiStr = CodeManager.BaudotStringToAscii(new byte[] { code }, ref _shiftState, _config.CodeSet, CodeManager.SendRecv.Send);
			Send?.Invoke(asciiStr);
			if (!IsConnected || Local)
			{
				return;
			}

			EnqueueSend(new byte[] { code });
			/*
			lock (_sendLock)
			{
				_sendBuffer.Enqueue(code);
			}
			*/

			Update?.Invoke();
		}

		private void EnqueueSend(byte[] codes)
		{
			/*
			while (_sendBuffer.Count > 10)
			{
				Thread.Sleep(100);
			}
			*/
			lock (_sendLock)
			{
				for (int i = 0; i < codes.Length; i++)
				{
					_sendBuffer.Enqueue(codes[i]);
				}
			}
		}

		public void SendCmd(ItelexCommands cmd, byte[] data = null)
		{
			SendCmd((int)cmd, data);
		}

		public void SendCmd(int cmdCode, byte[] data = null)
		{
			if (ConnectionState == ConnectionStates.Disconnected && CentralexState==CentralexStates.None)
			{
				return;
			}
			if (_client.Client == null)
			{
				Disconnect();
				return;
			}

			byte[] sendData;
			if (data != null)
			{
				sendData = new byte[data.Length + 2];
				sendData[0] = (byte)cmdCode;
				sendData[1] = (byte)data.Length;
				Buffer.BlockCopy(data, 0, sendData, 2, data.Length);
			}
			else
			{
				sendData = new byte[2];
				sendData[0] = (byte)cmdCode;
				sendData[1] = 0;
			}

			ItelexPacket packet = new ItelexPacket(sendData);

			if (packet.CommandType != ItelexCommands.Ack)
			{
				//Logging.Instance.Log(LogTypes.Debug, TAG, nameof(DecodePacket),
				//		$"Send packet {packet.CommandType} {packet.Command:X02} {packet.GetDebugData()}");
				Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendCmd),
						$"Send packet {packet.CommandType} {packet.GetDebugPacket()}");
			}

			switch ((ItelexCommands)packet.Command)
			{
				case ItelexCommands.BaudotData:
					AddTransCharCount(data.Length);
					Logging.Instance.AppendBinary(packet.Data, Logging.BinaryModes.Send);
					//Debug.WriteLine($"BaudotData {packet.GetDebugData()}");
					break;
				case ItelexCommands.Heartbeat:
				case ItelexCommands.Ack:
					break;
				case ItelexCommands.DirectDial:
				case ItelexCommands.End:
				case ItelexCommands.Reject:
				case ItelexCommands.ProtocolVersion:
				case ItelexCommands.SelfTest:
				case ItelexCommands.RemoteConfig:
					break;
			}
			try
			{
				_client.Client.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, EndSend, null);
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendCmd), "", ex);
				Disconnect();
			}
		}

		private void StartReceive()
		{
			if (ConnectionState == ConnectionStates.Disconnected && CentralexState == CentralexStates.None)
			{
				return;
			}
			if (_client.Client==null)
			{
				Disconnect();
				return;
			}

			byte[] buffer = new byte[RECV_BUFFERSIZE];
			try
			{
				_client.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, DataReceived, buffer);
			}
			catch
			{
				Disconnect();
			}
		}

		private void EndSend(IAsyncResult ar)
		{
			if (ConnectionState == ConnectionStates.Disconnected && CentralexState==CentralexStates.None)
			{
				return;
			}
			if (_client.Client == null)
			{
				Disconnect();
				return;
			}

			try
			{
				_client.Client.EndSend(ar);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		private void DataReceived(IAsyncResult ar)
		{
			
			if (ConnectionState == ConnectionStates.Disconnected && CentralexState == CentralexStates.None)
			{
				return;
			}
			if (_client.Client == null)
			{
				Disconnect();
				return;
			}

			int dataReadCount;

			try
			{
				dataReadCount = _client.Client.EndReceive(ar);
			}
			catch
			{
				Disconnect();
				return;
			}

			if (dataReadCount == 0)
			{
				Disconnect();
				return;
			}

			byte[] byteData = ar.AsyncState as byte[];
			byte[] newData = byteData.Take(dataReadCount).ToArray();

			if (ConnectionState == ConnectionStates.Connected)
			{
				if (newData[0] <= 0x09 || newData[0] >= 0x10 && newData[0] < 0x1F)
				{
					ConnectionState = ConnectionStates.ItelexTexting;
				}
				else
				{
					ConnectionState = ConnectionStates.AsciiTexting;
				}
				Update?.Invoke();
			}

			if (ConnectionState == ConnectionStates.AsciiTexting)
			{	// ascii
				string asciiText = Encoding.ASCII.GetString(newData, 0, newData.Length);
				asciiText = asciiText.Replace('@', CodeManager.ASC_WRU);
				Received?.Invoke(asciiText);
				_lastSendRecvIdleMs = Helper.GetTicksMs();
			}
			else
			{	// i-telex
				int dataPos = 0;
				while (dataPos + 2 <= newData.Length)
				{
					//Debug.WriteLine($"datapos={dataPos} cmd={newData[dataPos]:X02}");
					int packetLen = newData[dataPos + 1] + 2;
					if (dataPos + packetLen > newData.Length)
					{
						// short packet data
						break;
					}

					byte[] packetData = new byte[packetLen];
					Buffer.BlockCopy(newData, dataPos, packetData, 0, packetLen);
					dataPos += packetLen;

					if (packetData.Length >= 2 && packetData.Length >= packetData[1] + 2)
					{
						ItelexPacket packet = new ItelexPacket(packetData);
						//Debug.WriteLine($"packet={packet}");
						DecodePacket(packet);
					}
				}
			}
			StartReceive();

#if false
			// if data comes in different frame (untested)
			if (newData.Length > 0)
			{
				// append newData to _incommingData
				byte[] newIncomingData = new byte[_incomingData.Length + newData.Length];
				Buffer.BlockCopy(_incomingData, 0, newIncomingData, 0, _incomingData.Length);
				Buffer.BlockCopy(newData, 0, newIncomingData, _incomingData.Length, newData.Length);
				_incomingData = newData;
			}

			bool exitWhile = false;
			while (exitWhile)
			{
				exitWhile = true;
				if (_incomingData.Length>2)
				{
					if (_incomingData[1]<=_incomingData.Length+2)
					{
						// telegram complete

					}
				}


			}
#endif
		}

		private void DecodePacket(ItelexPacket packet)
		{
			/*
			if (packet.CommandType != ItelexCommands.Ack)
			{
				//Logging.Instance.Log(LogTypes.Debug, TAG, nameof(DecodePacket),
				//		$"Recv packet {packet.CommandType} {packet.Command:X02} {packet.GetDebugData()}");
				Logging.Instance.Debug(TAG, nameof(DecodePacket),
					$"Recv packet {packet.CommandType} {packet.GetDebugPacket()}");
			}
			if (packet.CommandType != ItelexCommands.Heartbeat && packet.CommandType != ItelexCommands.Ack)
			{
				Debug.WriteLine($"Recv packet {packet.CommandType} {packet.GetDebugPacket()}");
			}
			*/

			switch ((ItelexCommands)packet.Command)
			{
				case ItelexCommands.Heartbeat:
					Logging.Instance.Debug(TAG, nameof(DecodePacket), $"recv heartbeat {packet.GetDebugPacket()}");
					break;

				case ItelexCommands.DirectDial:
					Logging.Instance.Info(TAG, nameof(DecodePacket), $"recv direct dial cmd  {packet.GetDebugPacket()}, number={packet.Data[0]}");
					int directDial = packet.Data[0];
					Message?.Invoke($"received direct dial cmd {directDial}");
					if (directDial == _config.IncomingExtensionNumber)
					{
						ConnectInit();
					}
					else
					{   // not connected
						byte[] data = Encoding.ASCII.GetBytes("nc");
						SendCmd(ItelexCommands.Reject, data);
						Message?.Invoke($"send reject ncc");
						Disconnect();
					}
					break;

				case ItelexCommands.BaudotData:
					if (packet.Len > 0)
					{
						Logging.Instance.AppendBinary(packet.Data, Logging.BinaryModes.Recv);
						string asciiStr = CodeManager.BaudotStringToAscii(packet.Data, ref _shiftState, _config.CodeSet, CodeManager.SendRecv.Recv);
						Logging.Instance.Debug(TAG, nameof(DecodePacket), $"recv baudot data {packet.Len} \"{CodeManager.AsciiToDebugStr(asciiStr)}\"");
						AddReceivedCharCount(packet.Data.Length);
						_lastSendRecvIdleMs = Helper.GetTicksMs();
						Received?.Invoke(asciiStr);
						BaudotSendRecv?.Invoke(packet.Data);
						Update?.Invoke();
					}
					break;

				case ItelexCommands.End:
					Logging.Instance.Info(TAG, nameof(DecodePacket), $"recv end cmd {packet.GetDebugPacket()}");
					Disconnect();
					break;

				case ItelexCommands.Reject:
					string reason = Encoding.ASCII.GetString(packet.Data, 0, packet.Data.Length);
					Logging.Instance.Info(TAG, nameof(DecodePacket), $"recv reject cmd {packet.GetDebugPacket()}, reason={reason}");
					Message?.Invoke($"{LngText(LngKeys.Message_Reject)} {reason.ToUpper()} ({ReasonToString(reason).ToUpper()})");
					if (CentralexState == CentralexStates.CentralexConnect && reason == "na")
					{
						CentralexState = CentralexStates.CentralexRejected;
					}
					else
					{
						Disconnect();
					}
					break;

				case ItelexCommands.Ack:
					lock (_sendLock)
					{
						_n_ack = packet.Data[0];
						_ackRecvFlag = true;
						_lastAckReceived = Helper.GetTicksMs();
					}
					//Debug.WriteLine($"recv ack cmd {_n_ack} ({CharsAckCount})");
					Logging.Instance.Debug(TAG, nameof(DecodePacket), $"recv ack cmd  {packet.GetDebugPacket()} ack={_n_ack} ({CharsAckCount})");
					Update?.Invoke();
					break;

				case ItelexCommands.ProtocolVersion:
					string versionStr = "";
					if (packet.Data.Length > 1)
					{
						// get version string
						versionStr = Encoding.ASCII.GetString(packet.Data, 1, packet.Data.Length - 1);
						versionStr = versionStr.TrimEnd('\x00'); // remove 00-byte suffix
					}
					Message?.Invoke($"received protocol version {packet.Data[0]:X2} '{versionStr}'");
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"recv protocol version cmd  {packet.GetDebugPacket()}, version={packet.Data[0]} '{versionStr}'");
					if (_connectionDirection == ConnectionDirections.In)
					{
						// answer with own version
						SendVersionCodeCmd();
					}
					break;

				case ItelexCommands.SelfTest:
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"recv self test cmd {packet.GetDebugPacket()}");
					break;

				case ItelexCommands.RemoteConfig:
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"recv remote config cmd {packet.GetDebugPacket()}");
					break;

				case ItelexCommands.RemoteConfirm:
					CentralexState = CentralexStates.CentralexConnected;
					break;

				case ItelexCommands.RemoteCall:
					SendAcceptCallRemoteCmd();
					ConnectInit();
					_connectionDirection = ConnectionDirections.In;
					if (_config.IncomingExtensionNumber != 0)
					{
						// set to connected and wait for direct dial command
						ConnectionState = ConnectionStates.TcpConnected;
					}
					else
					{
						// no direct dial command neccessary
						ConnectInit();
					}
					Message?.Invoke($"{LngText(LngKeys.Message_IncomingConnection)} centralex");
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(Listener), $"incoming connection from centralex");
					Update?.Invoke();
					break;
			}
		}

		private void AddReceivedCharCount(int n)
		{
			_n_recv = (_n_recv + n) & 0xFF;
		}

		private void AddTransCharCount(int n)
		{
			_n_trans = (_n_trans + n) & 0xFF;
		}

		private string ReasonToString(string reason)
		{
			switch(reason.ToLower())
			{
				case "occ":
					return "occupied";
				case "abs":
					return "temporarily disabled";
				case "na":
					return "not allowed";
				case "nc":
					return "not connected";
				case "der":
					return "derailed";
				default:
					return reason;
			}
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		public void Dispose()
		{
			Disconnect();
		}

	}
}
