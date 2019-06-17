using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace WinTlx
{
	public partial class MainForm : Form
	{
		private const string TAG = nameof(MainForm);

		private System.Timers.Timer _clockTimer;

		private int _fixedWidth;

		private ConfigData _configData;

		private SubscriberServer _subscriberServer;
		private ItelexProtocol _itelex;
		TapePunchHorizontalForm _tapePunchForm;
		EyeballChar _eyeballChar;

		public const int SCREEN_WIDTH = 68;
		public const int CHAR_HEIGHT = 19;
		public const int CHAR_WIDTH = 9;

		private int _screenHeight = 25;
		private List<ScreenLine> _screen = new List<ScreenLine>();
		private int _screenX = 0;
		private int _screenY = 0;
		private int _screenEditPos0 = 0;
		private int _screenShowPos0 = 0;
		//private bool _screenDirty = false;

		private System.Timers.Timer _outputTimer;
		private Queue<ScreenChar> _outputBuffer;

		//private bool _refreshActive;
		//private System.Timers.Timer _refreshTimer;

		public MainForm()
		{
			InitializeComponent();
			_fixedWidth = this.Width;
			TerminalPb.ContextMenuStrip = CreateContextMenu();

			Logging.Instance.Log(LogTypes.Info, TAG, "Start", $"{Helper.GetVersion()}");

			this.Text = Helper.GetVersion();

			KeyPreview = true;

			MemberCb.DataSource = null;
			MemberCb.DisplayMember = "DisplayName";

			//ConnectBtn.Enabled = false;

			SendLineFeedBtn.Text = "\u2261";
			//TimeBtn.Text = "\u2299";

			InactivityTimoutTb.Text = "";
			ConnTimeTb.Text = "";
			LnColTb.Text = "";
			SendAckTb.Text = "";

			RecvOnCb.Enabled = true;

			this.KeyPreview = true;
			this.KeyDown += Form_KeyDown;
			this.KeyPress += Form_KeyPress;

			_configData = ConfigManager.Instance.LoadConfig();
			if (_configData == null)
			{
				_configData = ConfigManager.Instance.GetDefaultConfig();
			}

			_itelex = new ItelexProtocol();
			_itelex.Received += ReceivedHandler;
			_itelex.Send += SendHandler;
			_itelex.BaudotSendRecv += BaudotSendRecvHandler;
			_itelex.Connected += ConnectedHandler;
			_itelex.Dropped += DroppedHandler;
			_itelex.Update += UpdatedHandler;
			_itelex.Message += MessageHandler;
			_itelex.InactivityTimeout = _configData.InactivityTimeout;

			_subscriberServer = new SubscriberServer();
			_subscriberServer.Message += SubcribeServerMessageHandler;

			_eyeballChar = EyeballChar.Instance;

			_clockTimer = new System.Timers.Timer(500);
			_clockTimer.Elapsed += ClockTimer_Elapsed;
			_clockTimer.Start();

			_outputBuffer = new Queue<ScreenChar>();
			_outputTimer = new System.Timers.Timer();
			_outputTimer.Elapsed += _outputTimer_Elapsed;
			SetOutputTimer(_configData.OutputSpeed);

			ClearScreen();

			SetConnectState();
			UpdatedHandler();

			SetFocus();
			SearchTb.Focus();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			MainForm_Resize(null, null);
			SetFocus();
			SearchTb.Focus();

#if !DEBUG
			string text =
				$"{Helper.GetVersion()}\r\r" +
				"by *dg* Detlef Gerhardt\r\r" +
				"Please note that this is a test and diagnostic tool for the i-Telex network. " +
				"The participants have real teletype machines connected to there i-Telex ports. " +
				"Please do not send longer text files or spam to i-Telex numbers!";
			MessageBox.Show(
				text,
				$"{Constants.PROGRAM_NAME}",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button1);

#endif
			//Expired();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Disconnect();
		}

		private void ExitBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form_KeyPress(object sender, KeyPressEventArgs e)
		{
		}

		private void Form_KeyDown(object sender, KeyEventArgs e)
		{
			if (KennungTb.Focused || SearchTb.Focused || MemberCb.Focused || AddressTb.Focused || PortTb.Focused ||
				ExtensionTb.Focused)
			{
				return;
			}

			if (e.Control)
			{
				switch (e.KeyCode)
				{
					case Keys.C: // copy
						CopyAction(null, null);
						e.Handled = true;
						e.SuppressKeyPress = true;
						return;
					case Keys.V: // paste
						PasteAction(null, null);
						e.Handled = true;
						e.SuppressKeyPress = true;
						return;
				}
			}

			int oldShowPos0 = _screenShowPos0;
			switch (e.KeyCode)
			{
				case Keys.Up:
					_screenShowPos0--;
					break;
				case Keys.Down:
					_screenShowPos0++;
					break;
				case Keys.PageUp:
					_screenShowPos0 -= _screenHeight - 1;
					break;
				case Keys.PageDown:
					_screenShowPos0 += _screenHeight - 1;
					break;
				case Keys.Home:
					_screenShowPos0 = 0;
					break;
				case Keys.End:
					_screenShowPos0 = _screen.Count - _screenHeight;
					break;
			}

			if (_screenShowPos0 != oldShowPos0)
			{
				if (_screenShowPos0 > _screen.Count - _screenHeight)
				{
					_screenShowPos0 = _screen.Count - _screenHeight;
				}
				if (_screenShowPos0 < 0)
				{
					_screenShowPos0 = 0;
				}
				ShowScreen();
				//_screenDirty = true;
				e.Handled = true;
				e.SuppressKeyPress = true;
				return;
			}

			string chrs = KeyDownConverter.ToStr(e.KeyValue, e.Modifiers);
			SendAsciiText(chrs);
			e.Handled = true;
			e.SuppressKeyPress = true;
		}

		private void ClockTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			DateTime dt = DateTime.Now;
			Helper.ControlInvokeRequired(DateTb, () => DateTb.Text = $"{dt:dd.MM.yyyy}");
			Helper.ControlInvokeRequired(TimeTb, () => TimeTb.Text = $"{dt:HH:mm:ss}");
		}

		private void PhoneEntryCb_SelectedIndexChanged(object sender, EventArgs e)
		{
			PeerQueryData entry = (PeerQueryData)MemberCb.SelectedItem;
			AddressTb.Text = entry.Address;
			PortTb.Text = entry.PortNumber != 0 ? entry.PortNumber.ToString() : "";
			ExtensionTb.Text = entry.ExtensionNumber != 0 ? entry.ExtensionNumber.ToString() : "";
		}

		private async void ConnectBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			if (_itelex.IsConnected || string.IsNullOrEmpty(PortTb.Text))
			{
				return;
			}

			ConnectBtn.Enabled = false;
			await ConnectOut();
			ConnectBtn.Enabled = true;
			SetConnectState();
		}

		private void DisconnectBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			_outputBuffer.Clear();
			Disconnect();
		}

		private void LocalBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			LocalBtn_DoClick();
		}

		private void LocalBtn_DoClick()
		{
			SetFocus();
			if (!_itelex.IsConnected)
			{
				return;
			}

			if (_itelex.Local)
			{
				_itelex.Local = false;
			}
			else
			{
				_itelex.Local = true;
			}
			SetConnectState();
		}


		private void KennungTb_Leave(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(KennungTb.Text))
			{
				KennungTb.Text = Constants.DEFAULT_KENNUNG;
			}
		}

		private void AddressTb_Leave(object sender, EventArgs e)
		{
			SetConnectState();
		}

		private void PortTb_Leave(object sender, EventArgs e)
		{
			SetConnectState();
		}

		private void SendWruBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendWerDa();
		}

		private void SendHereIsBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendHereIs();
		}

		private void SendBellBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText("\x7");
		}

		private void SendLettersBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			_itelex.SendAsciiChar(CodeConversion.ASC_LTRS);
		}

		private void SendFiguresBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			_itelex.SendAsciiChar(CodeConversion.ASC_FIGS);
		}

		private void SendCarriageReturnBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			_itelex.SendAsciiText("\r");
		}

		private void SendLineFeedBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			_itelex.SendAsciiText("\n");
		}

		private void SendTimeBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText($"{DateTime.Now:dd.MM.yyyy  HH:mm}\r\n");
		}

		private void Code32Btn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText("\x00");
		}

		private void SendLineBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText("\r\n");
			SendAsciiText(new string('-', 68));
			SendAsciiText("\r\n");
		}

		private void SendRyBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			for (int l = 0; l < 10; l++)
			{
				for (int i = 0; i < 33; i++)
				{
					SendAsciiText("ry");
				}
				SendAsciiText("\r\n");
			}
		}

		private void SendFoxBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText("the quick brown fox jumps over the lazy dog 1234567890/:,-=()");
			//SendAsciiText("\r\n");
		}

		private void TerminalPb_MouseClick(object sender, MouseEventArgs e)
		{
			SetFocus();
		}

		private void ReceivedHandler(string asciiText)
		{
			string str = "";
			for (int i = 0; i < asciiText.Length; i++)
			{
				switch (asciiText[i])
				{
					case CodeConversion.ASC_BEL:
						SystemSounds.Beep.Play();
						AddText("\u04E8", CharAttributes.Recv);
						return;
					case CodeConversion.ASC_WRU:
						SendHereIs();
						AddText("\u2629", CharAttributes.Recv);
						return;
				}
			}
			AddText(asciiText, CharAttributes.Recv);
		}

		private void SendHandler(string asciiText)
		{
			string dispText = "";
			for (int i = 0; i < asciiText.Length; i++)
			{
				char c = asciiText[i];
				switch (c)
				{
					case CodeConversion.ASC_CODE32:
						continue;
					case CodeConversion.ASC_BEL:
						SystemSounds.Beep.Play();
						c = '\u04E8';
						break;
					case CodeConversion.ASC_WRU:
						c = '\u2629';
						break;
					case CodeConversion.ASC_LTRS:
					case CodeConversion.ASC_FIGS:
						continue;
				}
				dispText += c;
			}
			AddText(dispText, CharAttributes.Send);
		}

		private void BaudotSendRecvHandler(byte[] code)
		{
			if (_tapePunchForm == null)
			{
				return;
			}

			for (int i = 0; i < code.Length; i++)
			{
				_tapePunchForm.PunchCode(code[i], _itelex.ShiftState);
			}
		}

		private void SubcribeServerMessageHandler(string message)
		{
			ShowLocalMessage(message);
		}

		private void ConnectedHandler()
		{
			ShowLocalMessage("CONNECTED");
			SetConnectState();
		}

		private void DroppedHandler()
		{
			ShowLocalMessage("DISCONNECTED");
			SetConnectState();
		}

		private void UpdatedHandler()
		{
			Helper.ControlInvokeRequired(SendAckTb, () => SendAckTb.Text = $"Snd {_itelex.CharsToSendCount}  Ack {_itelex.CharsAckCount}");
			Helper.ControlInvokeRequired(InactivityTimoutTb, () => InactivityTimoutTb.Text = $"Timeout {_itelex.InactivityTimer} sec");
			Helper.ControlInvokeRequired(ConnTimeTb, () => ConnTimeTb.Text = $"Conn {_itelex.ConnTimeMin} min");

			Helper.ControlInvokeRequired(KennungTb, () => KennungTb.Text = _configData.Kennung);

			Helper.ControlInvokeRequired(SendLettersBtn, () =>
			{
				if (_itelex.ShiftState == CodeConversion.ShiftStates.Unknown || _itelex.ShiftState == CodeConversion.ShiftStates.Figs)
				{
					SendLettersBtn.ForeColor = Color.Black;
				}
				else
				{
					SendLettersBtn.ForeColor = Color.Green;
				}
			});

			Helper.ControlInvokeRequired(SendFiguresBtn, () =>
			{
				if (_itelex.ShiftState == CodeConversion.ShiftStates.Unknown || _itelex.ShiftState == CodeConversion.ShiftStates.Ltr)
				{
					SendFiguresBtn.ForeColor = Color.Black;
				}
				else
				{
					SendFiguresBtn.ForeColor = Color.Green;
				}
			});

			bool itelex;
			bool ascii;
			if (_itelex.IsConnected)
			{
				itelex = _itelex.ConnectionState == ItelexProtocol.ConnectionStates.BaudotTexting;
				ascii = _itelex.ConnectionState == ItelexProtocol.ConnectionStates.AsciiTexting;
			}
			else
			{
				itelex = false;
				ascii = false;
			}

			Helper.ControlInvokeRequired(ProtocolItelexRb, () =>
				ProtocolItelexRb.Checked = itelex
			);
			Helper.ControlInvokeRequired(ProtocolAsciiRb, () =>
				ProtocolAsciiRb.Checked = ascii
			);

			Helper.ControlInvokeRequired(RecvOnCb, () =>
			{
				RecvOnCb.Checked = _itelex.RecvOn;

				/*
				if (_itelex.RecvOn)
				{
					RecvOnOffBtn.ForeColor = Color.Green;
					RecvOnOffBtn.Text = "Recv Off";
					//UpdateIpAddressBtn.Enabled = true;
				}
				else
				{
					RecvOnOffBtn.ForeColor = Color.Black;
					RecvOnOffBtn.Text = "Recv On";
					//UpdateIpAddressBtn.Enabled = false;
				}
				*/
			});
		}

		private async Task<bool> ConnectOut()
		{
			AddressTb.Text = AddressTb.Text.Trim();
			if (string.IsNullOrWhiteSpace(AddressTb.Text))
			{
				ShowLocalMessage("NO ADDRESS");
				return false;
			}

			PortTb.Text = PortTb.Text.Trim();
			int? port = Helper.ToInt(PortTb.Text);
			if (port == null)
			{
				ShowLocalMessage("INVALID PORT");
				return false;
			}

			ExtensionTb.Text = ExtensionTb.Text.Trim();
			int? extension = null;
			if (!string.IsNullOrWhiteSpace(ExtensionTb.Text))
			{
				extension = Helper.ToInt(ExtensionTb.Text);
				if (extension == null)
				{
					ShowLocalMessage("INVALID EXTENSION NUMBER");
					return false;
				}
			}

			bool success = await _itelex.ConnectOut(AddressTb.Text, port.Value, false);
			if (!success)
			{
				ShowLocalMessage("CONNECTION ERROR");
				return false;
			}

			_itelex.SendVersionCodeCmd();
			if (extension != null)
			{
				_itelex.SendDirectDialCmd(extension.Value);
			}

			//_itelex.StartAck();

			return true;
		}

		private void Disconnect()
		{
			if (_itelex != null)
			{
				_itelex.SendEndCmd();
				_itelex.Disconnect();
				SetConnectState();
			}
		}

		private void SendWerDa()
		{
			SendAsciiText(CodeConversion.ASC_WRU.ToString());
		}

		private void SendHereIs()
		{
#if DEBUG
			SendAsciiText($"\r\n{KennungTb.Text}");
#else
			SendAsciiText($"\r\n{KennungTb.Text} (wintlx)");
#endif
		}

		private void SetConnectState()
		{
			if (!_itelex.IsConnected)
			{
				LocalBtn.ForeColor = Color.Green;
				ConnectBtn.ForeColor = Color.Black;
				DisconnectBtn.ForeColor = Color.Gray;
				if (string.IsNullOrEmpty(AddressTb.Text) || string.IsNullOrEmpty(PortTb.Text))
				{
					ConnectBtn.ForeColor = Color.Gray;
				}
				else
				{
					ConnectBtn.ForeColor = Color.Black;
				}
			}
			else
			{
				if (_itelex.Local)
				{
					LocalBtn.ForeColor = Color.Green;
					ConnectBtn.ForeColor = Color.Green;
					DisconnectBtn.ForeColor = Color.Black;
				}
				else
				{
					LocalBtn.ForeColor = Color.Black;
					ConnectBtn.ForeColor = Color.Green;
					DisconnectBtn.ForeColor = Color.Black;
				}
			}
		}

		private void ClearBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			ClearScreen();
		}

		private void SendFileBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendFileForm sendFileForm = new SendFileForm();
			sendFileForm.ShowDialog();

			if (sendFileForm.AsciiText != null)
			{
				SendAsciiText(sendFileForm.AsciiText);
			}
			return;
		}

		private async void QueryBtn_Click(object sender, EventArgs e)
		{
			SetFocus();

			SearchTb.Text = SearchTb.Text.Trim();

			Logging.Instance.Log(LogTypes.Info, TAG, nameof(QueryBtn_Click), $"SearchText='{SearchTb.Text}'");

			if (string.IsNullOrWhiteSpace(_configData.SubscribeServerAddress) || _configData.SubscribeServerPort == 0)
			{
				SubcribeServerMessageHandler("invalid subscribe server address or port");
				return;
			}

			PeerQueryData[] list = null;

			uint num;
			if (!uint.TryParse(SearchTb.Text, out num))
				num = 0;

			await Task.Run(() =>
			{
				if (!_subscriberServer.Connect(_configData.SubscribeServerAddress, _configData.SubscribeServerPort))
				{
					return;
				}
				if (num > 0)
				{
					// query number
					PeerQueryReply queryReply = _subscriberServer.SendPeerQuery(num.ToString());
					_subscriberServer.Disconnect();
					if (!queryReply.Valid)
					{
						SubcribeServerMessageHandler(queryReply.Error);
						return;
					}
					if (queryReply.Data != null)
					{
						list = new PeerQueryData[] { queryReply.Data };
					}
					else
					{
						list = new PeerQueryData[0];
					}
				}
				else
				{
					// search member
					PeerSearchReply searchReply = _subscriberServer.SendPeerSearch(SearchTb.Text);
					_subscriberServer.Disconnect();
					if (!searchReply.Valid)
					{
						SubcribeServerMessageHandler(searchReply.Error);
						return;
					}
					list = searchReply.List;
				}
			});

			SubcribeServerMessageHandler($"{list?.Length} member(s) found");

			MemberCb.DataSource = list;
			MemberCb.DisplayMember = "Display";
			if (list == null)
			{
			}
			else if (list.Length == 0)
			{
				MemberCb.Text = "";
				AddressTb.Text = "";
				PortTb.Text = "";
				ExtensionTb.Text = "";
			}
			else
			{
				MemberCb.SelectedIndex = 0;
			}

			SetConnectState();
		}

		private void ClearScreen()
		{
			_outputBuffer.Clear();

			_screen.Clear();
			_screenX = 0;
			_screenY = 0;
			_screenEditPos0 = 0;
			_screenShowPos0 = 0;
			ShowScreen();
		}

		private void AddText(string asciiText, CharAttributes attr, bool fast = false)
		{
			if (fast || _configData.OutputSpeed==0)
			{
				// output immediatly
				OutputText(asciiText, attr);
			}
			else
			{
				// put into output queue
				foreach(char chr in asciiText)
				{
					_outputBuffer.Enqueue(new ScreenChar(chr, attr));
				}
			}
		}

		private void SetOutputTimer(int charSec)
		{
			_outputTimer.Stop();
			if (charSec != 0)
			{
				_outputTimer.Interval = 1000 / charSec * 5 + 1;
				_outputTimer.Start();
			}
		}

		private void ClearOutputBuffer()
		{
			_outputBuffer.Clear();
		}

		private void _outputTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_outputBuffer.Count > 0)
			{
				ScreenChar chr = _outputBuffer.Dequeue();
				OutputText(chr.Char.ToString(), chr.Attr);
			}
		}

		private void ShowLocalMessage(string message)
		{
			if (_screenX > 0)
			{
				AddText("\r\n", CharAttributes.Message, true);
			}
			AddText($"{message.ToUpper()}\r\n", CharAttributes.Message, true);
		}

		private void OutputText(string asciiText, CharAttributes attr)
		{
			if (string.IsNullOrEmpty(asciiText))
			{
				return;
			}

			for (int i = 0; i < asciiText.Length; i++)
			{
				switch (asciiText[i])
				{
					case '\n':
						IncScreenY();
						break;
					case '\r':
						_screenX = 0;
						break;
					case '\x0': // code32
						break;
					default:
						if (_screenY + _screenEditPos0 >= _screen.Count)
						{
							_screen.Add(new ScreenLine());
						}
						_screen[_screenEditPos0 + _screenY].Line[_screenX] = new ScreenChar(asciiText[i], attr);
						IncScreenX();
						break;
				}
			}

			_screenShowPos0 = _screenEditPos0;

			ShowScreen();
			//_screenDirty = true;
			CommLog(asciiText);
		}

		private void IncScreenX()
		{
			if (_screenX < SCREEN_WIDTH)
			{
				_screenX++;
				if (_screenX == 60)
					SystemSounds.Beep.Play();
			}
			else
			{
				SystemSounds.Exclamation.Play();
			}
		}

		private void IncScreenY()
		{
			_screen.Add(new ScreenLine());
			_screenY++;
			if (_screenY >= _screenHeight)
			{
				_screenEditPos0++;
				_screenShowPos0 = _screenEditPos0;
				_screenY--;
				ShowScreen();
				//_screenDirty = true;
			}
		}

		private void ShowScreen()
		{
			Helper.ControlInvokeRequired(TerminalPb, () =>
			{
				TerminalPb.Refresh();
			});

			Helper.ControlInvokeRequired(LnColTb, () =>
			{
				LnColTb.Text = $"Ln {_screenEditPos0 + _screenY + 1}  Col {_screenX + 1}";
			});
		}

		private void SendBufferTb_TextChanged(object sender, EventArgs e)
		{
		}

		private void AckLbl_Click(object sender, EventArgs e)
		{
		}

		private void SendAsciiText(string text)
		{
			_itelex?.SendAsciiText(text);
		}

		private void SetFocus()
		{
			TerminalPb.Focus();
		}

		private object _commLogLock = new object();

		private void CommLog(string text)
		{
			lock (_commLogLock)
			{
				try
				{
					File.AppendAllText($"{Constants.PROGRAM_NAME}.log", text);
				}
				catch (Exception ex)
				{
					Logging.Instance.Error(TAG, nameof(CommLog), "", ex);
				}
			}
		}

		private void AboutBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			string text =
				$"{Helper.GetVersion()}\r\r" +
				"by *dg* Detlef Gerhardt\r\r" +
				"Send feedback to\r" +
				"feedback@dgerhardt.de";
			MessageBox.Show(
				text,
				$"About {Constants.PROGRAM_NAME}",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button1);
		}

		private bool Expired()
		{
			if (!Helper.IsExpired())
			{
				return false;
			}

			string text =
				$"{Helper.GetVersion()}\r\r" +
				"This test version expired\r\r" +
				"Please get a new version at\rhttps://github.com/detlefgerhardt/WinTlx/tree/master/Binaries";
			MessageBox.Show(
				text,
				$"Expired",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button1);

			return true;
		}

		private ContextMenuStrip CreateContextMenu()
		{
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			bool needSep = false;
			List<ToolStripMenuItem> endItems = new List<ToolStripMenuItem>();
			endItems.Add(new ToolStripMenuItem("Clear"));
			endItems.Add(new ToolStripMenuItem("Copy"));
			endItems.Add(new ToolStripMenuItem("Paste"));
			contextMenu.ItemClicked += ConectMenu_ItemClicked;
			return contextMenu;
		}

		private void ConectMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			// hide context menu
			ContextMenuStrip cms = (ContextMenuStrip)sender;
			cms.Hide();

			// get menu item
			ToolStripItem tsi = e.ClickedItem;
		}

		private void TerminalPb_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{   //click event
				ContextMenu contextMenu = new ContextMenu();
				MenuItem menuItem = new MenuItem("Clear");
				menuItem.Click += new EventHandler(ClearAction);
				contextMenu.MenuItems.Add(menuItem);
				menuItem = new MenuItem("Copy");
				menuItem.Click += new EventHandler(CopyAction);
				contextMenu.MenuItems.Add(menuItem);
				menuItem = new MenuItem("Paste");
				menuItem.Click += new EventHandler(PasteAction);
				contextMenu.MenuItems.Add(menuItem);
				TerminalPb.ContextMenu = contextMenu;
			}
		}

		private void ClearAction(object sender, EventArgs e)
		{
			ClearScreen();
		}

		private void CopyAction(object sender, EventArgs e)
		{
			string str = "";
			foreach(ScreenLine line in _screen)
			{
				str += line.LineStr + "\r\n";
			}
			Clipboard.SetData(DataFormats.Text, str);
		}

		private void PasteAction(object sender, EventArgs e)
		{
			if (Clipboard.ContainsText(TextDataFormat.Text))
			{
				SendAsciiText(Clipboard.GetText());
			}
		}

		private void MessageHandler(string asciiText)
		{
			ShowLocalMessage(asciiText);
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			Debug.WriteLine(this.Height);

			// prevent width change
			this.Width = _fixedWidth;

			TerminalPb.Height = this.Height - 310 + 50;
			_screenHeight = TerminalPb.Height / CHAR_HEIGHT;


			_screenEditPos0 = _screen.Count - _screenHeight;
			if (_screenEditPos0 < 0)
				_screenEditPos0 = 0;
			_screenShowPos0 = _screenEditPos0;

			_screenY = _screen.Count - _screenEditPos0 - 1;

#warning TODO why is _screenY < 0 sometimes?
			if (_screenY < 0)
				_screenY = 0;

			ShowScreen();
			//_screenDirty = true;

			_tapePunchForm?.SetPosition(this.Bounds);
		}

		private void MainForm_LocationChanged(object sender, EventArgs e)
		{
			_tapePunchForm?.SetPosition(this.Bounds);
		}

		private void MainForm_Activated(object sender, EventArgs e)
		{
			//_tapePunchForm?.Activate();
		}

		private void SendClientUpdate(int number, int pin, int publicPort)
		{
			ClientUpdateReply reply = _subscriberServer.SendClientUpdate(number, pin, publicPort);
			if (reply.Success)
			{
				SubcribeServerMessageHandler($"update {number} {reply.IpAddress}:{publicPort}");
			}
			else
			{
				SubcribeServerMessageHandler($"update {number} {reply.Error}");
			}
		}

		private void RecvOnCb_Click(object sender, EventArgs e)
		{
			if (!_itelex.RecvOn)
			{
				_itelex.SetRecvOn(_configData.IncomingLocalPort);
				UpdateIpAddress();
			}
			else
			{
				_itelex.SetRecvOff();
			}
		}

		private void UpdateIpAddressBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			UpdateIpAddress();
		}

		private void UpdateIpAddress()
		{
			if (_configData.SubscribeServerUpdatePin == 0 || _configData.OwnNumber == 0 || _configData.IncomingLocalPort == 0)
			{
				return;
			}

			_subscriberServer.Connect(_configData.SubscribeServerAddress, _configData.SubscribeServerPort);
			SendClientUpdate(_configData.OwnNumber, _configData.SubscribeServerUpdatePin, _configData.IncomingPublicPort);
			_subscriberServer.Disconnect();
		}

		private void TapePunchBtn_Click(object sender, EventArgs e)
		{
			if (_tapePunchForm == null)
			{
				_tapePunchForm = new TapePunchHorizontalForm(this.Bounds);
				_tapePunchForm.Show();
			}
			else
			{
				_tapePunchForm.Close();
				_tapePunchForm = null;
			}
			SetFocus();
		}

		private void ConfigBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			ConfigForm configForm = new ConfigForm(this.Bounds);
			configForm.SetData(_configData);
			configForm.ShowDialog();
			if (!configForm.Canceled)
			{
				_configData = configForm.GetData();
				ConfigManager.Instance.SaveConfig(_configData);
				_itelex.InactivityTimeout = _configData.InactivityTimeout;
				SetOutputTimer(_configData.OutputSpeed);
				UpdatedHandler();
			}
		}

		private void EyeballCharCb_CheckedChanged(object sender, EventArgs e)
		{
			if (EyeballCharCb.Checked)
			{
				_itelex.SendAsciiText("\r\neyeball char mode active - start tape punch\r\n");
			}
			_itelex.EyeballCharActive = EyeballCharCb.Checked;
		}

		private void LinealPnl_Paint(object sender, PaintEventArgs e)
		{
			Helper.PaintRuler(e.Graphics, SCREEN_WIDTH, 8.98F);
		}

		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			/*
			if (_screenDirty)
			{
				Debug.WriteLine(_screenDirty);
				_screenDirty = false;
				ShowScreen();
			}
			*/
		}

		private void TerminalPb_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			TerminalRefresh(g);
		}

		private void TerminalRefresh(Graphics g)
		{
			Font font = new Font("Consolas", 12);
			//g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			//g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.Clear(Color.White);

			Debug.WriteLine($"{_screen.Count} {_screenShowPos0} {_screenEditPos0} {_screenX} {_screenY}");

			for (int y = 0; y < _screenHeight; y++)
			{
				for (int x = 0; x < SCREEN_WIDTH + 1; x++)
				{
					ScreenChar scrChr = null;
					if (y + _screenShowPos0 < _screen.Count)
					{
						scrChr = _screen[_screenShowPos0 + y].Line[x];
					}
					if (x == _screenX && y == _screenY && _screenShowPos0 == _screenEditPos0)
					{
						scrChr = new ScreenChar('_', CharAttributes.Send);
					}
					if (scrChr != null && scrChr.Char != ' ' && scrChr.Char != 0x00)
					{
						Point p = new Point(x * CHAR_WIDTH, y * CHAR_HEIGHT);
						g.DrawString(scrChr.Char.ToString(), font, new SolidBrush(scrChr.AttrColor), p);
					}
				}
			}
		}
	}
}
