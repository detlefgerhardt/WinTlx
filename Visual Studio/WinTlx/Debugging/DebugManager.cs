using System;
using System.Drawing;
using System.IO;
using System.Text;
using WinTlx.Codes;

namespace WinTlx.Debugging
{
	class DebugManager
	{
		private const string TAG = nameof(DebugManager);

		public enum Modes { Recv, Send, Message };

		private DebugForm _debugForm;

		private KeyStates _keyStates;

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static DebugManager instance;

		public static DebugManager Instance => instance ?? (instance = new DebugManager());

		private DebugManager()
		{
		}

		public void OpenDebugForm(Rectangle? position)
		{
			if (_debugForm == null)
			{
				_debugForm = new DebugForm(position);
				_debugForm.CloseEvent += DebugForm_CloseEvent;
				_debugForm.Show();
			}
			else
			{
				_debugForm.BringToFront();
			}
		}

		private void DebugForm_CloseEvent()
		{
			_debugForm.CloseEvent += DebugForm_CloseEvent;
			_debugForm = null;
		}

		public void InitConnection()
		{
			_keyStates = new KeyStates();
		}

		public void Clear()
		{
			_debugForm.ClearDebugText();
		}

		public void WriteCmd(ItelexPacket itxPkt, Modes mode)
		{
			WriteCmd(itxPkt, mode, null);
		}

		public void WriteCmd(ItelexPacket itxPkt, Modes mode, Acknowledge ack)
		{
			if (_debugForm == null) return;

			if (itxPkt.CommandType == ItelexCommands.Heartbeat && !_debugForm.ShowHeartbeat) return;

			if (itxPkt.CommandType == ItelexCommands.Ack && !_debugForm.ShowAck) return;

			string debugStr = CmdToString(itxPkt, mode, ack);
			Write(debugStr + "\r\n", mode);
		}

		private string CmdToString(ItelexPacket itxPkt, Modes mode, Acknowledge ack)
		{
			string cmdStr;
			switch (itxPkt.CommandType)
			{
				case ItelexCommands.Heartbeat:
					return $"Heartbeat {itxPkt.GetDebugData()}";
				case ItelexCommands.DirectDial:
					cmdStr = "DirectDial";
					if (itxPkt.Data.Length==0)
					{
						return $"{cmdStr} no data";
					}
					else
					{
						return $"{cmdStr} {itxPkt.Data[0]} [{itxPkt.GetDebugPacket()}]";
					}
				case ItelexCommands.BaudotData:
					cmdStr = "BaudotData";
					if (itxPkt.Data.Length == 0)
					{
						return $"{cmdStr} no data";
					}
					else
					{
						string asciiStr = CodeManager.BaudotStringToAscii(itxPkt.Data, _keyStates, CodeManager.SendRecv.Recv, true);
						asciiStr = ConvDebugText(asciiStr);
						return $"{cmdStr} {asciiStr} [{itxPkt.GetDebugPacket()}] {ack.LocalBufferCount}";
					}
				case ItelexCommands.End:
					return $"End {itxPkt.GetDebugData()}";
				case ItelexCommands.Reject:
					cmdStr = "Reject";
					if (itxPkt.Data.Length == 0)
					{
						return $"{cmdStr} no data";
					}
					else
					{
						string rejectReason = Encoding.ASCII.GetString(itxPkt.Data, 0, itxPkt.Data.Length);
						rejectReason = rejectReason.TrimEnd('\x00');
						return $"{cmdStr} '{rejectReason}' [{itxPkt.GetDebugPacket()}]";
					}
				case ItelexCommands.Ack:
					cmdStr = $"Ack";
					if (itxPkt.Data.Length == 0)
					{
						return $"{cmdStr} no data";
					}
					else
					{
						if (ack == null)
						{
							return $"{cmdStr} [{itxPkt.GetDebugPacket()}]";
						}
						else
						{
							int ackCnt = itxPkt.Data[0];
							if (mode == Modes.Recv)
							{
								return $"{cmdStr} ack={ackCnt} sent={ack.SendCnt}/{ack.SendAckCnt} buf={ack.RemoteBufferCount} [{itxPkt.GetDebugPacket()}]";
							}
							else
							{
								return $"{cmdStr} ack={ackCnt} recv={ack.ReceivedCnt}/{ack.ReceivedAckCnt} buf={ack.LocalBufferCount} [{itxPkt.GetDebugPacket()}]";
							}
						}
					}
				case ItelexCommands.ProtocolVersion:
					cmdStr = "ProtocolVersion";
					if (itxPkt.Data.Length == 0)
					{
						return $"{cmdStr} no data";
					}
					else
					{
						string versionStr = Encoding.ASCII.GetString(itxPkt.Data, 1, itxPkt.Data.Length - 1);
						versionStr = versionStr.TrimEnd('\x00'); // remove 00-byte suffix
						versionStr = ConvDebugText(versionStr);
						return $"{cmdStr} {itxPkt.Data[0]} '{versionStr}' [{itxPkt.GetDebugPacket()}]";
					}
				case ItelexCommands.SelfTest:
					return $"SelfTest {itxPkt.GetDebugData()}";
				case ItelexCommands.RemoteConfig:
					return $"RemoteConfig {itxPkt.GetDebugData()}";
				case ItelexCommands.ConnectRemote:
					return $"ConnectRemote {itxPkt.GetDebugData()}";
				case ItelexCommands.RemoteConfirm:
					return $"RemoteConfirm {itxPkt.GetDebugData()}";
				case ItelexCommands.RemoteCall:
					return $"RemoteCall {itxPkt.GetDebugData()}";
				case ItelexCommands.AcceptCallRemote:
					return $"AcceptCallRemote {itxPkt.GetDebugData()}";
				default:
					return $"Unknown cmd {itxPkt.GetDebugData()}";
			}
		}

