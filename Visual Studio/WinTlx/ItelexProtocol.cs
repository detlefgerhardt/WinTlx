using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Languages;

namespace WinTlx
{
	public enum ItelexCommands
	{
		Heartbeart = 0,
		DirectDial = 1,
		BaudotData = 2,
		End = 3,
		Reject = 4,
		Ack = 6,
		ProtocolVersion = 7,
		SelfTest = 8,
		RemoteConfig = 9
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
		//private byte[] _incomingData = new byte[0];

		private Timer _sendTimer;
		private bool _sendTimerActive;
		private Timer _ackTimer;
		private bool _ackTimerActive;
		private long _lastSentMs;

		private ConfigData _config => ConfigManager.Instance.Config;

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

		private DateTime _lastSendRecvTime;
		private int _lastActivityTimer;
		public int IdleTimer { get; set; }

		private EyeballChar _eyeballChar;
		public bool EyeballCharActive { get; set; }

		private ShiftStates _shiftState;
		public ShiftStates ShiftState
		{
			get {return _shiftState; }
			set { _shiftState = value; }
		}

		//private CodeConversion.ShiftStates _echoShiftState;

		//private bool _startAck;
		private int _n_recv;
		private int _n_trans;
		private int _n_ack;
		private Queue<byte> _sendBuffer;
		private object _sendBufferLock = new object();

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
				int trans = _n_trans;
				if (_n_ack > trans)
					trans += 256;
				return trans - _n_ack;
			}
		}

		public bool Local { get; set; } = true;

		public bool IsConnected => ConnectionState != ConnectionStates.Disconnected && ConnectionState != ConnectionStates.ItelexDisconnected;

		public bool RecvOn { get; set; }

		public ConnectionStates ConnectionState { get; set; }
		private bool _incoming = false;
		//private bool _outgoing = false;

		public string ConnectionStateString
		{
			get
			{
				switch(ConnectionState)
				{
					case ConnectionStates.Disconnected:
						return "Disconnected";
					case ConnectionStates.TcpConnected:
						return "TCP connection";
					case ConnectionStates.Connected:
						if (_incoming)
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
		/// Contructor
		/// </summary>
		private ItelexProtocol()
		{
			_eyeballChar = EyeballChar.Instance;
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
					_incoming = true;
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
					_incoming = false;
				}
				catch (Exception ex)
				{
					if (ex.HResult != -2147467259)
					{
						Logging.Instance.Error(TAG, nameof(Listener), "", ex);
					}
				}
			}
		}

		public async Task<bool> ConnectOut(string host, int port, int extensionNumber, bool asciiMode=false)
		{
			bool status = false;
			await Task.Run(() =>
			{
				try
				{
					_client = new TcpClient(host, port);
					if (_client == null || !_client.Connected)
					{
						Logging.Instance.Log(LogTypes.Info, TAG, nameof(Listener), $"outgoing connection {host}:{port} failed");
						return false;
					}
				}
				catch (Exception ex)
				{
					Logging.Instance.Error(TAG, nameof(ConnectOut), "", ex);
					return false;
				}

				//Logging.Instance.Debug(TAG, nameof(SetRecvOff), "outgoing connection to {host}:{port}");

				ConnectionState = ConnectionStates.TcpConnected;
				StartReceive();

				SendVersionCodeCmd();
				Update?.Invoke();
				//if (extensionNumber != 0)
				{
					//Message?.Invoke($"send direct dial cmd {extensionNumber}");
					SendDirectDialCmd(extensionNumber);
				}
				ConnectInit();

				Logging.Instance.Log(LogTypes.Info, TAG, nameof(Listener), $"outgoing connection {host}:{port}");

				if (asciiMode)
				{
					ConnectionState = ConnectionStates.AsciiTexting;
					SendAsciiText("\r\n");
				}
				else
				{
					SendCode(CodeManager.BAU_FIGS, ref _shiftState);
				}
				Update?.Invoke();

				//_outgoing = true;
				status = true;
				return true;
			});

			return status;
		}

		private void ConnectInit()
		{
			Debug.WriteLine($"Start {nameof(ConnectInit)}");

			_n_recv = 0;
			_n_trans = 0;
			_n_ack = 0;
			//_startAck = false;
			_sendBuffer = new Queue<byte>();
			//_ackTimerActive = false;
			_ackTimer = new Timer(2000);
			_ackTimer.Elapsed += AckTimer_Elapsed;
			_ackTimer.Start();

			_sendTimerActive = false;
			_sendTimer = new Timer(100);
			_sendTimer.Elapsed += SendTimer_Elapsed;
			_sendTimer.Start();

			EyeballCharActive = false;

			_shiftState = ShiftStates.Unknown;
			//_echoShiftState = CodeConversion.ShiftStates.Unknown;
			_lastActivityTimer = 0;

			Local = false;
			SetLastSendRecv();

			_connStartTime = DateTime.Now;

			ConnectionState = ConnectionStates.Connected;

			Update?.Invoke();
			Connected?.Invoke();

			Debug.WriteLine($"End {nameof(ConnectInit)}");
		}

		public void Disconnect()
		{
			lock (DisconnectLock)
			{
				if (ConnectionState == ConnectionStates.Disconnected)
				{
					return;
				}

				_ackTimer.Stop();
				_sendTimer.Stop();

				ConnectionState = ConnectionStates.Disconnected;
				_client.Close();

				Local = false;

				Logging.Instance.Log(LogTypes.Info, TAG, nameof(Disconnect), $"connection dropped");

				Dropped?.Invoke();
				Update?.Invoke();
			}
		}

		private void SendTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!IsConnected || _ackTimerActive || _sendTimerActive)
			{
				//Debug.WriteLine("!connected");
				return;
			}

			_sendTimerActive = true;

			if (_config.IdleTimeout > 0)
			{
				IdleTimer = (int)(_config.IdleTimeout - DateTime.Now.Subtract(_lastSendRecvTime).Ticks / 10000000);
				if (IdleTimer < 0)
					IdleTimer = 0;
				if (IdleTimer != _lastActivityTimer)
				{
					Update?.Invoke();
				}
				_lastActivityTimer = IdleTimer;

				if (IdleTimer == 0)
				{
					Message?.Invoke(LngText(LngKeys.Message_IdleTimeout));
					SendEndCmd();
					Disconnect();
					_sendTimerActive = false;
					return;
				}
			}

			if (_sendBuffer.Count == 0)
			{   // nothing to send
				_sendTimerActive = false;
				return;
			}

			if (Helper.GetTicksMs() - _lastSentMs < Constants.WAIT_BEFORE_SEND_MSEC)
			{
				// wait xxx sec before sending
				_sendTimerActive = false;
				return;
			}

			//if (CharsAckCount >= Constants.WAIT_BEFORE_SEND_ACK)
			//{
			//	Debug.WriteLine($"CharsAckCount={CharsAckCount}");
			//}

			if (CharsAckCount <= Constants.WAIT_BEFORE_SEND_ACK && _sendBuffer.Count > 0)
			{
				// all characters processed at receiver side
				int cnt = _sendBuffer.Count;
				if (cnt > Constants.WAIT_BEFORE_SEND_ACK)
					cnt = Constants.WAIT_BEFORE_SEND_ACK;
				byte[] baudotData = new byte[cnt];
				lock (_sendBufferLock)
				{
					for (int i = 0; i < cnt && _sendBuffer.Count > 0; i++)
					{
						try
						{
							baudotData[i] = _sendBuffer.Dequeue();
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.Message);
						}
					}
				}
				SendCmd(ItelexCommands.BaudotData, baudotData);
				Update?.Invoke();
			}

			_sendTimerActive = false;
		}

		private void AckTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!IsConnected || ConnectionState == ConnectionStates.AsciiTexting || _ackTimerActive)
			{
				return;
			}

			_ackTimerActive = true;

			// send ack
			byte[] data = new byte[] { (byte)_n_recv };
			SendCmd(ItelexCommands.Ack, data);

			_ackTimerActive = false;
		}

		private void StartReceive()
		{
			if (ConnectionState == ConnectionStates.Disconnected)
			{
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

		public void SendAsciiChar(char asciiChr)
		{
			SetLastSendRecv();
			string asciiStr = asciiChr.ToString();
			SendAsciiText(asciiStr);
		}

		public void SendAsciiText(string asciiStr)
		{
			if (string.IsNullOrEmpty(asciiStr))
			{
				return;
			}

			SetLastSendRecv();

			string telexData = CodeManager.AsciiStringToTelex(asciiStr, _config.CodeSet);
			Send?.Invoke(telexData);

			if (ConnectionState == ConnectionStates.AsciiTexting)
			{
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
			if (ConnectionState != ConnectionStates.Disconnected)
			{
				Logging.Instance.Log(LogTypes.Info, TAG, nameof(SendDirectDialCmd), $"code={code}");
				byte[] data = new byte[] { (byte)code };
				SendCmd(ItelexCommands.DirectDial, data);
			}
		}

		public void SendVersionCodeCmd()
		{
			if (ConnectionState != ConnectionStates.Disconnected)
			{
				string version = Helper.GetVersionCode();
				byte[] versionData = Encoding.ASCII.GetBytes(version);
				byte[] data = new byte[versionData.Length + 1];
				data[0] = 1;    // version 1
				Buffer.BlockCopy(versionData, 0, data, 1, versionData.Length);
				SendCmd(ItelexCommands.ProtocolVersion, data);
			}
		}

		public void SendEndCmd()
		{
			if (ConnectionState != ConnectionStates.Disconnected)
			{
				SendCmd(ItelexCommands.End);
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
					byte[] newCodes =
							_eyeballChar.GetPunchCodes(codes[i], shiftState);
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

			lock (_sendBufferLock)
			{
				for (int i = 0; i < codes.Length; i++)
				{
					_sendBuffer.Enqueue(codes[i]);
				}
			}
			Update?.Invoke();
			_lastSentMs = Helper.GetTicksMs();
		}

		public void SendBaudotCode(byte code)
		{
			string asciiStr = CodeManager.BaudotStringToAscii(new byte[] { code }, ref _shiftState, _config.CodeSet, CodeManager.SendRecv.Send);
			Send?.Invoke(asciiStr);

			if (IsConnected && !Local)
			{
				lock (_sendBufferLock)
				{
					_sendBuffer.Enqueue(code);
				}
			}

			Update?.Invoke();
			_lastSentMs = Helper.GetTicksMs();
		}

		public void SendCmd(ItelexCommands cmd, byte[] data = null)
		{
			if (ConnectionState == ConnectionStates.Disconnected)
			{
				return;
			}

			SendCmd((int)cmd, data);
		}

		public void SendCmd(int cmdCode, byte[] data = null)
		{
			if (ConnectionState == ConnectionStates.Disconnected)
			{
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
					break;
				case ItelexCommands.Heartbeart:
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

		private void EndSend(IAsyncResult ar)
		{
			if (ConnectionState == ConnectionStates.Disconnected)
			{
				return;
			}

			try
			{
				_client.Client.EndSend(ar);
			}
			catch
			{
			}
		}

		private void DataReceived(IAsyncResult ar)
		{
			if (ConnectionState == ConnectionStates.Disconnected)
			{
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
				SetLastSendRecv();
			}
			else
			{	// i-telex
				int dataPos = 0;
				while (dataPos + 2 <= newData.Length)
				{
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
						DecodePacket(packet);
					}
				}
			}

			//_incomingData = new byte[0];
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
			if (packet.CommandType != ItelexCommands.Ack)
			{
				if (packet.CommandType != ItelexCommands.Ack)
				{
					//Logging.Instance.Log(LogTypes.Debug, TAG, nameof(DecodePacket),
					//		$"Recv packet {packet.CommandType} {packet.Command:X02} {packet.GetDebugData()}");
					Logging.Instance.Log(LogTypes.Debug, TAG, nameof(DecodePacket),
						$"Recv packet {packet.CommandType} {packet.GetDebugPacket()}");
				}
			}

			switch ((ItelexCommands)packet.Command)
			{
				case ItelexCommands.Heartbeart:
					break;
				case ItelexCommands.DirectDial:
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"direct dial command, number={packet.Data[0]}");
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
						string asciiStr = CodeManager.BaudotStringToAscii(packet.Data, ref _shiftState, _config.CodeSet, CodeManager.SendRecv.Recv);
						Logging.Instance.Log(LogTypes.Debug, TAG, nameof(DecodePacket), $"Recv {packet.CommandType} {packet.Len} \"{CodeManager.AsciiToDebugStr(asciiStr)}\"");
						AddReceivedCharCount(packet.Data.Length);
						Received?.Invoke(asciiStr);
						BaudotSendRecv?.Invoke(packet.Data);
						Update?.Invoke();
						SetLastSendRecv();
					}
					break;
				case ItelexCommands.End:
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"end command");
					Disconnect();
					break;
				case ItelexCommands.Reject:
					string reason = Encoding.ASCII.GetString(packet.Data, 0, packet.Data.Length);
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"reject command, reason={reason}");
					Message?.Invoke($"{LngText(LngKeys.Message_Reject)} {reason.ToUpper()} ({ReasonToString(reason).ToUpper()})");
					Disconnect();
					break;
				case ItelexCommands.Ack:
					_n_ack = packet.Data[0];
					//Debug.WriteLine($"Ack {_n_ack} received");
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
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"protocol version command, version={packet.Data[0]} '{versionStr}'");
					break;
				case ItelexCommands.SelfTest:
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"self test command");
					break;
				case ItelexCommands.RemoteConfig:
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"remote config command");
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

		private void SetLastSendRecv()
		{
			_lastSendRecvTime = DateTime.Now;
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
