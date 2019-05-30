using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WinTelex
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
			Connected, // after TCP connect and before first data (texting mode unknown)
			AsciiTexting,
			BaudotTexting,
			BaudotDisconnected // waiting for reconnect
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

		private const int RECV_BUFFERSIZE = 255 + 2;

		private TcpListener _tcpListener;

		private TcpClient _client;
		private byte[] _incomingData = new byte[0];

		private Timer _sendTimer;
		private bool _sendTimerActive;
		private Timer _ackTimer;
		private bool _ackTimerActive;
		private long _lastSentMs;

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
		private int _lastInactivityTimer;
		public int InactivityTimer { get; set; }
		public int InactivityTimeout { get; set; }

		private CodeConversion.ShiftStates _shiftState;
		public CodeConversion.ShiftStates ShiftState
		{
			get {return _shiftState; }
			set { _shiftState = value; }
		}

		private CodeConversion.ShiftStates _echoShiftState;

		//private bool _startAck;
		private int _n_recv;
		private int _n_trans;
		private int _n_ack;
		private Queue<byte> _sendBuffer;

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

		public bool IsConnected => ConnectionState != ConnectionStates.Disconnected && ConnectionState != ConnectionStates.BaudotDisconnected;

		public bool RecvOn { get; set; }

		public ConnectionStates ConnectionState { get; set; }
		private bool _incoming = false;
		private bool _outgoing = false;

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

					Debug.WriteLine("wait for incoming connection");
					// wait for connection
					_client = _tcpListener.AcceptTcpClient();
					ConnectInit();
					_incoming = true;

					IPAddress remoteAddr = ((IPEndPoint)_client.Client.RemoteEndPoint).Address;
					Message?.Invoke($"INCOMING CONNECTION FROM {remoteAddr}");
					Debug.WriteLine($"incoming connection from {remoteAddr}");
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(Listener), $"incoming connection from {remoteAddr}");

					while (true)
					{
						if (_client == null)
							return;
						if (!_client.Connected)
							break;
					}

					Disconnect();
					_incoming = false;
					Debug.WriteLine("incoming disconnected");
				}
				catch (Exception ex)
				{
					Logging.Instance.Error(TAG, nameof(Listener), "", ex);
					Debug.WriteLine($"{_client} {ex.Message}");
				}
			}
		}

		public bool ConnectOut(string host, int port, bool asciiMode=false)
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
			catch(Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(ConnectOut), "", ex);
				Debug.WriteLine(ex.Message);
				return false;
			}

			if (!ConnectInit())
			{
				return false;
			}

			Logging.Instance.Log(LogTypes.Info, TAG, nameof(Listener), $"outgoing connection {host}:{port}");

			if (asciiMode)
			{
				ConnectionState = ConnectionStates.AsciiTexting;
				SendAsciiText("\r\n");
			}
			else
			{
				SendBaudotCode(CodeConversion.FIG_SHIFT, ref _shiftState);
				Update?.Invoke();
			}

			_outgoing = true;
			return true;
		}

		private bool ConnectInit()
		{
			_n_recv = 0;
			_n_trans = 0;
			_n_ack = 0;
			//_startAck = false;
			_sendBuffer = new Queue<byte>();
			_ackTimerActive = false;
			_ackTimer = new Timer(2000);
			_ackTimer.Elapsed += AckTimer_Elapsed;
			_ackTimer.Start();

			_sendTimerActive = false;
			_sendTimer = new Timer(100);
			_sendTimer.Elapsed += SendTimer_Elapsed;
			_sendTimer.Start();

			ConnectionState = ConnectionStates.Connected;
			_shiftState = CodeConversion.ShiftStates.Unknown;
			_echoShiftState = CodeConversion.ShiftStates.Unknown;
			_lastInactivityTimer = 0;
			StartReceive();

			Local = false;
			SetLastSendRecv();

			_connStartTime = DateTime.Now;

			Update?.Invoke();
			Connected?.Invoke();

			return true;
		}

		/*
		public void StartAck()
		{
			_startAck = true;
		}
		*/

		public void Disconnect()
		{
			if (!IsConnected)
			{
				return;
			}

			Debug.WriteLine("DropConnection");

			_ackTimer.Stop();
			_sendTimer.Stop();

			ConnectionState = ConnectionStates.Disconnected;
			_client.Close();

			Local = true;

			Logging.Instance.Log(LogTypes.Info, TAG, nameof(Disconnect), $"connection dropped");

			Dropped?.Invoke();
		}

		private void SendTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!IsConnected || _ackTimerActive)
			{
				return;
			}

			InactivityTimer = (int)(InactivityTimeout -
										DateTime.Now.Subtract(_lastSendRecvTime).Ticks / 10000000);
			if (InactivityTimer < 0)
				InactivityTimer = 0;
			if (InactivityTimer != _lastInactivityTimer)
			{
				Update?.Invoke();
			}
			_lastInactivityTimer = InactivityTimer;

			if (InactivityTimer == 0)
			{
				Message?.Invoke("INACTIVITY TIMEOUT");
				SendEndCmd();
				Disconnect();
			}

			if (_sendBuffer.Count == 0)
			{   // nothing to send
				return;
			}

			if (Helper.GetTicksMs() - _lastSentMs < Constants.WAIT_BEFORE_SEND_MSEC)
			{
				// wait xxx sec before sending
				return;
			}

			_sendTimerActive = true;

			if (CharsAckCount < Constants.WAIT_BEFORE_SEND_ACK && _sendBuffer.Count > 0)
			{
				// all characters processed at receiver side
				int cnt = _sendBuffer.Count;
				if (cnt > Constants.WAIT_BEFORE_SEND_ACK)
					cnt = Constants.WAIT_BEFORE_SEND_ACK;
				byte[] baudotData = new byte[cnt];
				for (int i = 0; i < cnt && _sendBuffer.Count > 0; i++)
				{
					baudotData[i] = _sendBuffer.Dequeue();
				}
				SendCmd(ItelexCommands.BaudotData, baudotData);
				Update?.Invoke();
			}

			_sendTimerActive = true;
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
			if (!IsConnected)
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
			SetLastSendRecv();

			string telexData = CodeConversion.AsciiStringToTelex(asciiStr);
			Send?.Invoke(telexData);

			if (ConnectionState == ConnectionStates.AsciiTexting)
			{
				byte[] data = Encoding.ASCII.GetBytes(asciiStr);
				_client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, EndSend, null);
			}
			else
			{
				byte[] baudotData = CodeConversion.AsciiStringToBaudot(asciiStr, ref _shiftState);
				//if (IsConnected)
				{
					for (int i = 0; i < baudotData.Length; i++)
					{
						byte chr = baudotData[i];
						SendBaudotCode(baudotData[i], ref _shiftState);
					}
				}
				Update?.Invoke();
			}
		}

		/*
		public void SendBaudotChar(byte baudotChr)
		{
			SendBaudotCode(baudotChr, ref _shiftState);
			Update?.Invoke();
		}
		*/

		public void SendDirectDialCmd(int code)
		{
			if (IsConnected)
			{
				Logging.Instance.Log(LogTypes.Info, TAG, nameof(SendDirectDialCmd), $"code={code}");
				byte[] data = new byte[] { (byte)code };
				SendCmd(ItelexCommands.DirectDial, data);
			}
		}

		public void SendVersionCodeCmd()
		{
			if (IsConnected)
			{
				byte[] data = new byte[] { 0x01 }; // version = 1
				SendCmd(ItelexCommands.ProtocolVersion, data);
			}
		}

		public void SendEndCmd()
		{
			if (IsConnected)
			{
				SendCmd(ItelexCommands.End);
			}
		}

		private void SendBaudotCode(byte baudotCode, ref CodeConversion.ShiftStates shiftState)
		{
			byte[] codes = CodeConversion.BaudotCodeToBaudotWithShift(baudotCode, shiftState, ref shiftState);
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

			for (int i = 0; i < codes.Length; i++)
			{
				if (IsConnected)
				{
					_sendBuffer.Enqueue(codes[i]);
				}
			}
			Update?.Invoke();
			//Debug.WriteLine($"enqueue {baudotCode:X2}");
			_lastSentMs = Helper.GetTicksMs();
		}

		public void SendCmd(ItelexCommands cmd, byte[] data = null)
		{
			if (!IsConnected)
				return;

			SendCmd((int)cmd, data);
		}

		public void SendCmd(int cmdCode, byte[] data = null)
		{
			if (!IsConnected)
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

			ItelexPacket sendPacket = new ItelexPacket(sendData);
			switch ((ItelexCommands)sendPacket.Command)
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
			if (!IsConnected)
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
					ConnectionState = ConnectionStates.BaudotTexting;
				}
				else
				{
					ConnectionState = ConnectionStates.AsciiTexting;
				}
			}

			if (ConnectionState == ConnectionStates.AsciiTexting)
			{	// ascii
				string asciiText = Encoding.ASCII.GetString(newData, 0, newData.Length);
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
						Debug.WriteLine(packet);
						DecodePacket(packet);
					}
				}
			}

			_incomingData = new byte[0];
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
			Debug.WriteLine($"incoming {packet}");

			switch ((ItelexCommands)packet.Command)
			{
				case ItelexCommands.Heartbeart:
					break;
				case ItelexCommands.DirectDial:
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"direct dial command, number={packet.Data[0]}");
					break;
				case ItelexCommands.BaudotData:
					if (packet.Len > 0)
					{
						string asciiStr = CodeConversion.BaudotStringToAscii(packet.Data, ref _shiftState);
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
					Message?.Invoke($"REJECT {reason.ToUpper()} ({ReasonToString(reason).ToUpper()})");
					Disconnect();
					break;
				case ItelexCommands.Ack:
					_n_ack = packet.Data[0];
					Update?.Invoke();
					break;
				case ItelexCommands.ProtocolVersion:
					string versionStr = "";
					if (packet.Data.Length > 1)
					{
						// get version string
						versionStr = Encoding.ASCII.GetString(packet.Data, 1, packet.Data.Length - 1);
					}
					Logging.Instance.Log(LogTypes.Info, TAG, nameof(DecodePacket), $"protocol version command, version={packet.Data[0]} {versionStr}");
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


		/*
		private int TransAckDiff()
		{
			int trans = _n_trans;
			if (_n_ack > trans)
				trans += 256;
			return _n_trans - _n_ack;
		}
		*/

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

		public void Dispose()
		{
			Disconnect();
		}

	}
}