		private int remoteBufferCount(int ack, int trans)
		{
			return ((trans + 256) - ack) % 256;
		}

		private int localBufferCount(int ack, int print)
		{
			return ((print + 256) - ack) % 256;
		}

		public void WriteLine(string text, Modes mode)
		{
			Write(text + "\r\n", mode);
		}

		public void Write(string text, Modes mode)
		{
			if (_debugForm != null)
			{
				while(true)
				{
					if (text.Length >=4 && text.Substring(text.Length-4, 4) == "\r\n\r\n")
					{
						text = text.Substring(0, text.Length - 2);
					}
					else
					{
						break;
					}
				}
				Color textColor = GetModeColor(mode);
				_debugForm.ShowDebugText(text, textColor);
			}
		}

		private string ConvDebugText(string str)
		{
			string convStr = "";
			for (int i = 0; i < str.Length; i++)
			{
				string convChr;
				switch (str[i])
				{
					case '\r':
						convChr = "<CR>";
						break;
					case '\n':
						convChr = "<LF>";
						break;
					case CodeManager.ASC_BEL:
						convChr = "<BEL>";
						break;
					case CodeManager.ASC_WRU:
						convChr = "<WRU>";
						break;
					case CodeManager.ASC_LTRS:
						convChr = "<BU>";
						break;
					case CodeManager.ASC_FIGS:
						convChr = "<ZI>";
						break;
					case CodeManager.ASC_CODE32:
						convChr = "<COD32>";
						break;
					case CodeManager.ASC_SHIFTF:
						convChr = "<F>";
						break;
					case CodeManager.ASC_SHIFTG:
						convChr = "<G>";
						break;
					case CodeManager.ASC_SHIFTH:
						convChr = "<H>";
						break;
					default:
						if (str[i] < 0x20 || str[i] > 0x7F)
						{
							convChr = $"<{str[i]:X02}>";
						}
						else
						{
							convChr = ((char)str[i]).ToString();
						}
						break;
				}
				convStr += convChr;
			}
			return convStr;
		}

		private Color GetModeColor(Modes mode)
		{
			switch (mode)
			{
				case Modes.Send:
				default:
					return Color.Red;
				case Modes.Recv:
					return Color.Black;
				case Modes.Message:
					return Color.Blue;
			}
		}

	}
}
