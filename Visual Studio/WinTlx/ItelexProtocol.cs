﻿using System;
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
using WinTlx.Debugging;
using WinTlx.Languages;
using WinTlx.Tools;

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

		public const byte VersionCode = 1;

		public enum ConnectionStates
		{
			Disconnected,
			TcpConnected, // after TCP connect and before first data (texting mode unknown)
			Connected, // after direct dial cmd was received or direct dial = 0
			ItelexDisconnected // waiting for reconnect
		}

		public enum Textings
		{
			Unknown,
			Ascii,
			Itelex
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

		//public delegate void ConnectEventHandler();
		//public event ConnectEventHandler Connected;

		public delegate void DroppedEventHandler(string rejectReason);
		public event DroppedEventHandler Dropped;

		public delegate void ReceivedEventHandler(string asciiChrs);
		public event ReceivedEventHandler Received;

		public delegate void UpdateEventHandler();
		public event UpdateEventHandler Update;

		public delegate void SendEventHandler(string asciiText);
		public event SendEventHandler Send;

		public delegate void BaudotSendRecvEventHandler(byte[] code);
		public event BaudotSendRecvEventHandler BaudotSendRecv;

		public delegate void MessageEventHandler(string asciiText, bool isTechMsg);
		public event MessageEventHandler Message;

		private const int RECV_BUFFERSIZE = 2048;

		private TcpListener _tcpListener;

		private readonly object DisconnectLock = new object();

		private TcpClientWithTimeout _tcpClientWithTimeout;
		private TcpClient _tcpClient;

		private readonly System.Timers.Timer _sendTimer;
		private bool _sendTimerActive;

		private readonly System.Timers.Timer _centralexReconnectTimer;

		private readonly System.Timers.Timer _ackTimer;
		private bool _ackTimerActive;
		//private int _lastRecvAckValue;
		//private bool _ackRecvFlag;

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

		private readonly byte[] _itelixSendBuffer = new byte[Constants.ITELIX_SENDBUFFER_SIZE];
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
					return "-";
				}
				else
				{
					int secs = (int)(DateTime.Now.Subtract(_connStartTime.Value).Ticks / 10000000);
					if (secs < 120)
					{
						return $"{secs} s";
					}
					else
					{
						return $"{secs / 60} m";
					}
				}
			}
		}

		private ConfigData _config => ConfigManager.Instance.Config;

		private DebugManager _debugManager;

		private readonly EyeballChar _eyeballChar;
		public bool EyeballCharActive { get; set; }

		public string OwnVersion { get; set; }

		private KeyStates _keyStates { get; set; } = new KeyStates();

		public ShiftStates ShiftState
		{
			get { return _keyStates.ShiftState; }
			set { _keyStates.ShiftState = value; }
		}

		private Acknowledge _ack;

		private ConcurrentQueue<byte> _sendBuffer;
		private readonly object _sendLock = new object();

		public int SendBufferCount
		{
			get
			{
				return _sendBuffer != null ? _sendBuffer.Count : 0;
			}
		}

		public bool IsConnected => ConnectionState != ConnectionStates.Disconnected && ConnectionState != ConnectionStates.ItelexDisconnected;

		public bool RecvOn { get; set; }

		private ConnectionStates _connectionState = ConnectionStates.Disconnected;
		public ConnectionStates ConnectionState
		{
			get
			{
				return _connectionState;
			}
			set
			{
				_connectionState = value;
				_debugManager.WriteLine($"connectionState = {_connectionState}", DebugManager.Modes.Message);
			}
		}

		private volatile Textings _texting;
		public Textings Texting
		{
			get { return _texting; }
			private set { _texting = value; }
		}

		private ConnectionDirections _connectionDirection;

		public string RejectReason = null;

		public string RemoteVersion { get; set; }

		private CentralexStates CentralexState { get; set; }
		private string _centralexHost;
		private int _centralexPort;

		public string ConnectionStateString
		{
			get
			{
				if (Texting != Textings.Unknown)
				{
					switch(Texting)
					{
						case Textings.Ascii:
							return "ASCII";
						case Textings.Itelex:
							return "i-Telex";
					}
				}

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

			_debugManager = DebugManager.Instance;

			_ack = new Acknowledge();

			_ackTimer = new System.Timers.Timer();
			_ackTimer.Elapsed += AckTimer_Elapsed;

			_sendTimer = new System.Timers.Timer();
			_sendTimer.Elapsed += SendTimer_Elapsed;

			_centralexReconnectTimer = new System.Timers.Timer();
			_centralexReconnectTimer.Elapsed += CentralexReconnectTimer_Elapsed;

			ConfigManager.Instance.ConfigChanged += Instance_ConfigChanged;

			_keyStates = new KeyStates(ShiftStates.Unknown, _config.CodeSet);
		}

		private void Instance_ConfigChanged()
		{
			_keyStates = new KeyStates(_keyStates.ShiftState, _config.CodeSet);
			if (IsConnected)
			{
				int ms = _config.SendFreqMs == 0 ? Constants.DEFAULT_SEND_FREQ_MS : _config.SendFreqMs;
				_sendTimer.Stop();
				_sendTimer.Interval = ms;
				_sendTimer.Start();
			}
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
					if (!RecvOn) return;

					// wait for connection
					_tcpClient = _tcpListener.AcceptTcpClient();

					ConnectIn();
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

		private void ConnectIn()
		{
			if (_config.IncomingExtensionNumber != 0)
			{
				// set to connected and wait for direct dial command
				//ConnectionState = ConnectionStates.TcpConnected;
			}
			else
			{
				// no direct dial command neccessary
				//ConnectInit();
			}
			ConnectInit();
			ConnectionState = ConnectionStates.TcpConnected;
			_connectionDirection = ConnectionDirections.In;
			StartReceive();

			IPAddress remoteAddr = ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Address;
			if (_config.ShowTechnicalMessages)
			{
				Message?.Invoke($"incomming connection {remoteAddr}", true);
			}
			else
			{
				Message?.Invoke($"{LngText(LngKeys.Message_IncomingConnection)}", false);
			}
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(Listener), $"incoming connection from {remoteAddr}");

			Update?.Invoke();

			TickTimer timer = new TickTimer();
			while (true)
			{
				if (timer.ElapsedMilliseconds > 3000)
				{
					_debugManager.WriteLine($"Texting mode timeout -> unknown", DebugManager.Modes.Recv);
					break;
				}
				if (ConnectionState == ConnectionStates.Disconnected)
				{
					_debugManager.WriteLine($"Disconnected", DebugManager.Modes.Recv);
					return;
				}
				if (Texting != Textings.Unknown)
				{
					break;
				}
				Thread.Sleep(100);
			}
			if (Texting == Textings.Unknown)
			{
				Texting = Textings.Ascii;
				_debugManager.WriteLine($"Texting mode unknown -> ASCII", DebugManager.Modes.Recv);
			}
			else
			{
				_debugManager.WriteLine($"Texting mode = {Texting}", DebugManager.Modes.Recv);
			}
			Logging.Instance.Debug(TAG, nameof(Listener), $"ConnectionState={ConnectionState} Texting={Texting} ElapsedMilliseconds={timer.ElapsedMilliseconds}ms");
			//_debugManager.WriteLine($"Texting = {Texting}", DebugManager.Modes.Message);
			ConnectionState = ConnectionStates.Connected;

			return;
		}

		public async Task<bool> ConnectOut(string host, int port, int? extensionNumber, bool asciiMode = false)
		{
			RejectReason = null;
			bool status = await Task.Run(() =>
			{
				string logMsg;
				try
				{
					logMsg = $"connecting to host={host}:{port}";
					string ipv4Str = SubscriberServer.SelectIp4Addr(host);
					BufferManager.Instance.LocalOutputMessage(
						LngText(LngKeys.Message_Connecting),
						logMsg,
						true, true);
					Logging.Instance.Info(TAG, nameof(ConnectOut), logMsg);

					logMsg = $"ipv4={ipv4Str}:{port}";
					BufferManager.Instance.LocalOutputMessage(null, logMsg, true, true);
					Logging.Instance.Info(TAG, nameof(ConnectOut), logMsg);

					if (string.IsNullOrEmpty(ipv4Str))
					{
						string dnsErrMsg = "dns request failed";
						BufferManager.Instance.LocalOutputMessage(LngText(LngKeys.Message_ConnectionError), dnsErrMsg, true, true);
						Logging.Instance.Warn(TAG, nameof(ConnectOut), dnsErrMsg);
						return false;
					}

					string asciiMsg = asciiMode ? "(ascii)" : "";
					_debugManager.WriteLine($"connect to {host}:{port} ext={extensionNumber} {asciiMsg} ipv4={ipv4Str}", DebugManager.Modes.Message);

					_tcpClientWithTimeout = new TcpClientWithTimeout(ipv4Str, port, Constants.TCPCLIENT_TIMEOUT);
					_tcpClient = _tcpClientWithTimeout.Connect();
					if (_tcpClient == null || !_tcpClient.Connected)
					{
						logMsg = "connect failed, timeout";
						BufferManager.Instance.LocalOutputMessage(LngText(LngKeys.Message_ConnectionError), logMsg, true, true);
						Logging.Instance.Info(TAG, nameof(ConnectOut), logMsg);
						return false;
					}
				}
				catch (Exception)
				{
					logMsg = "connect failed, timeout";
					BufferManager.Instance.LocalOutputMessage(LngText(LngKeys.Message_ConnectionError), logMsg, true, true);
					Logging.Instance.Info(TAG, nameof(ConnectOut), logMsg);
					return false;
				}

				// connected

				logMsg = $"connected to {host}:{port}";
				BufferManager.Instance.LocalOutputMessage(LngText(LngKeys.Message_Connected), logMsg, true, true);
				Logging.Instance.Info(TAG, nameof(ConnectOut), $"logMsg");

				ConnectInit();
				ConnectionState = ConnectionStates.TcpConnected;
				_connectionDirection = ConnectionDirections.Out;

				StartReceive();

				//SendEndCmd();
				//return false;

				TickTimer waitTexting = new TickTimer();
				while (!waitTexting.IsElapsedMilliseconds(2000) && Texting == Textings.Unknown)
				{
				}
				if (Texting == Textings.Ascii) asciiMode = true;

				//Debug.WriteLine($"Connect out: {asciiMode} ");

				if (!asciiMode)
				{
					return ConnectOutItelex(extensionNumber);
				}
				else
				{
					return ConnectOutAscii(extensionNumber);
				}
			});
			return status;
		}

		private bool ConnectOutItelex(int? extensionNumber)
		{
			TickTimer timer = new TickTimer();
			while (timer.ElapsedMilliseconds < 500)
			{
				if (Texting != Textings.Unknown) break;
				if (ConnectionState == ConnectionStates.Disconnected) return false;
				//await Task.Delay(100);
				Thread.Sleep(100);
			}

			SendHeartbeatCmd();
			SendVersionCodeCmd();

			// wait 10 seconds for version cmd and extension cmd from remote

			timer = new TickTimer();
			timer.Start();
			while (!timer.IsElapsedMilliseconds(10000))
			{
				if (RemoteVersion != null) break;
				if (ConnectionState == ConnectionStates.Disconnected) return false;
				//await Task.Delay(100);
				Thread.Sleep(100);
			}

			if (RemoteVersion == null)
			{
				Logging.Instance.Info(TAG, nameof(ConnectOut), $"Timeout waiting for version response");
				if (_config.ShowTechnicalMessages)
				{
					Message?.Invoke($"no version packet received", true);
				}
			}
			else
			{
				if (_config.ShowTechnicalMessages)
				{
					Message?.Invoke($"received version cmd {RemoteVersion}", true);
				}
				Logging.Instance.Info(TAG, nameof(ConnectOut), $"received version {RemoteVersion}");
			}

			if (ConnectionState == ConnectionStates.Disconnected) return false;
			if (Texting == Textings.Unknown)
			{
				Texting = _config.DefaultProtocolAscii ? Textings.Ascii : Textings.Itelex;
				_debugManager.WriteLine($"set protocol to default {Texting}", DebugManager.Modes.Message);
			}

			if (Texting == Textings.Ascii) return true;

			if (_config.ShowTechnicalMessages)
			{
				Message?.Invoke($"send direct dial cmd {extensionNumber}", true);
			}
			SendDirectDialCmd(extensionNumber.GetValueOrDefault());

			ConnectionState = ConnectionStates.Connected;
			Update?.Invoke();

			// wait for reject command
			timer.Start();
			while (timer.ElapsedMilliseconds < 2000)
			{
				if (ConnectionState == ConnectionStates.Disconnected) return false;
				//await Task.Delay(100);
				Thread.Sleep(100);
			}

			return true;
		}

		public bool ConnectOutAscii(int? extensionNumber)
		{
			//await Task.Delay(1000);

			Texting = Textings.Ascii;
			if (_config.ShowTechnicalMessages)
			{
				Message?.Invoke($"ascii texting", true);
			}
			Logging.Instance.Info(TAG, nameof(ConnectOut), $"ascii texting");
			//_debugManager.WriteLine($"ConnectionState1 = {ConnectionState}", DebugManager.Modes.Recv);
			int ext = extensionNumber.GetValueOrDefault();
			if (extensionNumber.GetValueOrDefault() != 0)
			{
				SendAsciiText($"*{ext}*");
			}
			//SendAsciiText("\r\n");
			ConnectionState = ConnectionStates.Connected;
			Update?.Invoke();
			return true;
		}

		public async Task<bool> CentralexConnectAsync(string host, int port)
		{
			_centralexHost = host;
			_centralexPort = port;

			CentralexConnectResults result = await Task.Run(() => CentralexConnect());

			bool success = false;
			string msg = "";
			switch (result)
			{
				case CentralexConnectResults.Ok:
					msg = $"connected to centralex {host}:{port}";
					_debugManager.WriteLine($"CentralexState = {CentralexState} {msg}", DebugManager.Modes.Recv);
					RecvOn = true;
					success = true;
					break;
				case CentralexConnectResults.TcpError:
					msg = $"tcp connection failed {host}:{port}";
					success = false;
					break;
				case CentralexConnectResults.TimeoutError:
					msg = $"centralex connection timeout {host}:{port}";
					success = false;
					break;
				case CentralexConnectResults.AuthError:
					msg = $"authentication error {host}:{port}";
					success = false;
					break;
				case CentralexConnectResults.OtherError:
					msg = $"centralex error {host}:{port}";
					success = false;
					break;
				default:
					success = false;
					break;
			}

			Logging.Instance.Info(TAG, nameof(CentralexConnectAsync), msg);
			_debugManager.WriteLine(msg, DebugManager.Modes.Message);
			if (_config.ShowTechnicalMessages)
			{
				Message?.Invoke(msg, true);
			}
			else
			{
				if (success)
				{
					Message?.Invoke(LngText(LngKeys.Message_CentralexConnected), false);
				}
				else
				{
					Message?.Invoke(LngText(LngKeys.Message_CentralexError), false);
				}
			}

			Update?.Invoke();
			return success;
		}

		private CentralexConnectResults CentralexConnect()
		{
			try
			{
				_tcpClient = new TcpClient(_centralexHost, _centralexPort);
				if (_tcpClient == null || !_tcpClient.Connected)
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
			_debugManager.WriteLine($"CentralexState = {CentralexState}", DebugManager.Modes.Message);

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
				if (sw.ElapsedMilliseconds > 10000)
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
			SendEndCmd();
			Thread.Sleep(2000);
			CentralexState = CentralexStates.None;
			_debugManager.WriteLine($"CentralexState = {CentralexState}", DebugManager.Modes.Message);
			Disconnect();
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
			_ack.Reset();

			_sendBuffer = new ConcurrentQueue<byte>();

			//_ackRecvFlag = false;
			_ackTimerActive = false;
			_ackTimer.Stop();
			SetAckTimerInterval(0); // set _ackTimer to default interval
			_ackTimer.Interval = 1000;
			_ackTimer.Start();

			_sendTimerActive = false;
			_sendTimer.Stop();
			_sendTimer.Interval = _config.SendFreqMs;
			_sendTimer.Start();

			EyeballCharActive = false;

			_keyStates = new KeyStates(ShiftStates.Unknown, _config.CodeSet);
			_lastSendRecvIdleMs = Helper.GetTicksMs();

			RejectReason = null;
			RemoteVersion = null;

			_connStartTime = DateTime.Now;

			Texting = Textings.Unknown;
			//ConnectionState = ConnectionStates.Connected;

			_debugManager.InitConnection();

			Update?.Invoke();
		}

		public void SetAckTimerInterval(int ms)
		{
			if (ms == 0) ms = 2000; // default is 2 sec.
			_ackTimer.Stop();
			_ackTimer.Interval = ms;
			_ackTimer.Start();
		}

		private bool _disconnecting = false;
		public void Disconnect()
		{
			if (ConnectionState == ConnectionStates.Disconnected) return;
			if (_disconnecting) return;

			_disconnecting = true;

			_ackTimer?.Stop();
			_sendTimer?.Stop();

			ConnectionState = ConnectionStates.Disconnected;

			_tcpClient?.Close();

			Logging.Instance.Info(TAG, nameof(Disconnect), $"connection dropped");

			Dropped?.Invoke(RejectReason);
			Update?.Invoke();

			if (CentralexState == CentralexStates.CentralexConnected)
			{
				//CentralexState = CentralexStates.CentralexConnect;
				//_debugManager.WriteLine($"CentralexState = {CentralexState}", DebugManager.Modes.Message);
				_centralexReconnectTimer.Interval = 5000;
				_centralexReconnectTimer.Start();
			}
			_disconnecting = false;
		}

		private void SendTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!IsConnected || _ackTimerActive || _sendTimerActive) return;

			try
			{
				_sendTimerActive = true;
				if (_tcpClient != null && !_tcpClient.Connected)
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

		/**
		 * send baudot data from _sendBuffer, use settings for max send buffer size and max remote buffer size
		 * send after WAIT_BEFORE_SEND_MSEC, when buffer is not full
		 */
		private void SendTimer()
		{
			int remoteBufferCount = _ack.RemoteBufferCount;
			for (int i = 0;
				!_sendBuffer.IsEmpty && _itelixSendCount < Constants.ITELIX_SENDBUFFER_SIZE && remoteBufferCount < _config.RemoteBufferSize;
				i++)
			{
				if (!_sendBuffer.TryDequeue(out byte baudotCode)) break;
				_itelixSendBuffer[_itelixSendCount++] = baudotCode;
				remoteBufferCount++;
			}

			if (_itelixSendCount == 0) return;

			// wait, if buffer not full
			if (_itelixSendCount < Constants.ITELIX_SENDBUFFER_SIZE &&
				Helper.GetTicksMs() - _lastSentMs < Constants.WAIT_BEFORE_SEND_MSEC)
			{
				return;
			}

			byte[] baudotData = new byte[_itelixSendCount];
			Buffer.BlockCopy(_itelixSendBuffer, 0, baudotData, 0, _itelixSendCount);
			SendBaudotCmd(baudotData);
			_ack.AddTransCharCount(_itelixSendCount);
			Logging.Instance.AppendBinary(baudotData, Logging.BinaryModes.Send);
			_itelixSendCount = 0;
			_lastSentMs = Helper.GetTicksMs();
			_lastSendRecvIdleMs = Helper.GetTicksMs();
		}

		private void AckTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_ackTimerActive) return;

			try
			{
				_ackTimerActive = true;

				if (CentralexState == CentralexStates.CentralexConnected && !IsConnected)
				{
					SendHeartbeatCmd();
					return;
				}

				if (ConnectionState != ConnectionStates.Connected || Texting == Textings.Ascii) return;

				// check received acks

				if (_ack.RemoteBufferCount > 0)
				{
					string timeout = null;
					if (_ack.IsLastAckCntTimeout())
					{
						// if ack was received and ack didn't change since the last AckTimer_Elapsed (2 sec)
						Logging.Instance.Warn(TAG, nameof(AckTimer_Elapsed), $"recv ack value changed timeout");
						timeout = "nochange timeout";
					}
					//_ackRecvFlag = false;

					if (_ack.LastAckReceived.IsElapsedSeconds(10))
					{
						Logging.Instance.Warn(TAG, nameof(AckTimer_Elapsed), $"ack recv timeout");
						timeout = "ack timeout";
					}
					if (timeout != null)
					{
						_debugManager.WriteLine($"{timeout} {_ack.LastAckChanged.ElapsedMilliseconds}", DebugManager.Modes.Message);
						//_ack.SendCnt = _ack.SendAckCnt;
					}
				}

				SendAckCmd(_ack.ReceivedAckCnt);
			}
			finally
			{
				_ackTimerActive = false;
			}
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
			if (string.IsNullOrEmpty(asciiStr)) return;

			string telexData = CodeManager.AsciiStringToTelex(asciiStr, _config.CodeSet);
			Send?.Invoke(CodeManager.AsciiWithBaudotEscCodeToAscii(asciiStr, _keyStates));

			if (Texting == Textings.Ascii)
			{
				if (ConnectionState == ConnectionStates.Disconnected) return;
				if (_tcpClient.Client == null)
				{
					Disconnect();
					return;
				}

				asciiStr = asciiStr.Replace("\x05", "@\x17");
				//asciiStr = asciiStr.Replace("\x05", "\x17");
				//asciiStr = asciiStr.Replace("\x05", "\x05");
				asciiStr = asciiStr.Replace(CodeManager.ASC_LTRS.ToString(), "");
				asciiStr = asciiStr.Replace(CodeManager.ASC_FIGS.ToString(), "");
				//asciiStr = asciiStr.Replace(CodeManager.ASC_BEL, '\x07');

				byte[] data = Encoding.ASCII.GetBytes(asciiStr);
				_tcpClient.Client.BeginSend(data, 0, data.Length, SocketFlags.None, EndSend, null);
			}
			else
			{
				for (int c = 0; c < asciiStr.Length; c++)
				{
					byte[] baudotData = CodeManager.AsciiStringToBaudot(asciiStr[c].ToString(), _keyStates);
					for (int i = 0; i < baudotData.Length; i++)
					{
						byte chr = baudotData[i];
						ShiftStates shiftState = ShiftState;
						SendCode(baudotData[i], ref shiftState);
						ShiftState = shiftState;
					}
				}
				Update?.Invoke();
			}
		}

		public void SendBaudotCmd(byte[] data)
		{
			SendCmd(ItelexCommands.BaudotData, data);
		}

		public void SendDirectDialCmd(int code)
		{
			Logging.Instance.Info(TAG, nameof(SendDirectDialCmd), $"send direct dial command extension={code}");
			byte[] data = new byte[] { (byte)code };
			SendCmd(ItelexCommands.DirectDial, data);
		}

		public void SendAckCmd(int ackVal)
		{
			byte[] data = new byte[] { (byte)ackVal };
			SendCmd(ItelexCommands.Ack, data);
		}

		public void SendHeartbeatCmd()
		{
			SendCmd(ItelexCommands.Heartbeat);
		}

		public void SendRejectCmd(string reason)
		{
			Logging.Instance.Info(TAG, nameof(SendRejectCmd), $"reason={reason}");
			byte[] data = Encoding.ASCII.GetBytes(reason);
			SendCmd(ItelexCommands.Reject, data);
		}

		public void SendVersionCodeCmd()
		{
			string version = !string.IsNullOrEmpty(OwnVersion) ? OwnVersion : Helper.GetItelexVersion(Constants.APP_CODE);
			if (_config.ShowTechnicalMessages)
			{
				Message?.Invoke($"send version cmd {VersionCode} '{version}'", true);
			}
			List<byte> data = new List<byte>();
			data.Add(VersionCode);
			int len = version.Length <= 5 ? version.Length : 5;

			for (int i=0; i<len; i++)
			{
				//if (char.IsDigit(version[i])) data.Add((byte)version[i]);
				data.Add((byte)version[i]);
			}
			if (data.Count < 6) data.Add(0x00);

			// data = new List<byte>() { 1 };
			SendCmd(ItelexCommands.ProtocolVersion, data.ToArray());
		}

		public void SendEndCmd()
		{
			Logging.Instance.Info(TAG, nameof(SendEndCmd), $"send end cmd");
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
			if (CentralexState == CentralexStates.CentralexConnected)
			{
				SendCmd(ItelexCommands.AcceptCallRemote);
			}
		}

		private void SendCode(byte baudotCode, ref ShiftStates shiftState)
		{
			byte[] codes = CodeManager.BaudotCodeToBaudotWithShift(baudotCode, shiftState, _keyStates);

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

			EnqueueSend(codes);
			Update?.Invoke();
		}

		public void SendBaudotCode(byte code)
		{
			string asciiStr = CodeManager.BaudotStringToAscii(new byte[] { code }, _keyStates, CodeManager.SendRecv.Send, false);
			Send?.Invoke(asciiStr);
			if (!IsConnected)
			{
				return;
			}

			EnqueueSend(new byte[] { code });
			Update?.Invoke();
		}

		private void EnqueueSend(byte[] codes)
		{
			for (int i = 0; i < codes.Length; i++)
			{
				_sendBuffer.Enqueue(codes[i]);
			}
		}

		public void SendCmd(ItelexCommands cmd, byte[] data = null)
		{
			SendCmd((int)cmd, data);
		}

		public void SendCmd(int cmdCode, byte[] data = null)
		{
			if (ConnectionState == ConnectionStates.Disconnected && CentralexState == CentralexStates.None) return;

			//if (cmdCode == 0x03)
			//{
			//	Debug.Write("");
			//}

			if (_tcpClient.Client == null)
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
				if (packet.CommandType == ItelexCommands.BaudotData)
				{
					Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendCmd),
							$"Send packet {packet.CommandType} {packet.GetDebugPacket()} " + 
							$"(rem_ack={_ack.SendAckCnt} send={_ack.SendCnt} rem_buf={_ack.RemoteBufferCount})");
				}
				else
				{
					Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendCmd),
							$"Send packet {packet.CommandType} {packet.GetDebugPacket()}");
				}
			}

			_debugManager.WriteCmd(packet, DebugManager.Modes.Send, _ack);

			if (_tcpClient?.Client == null) return;

			try
			{
				_tcpClient.Client.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, EndSend, null);
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendCmd), "", ex);
				Disconnect();
			}
		}

		private void StartReceive()
		{
			if (ConnectionState == ConnectionStates.Disconnected && CentralexState == CentralexStates.None) return;

			if (_tcpClient.Client==null)
			{
				Disconnect();
				return;
			}

			byte[] buffer = new byte[RECV_BUFFERSIZE];
			try
			{
				_tcpClient.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, DataReceived, buffer);
			}
			catch
			{
				Disconnect();
			}
		}

		private void EndSend(IAsyncResult ar)
		{
			if (ConnectionState == ConnectionStates.Disconnected && CentralexState == CentralexStates.None) return;

			if (_tcpClient.Client == null)
			{
				Disconnect();
				return;
			}

			try
			{
				_tcpClient.Client.EndSend(ar);
			}
			catch(Exception)
			{
			}
		}

		private void DataReceived(IAsyncResult ar)
		{
			if (ConnectionState == ConnectionStates.Disconnected && CentralexState == CentralexStates.None) return;

			if (_tcpClient.Client == null)
			{
				Disconnect();
				return;
			}

			int dataReadCount;

			try
			{
				dataReadCount = _tcpClient.Client.EndReceive(ar);
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

			if (ConnectionState == ConnectionStates.TcpConnected)
			{
				if (newData[0] <= 0x09 || newData[0] >= 0x10 && newData[0] < 0x1F)
				{
					_debugManager.WriteLine($"Recv char {newData[0]:X02} -> i-Telex", DebugManager.Modes.Recv);
					List<string> dmp = Helper.DumpByteArrayStr(newData, 0);
					_debugManager.WriteLine($"Recv dump {dmp[0]}", DebugManager.Modes.Recv);
					Texting = Textings.Itelex;
				}
				else
				{
					_debugManager.WriteLine($"Recv char {newData[0]:X02} -> ASCII", DebugManager.Modes.Recv);
					Texting = Textings.Ascii;
				}
				//Debug.WriteLine($"recv {Texting}");
			}

			try
			{
				if (Texting == Textings.Ascii)
				{   // ascii
					string asciiText = Encoding.ASCII.GetString(newData, 0, newData.Length);
					//Debug.WriteLine($"recv ascii: {asciiText}");
					asciiText = asciiText.Replace('@', CodeManager.ASC_WRU);
					Received?.Invoke(asciiText);
					_lastSendRecvIdleMs = Helper.GetTicksMs();
				}
				else
				{   // i-telex
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
							//Debug.WriteLine($"{packet}");
						}
					}
				}
			}
			catch(Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(DataReceived), "error", ex);
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
			_debugManager.WriteCmd(packet, DebugManager.Modes.Recv, _ack);

			// check for empty packet
			switch (packet.CommandType)
			{
				case ItelexCommands.DirectDial:
				case ItelexCommands.BaudotData:
				case ItelexCommands.Reject:
				case ItelexCommands.ProtocolVersion:
					if (packet.Len == 0)
					{
						Logging.Instance.Warn(TAG, nameof(DecodePacket), $"empty {packet.CommandType} received");
						return;
					}
					break;
			}

			switch (packet.CommandType)
			{
				case ItelexCommands.Heartbeat:
					Logging.Instance.Debug(TAG, nameof(DecodePacket), $"recv heartbeat {packet.GetDebugPacket()}");
					break;

				case ItelexCommands.DirectDial:
					Logging.Instance.Info(TAG, nameof(DecodePacket), $"recv direct dial cmd  {packet.GetDebugPacket()}, number={packet.Data[0]}");
					int directDial = packet.Data[0];
					if (_config.ShowTechnicalMessages)
					{
						Message?.Invoke($"received direct dial cmd {directDial}", true);
					}
					if (directDial == _config.IncomingExtensionNumber)
					{
						ConnectInit();
					}
					else
					{   // invalid extension
						byte[] data = Encoding.ASCII.GetBytes("nc" + (char)0x00);
						SendCmd(ItelexCommands.Reject, data);
						if (_config.ShowTechnicalMessages)
						{
							Message?.Invoke($"send reject nc", true);
						}
						SendEndCmd();
						Thread.Sleep(2000);
						Disconnect();
					}
					break;

				case ItelexCommands.BaudotData:
					Logging.Instance.AppendBinary(packet.Data, Logging.BinaryModes.Recv);
					string asciiStr = "";
					foreach (byte code in packet.Data)
					{	// decode byte by byte to preserve ack-count
						string chrs = CodeManager.BaudotStringToAscii(new byte[] { code }, _keyStates, CodeManager.SendRecv.Recv, false);
						asciiStr += chrs;
						Received?.Invoke(chrs);
					}
					Logging.Instance.Debug(TAG, nameof(DecodePacket), $"recv baudot data {packet.Len} \"{CodeManager.AsciiToDebugStr(asciiStr)}\"");
					_ack.AddReceivedCharCount(packet.Len);
					_lastSendRecvIdleMs = Helper.GetTicksMs();
					BaudotSendRecv?.Invoke(packet.Data);
					Update?.Invoke();
					break;

				case ItelexCommands.End:
					Logging.Instance.Info(TAG, nameof(DecodePacket), $"recv end cmd {packet.GetDebugPacket()}");
					Disconnect();
					break;

				case ItelexCommands.Reject:
					RejectReason = Encoding.ASCII.GetString(packet.Data, 0, packet.Data.Length);
					RejectReason = RejectReason.TrimEnd('\x00');
					Logging.Instance.Info(TAG, nameof(DecodePacket), $"recv reject cmd {packet.GetDebugPacket()}, reason={RejectReason}");
					Message?.Invoke($"reject {RejectReason.ToUpper()} ({ReasonToString(RejectReason).ToUpper()})", false);
					/*
					if (_config.ShowTechnicalMessages)
					{
						Message?.Invoke($"reject received {RejectReason.ToUpper()} ({ReasonToString(RejectReason).ToUpper()})", true);
					}
					*/
					if (CentralexState == CentralexStates.CentralexConnect && RejectReason == "na")
					{
						CentralexState = CentralexStates.CentralexRejected;
						_debugManager.WriteLine($"CentralexState = {CentralexState}", DebugManager.Modes.Message);
					}
					else
					{
						Disconnect();
					}
					break;

				case ItelexCommands.Ack:
					_ack.SendAckCnt = packet.Data[0];
					Logging.Instance.Debug(TAG, nameof(DecodePacket),
						$"recv ack cmd  {packet.GetDebugPacket()} (rem_ack={_ack.SendAckCnt} send={_ack.SendCnt} rem_buf={_ack.RemoteBufferCount}/{ConfigManager.Instance.Config.RemoteBufferSize})");
					Update?.Invoke();
					break;

				case ItelexCommands.ProtocolVersion:
					string versionStr = "";
					if (packet.Len > 1)
					{
						// get version string
						versionStr = Encoding.ASCII.GetString(packet.Data, 1, packet.Data.Length - 1);
						versionStr = versionStr.TrimEnd('\x00'); // remove 00-byte suffix
					}
					RemoteVersion = $"{packet.Data[0]} '{versionStr}'";
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"recv protocol version cmd  {packet.GetDebugPacket()}, version={RemoteVersion}");
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
					_debugManager.WriteLine($"CentralexState = {CentralexState}", DebugManager.Modes.Message);
					break;

				case ItelexCommands.RemoteCall:
					_debugManager.WriteLine($"SendAcceptCallRemoteCmd", DebugManager.Modes.Message);
					SendAcceptCallRemoteCmd();
					Message?.Invoke($"{LngText(LngKeys.Message_IncomingConnection)} centralex", true);
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(Listener), $"incoming connection from centralex");
					_debugManager.WriteLine($"Centralex RemoteCall", DebugManager.Modes.Message);
					ConnectIn();
					Update?.Invoke();
					break;
			}
		}

		public void AddPrintedCharCount(int n)
		{
			_ack.AddPrintedCharCount(n);
		}

		public int GetRemoteBufferCount()
		{
			return _ack.RemoteBufferCount;
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

	public class Acknowledge
	{
		public int ReceivedCnt { get; set; } // characters received from remote

		public int ReceivedAckCnt { get; set; } // n_recv, characters received/printed locally

		public int SendCnt { get; set; } // n_trans, characters send to remote

		// n_ack, characters printed by remote
		private int _sendAckCnt;
		public int SendAckCnt
		{
			get
			{
				return _sendAckCnt;
			}
			set
			{
				if (value != _sendAckCnt) LastAckChanged.Start();
				_sendAckCnt = value;
			}
		}

		public TickTimer LastAckChanged { get; set; }

		public TickTimer LastAckReceived { get; set; }

		public void Reset()
		{
			ReceivedCnt = 0;
			ReceivedAckCnt = 0;
			SendCnt = 0;
			//SendAckCnt = 0;
			_sendAckCnt = 0;
			LastAckChanged = new TickTimer(false);
			LastAckReceived = new TickTimer(false);
		}

		public void AddReceivedCharCount(int n)
		{
			ReceivedCnt = (ReceivedCnt + n) % 256;
		}

		public void AddPrintedCharCount(int n)
		{
			ReceivedAckCnt = (ReceivedAckCnt + n) % 256;
		}

		public void AddTransCharCount(int n)
		{
			SendCnt = (SendCnt + n) % 256;
			LastAckChanged.Start();
		}

		public bool IsLastAckCntTimeout()
		{
			return RemoteBufferCount > 0 && LastAckChanged.IsElapsedSeconds(5);
		}

		public int RemoteBufferCount
		{
			get
			{
				int send = SendCnt;
				if (SendAckCnt > send) send += 256;
				return send - SendAckCnt;
			}
		}

		public int LocalBufferCount
		{
			get
			{
				int recv = ReceivedCnt;
				if (ReceivedAckCnt > recv) recv += 256;
				return recv - ReceivedAckCnt;
			}
		}
	}
}
