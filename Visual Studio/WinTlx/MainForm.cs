using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WinTlx.Config;
using WinTlx.Languages;
using WinTlx.Scheduler;

namespace WinTlx
{
	public partial class MainForm : Form
	{
		private const string TAG = nameof(MainForm);

		private System.Timers.Timer _clockTimer;

		private int _fixedWidth;

		private SubscriberServer _subscriberServer;
		private ItelexProtocol _itelex;
		TapePunchHorizontalForm _tapePunchForm;
		SpecialCharacters _specialCharacters = SpecialCharacters.Instance;

		public const int SCREEN_WIDTH = 68;
		public const int CHAR_HEIGHT = 19;
		public const int CHAR_WIDTH = 9;

		private int _screenHeight = 25;
		private List<ScreenLine> _screen = new List<ScreenLine>();
		private int _screenX = 0;
		private int _screenY = 0;
		private int _screenEditPos0 = 0;
		private int _screenShowPos0 = 0;

		private System.Timers.Timer _outputTimer;
		private Queue<ScreenChar> _outputBuffer;

		private ConfigData _config => ConfigManager.Instance.Config;

		private SchedulerManager _schedulerManager;

		public MainForm()
		{
			InitializeComponent();
			_fixedWidth = this.Width;
			TerminalPb.ContextMenuStrip = CreateContextMenu();

			string x = "✠";

			Logging.Instance.Log(LogTypes.Info, TAG, "Start", $"{Helper.GetVersion()}");

			this.Text = Helper.GetVersion();

			this.KeyPreview = true;

			MemberCb.DataSource = null;
			MemberCb.DisplayMember = "DisplayName";

			SendLineFeedBtn.Text = "\u2261";

			IdleTimoutTb.Text = "";
			ConnTimeTb.Text = "";
			LnColTb.Text = "";
			SendAckTb.Text = "";

			RecvOnCb.Enabled = true;

			this.KeyPreview = true;

			ConfigManager.Instance.LoadConfig();

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageManager.Instance.ChangeLanguage(_config.Language);

			_itelex = new ItelexProtocol();
			_itelex.Received += ReceivedHandler;
			_itelex.Send += SendHandler;
			_itelex.BaudotSendRecv += BaudotSendRecvHandler;
			_itelex.Connected += ConnectedHandler;
			_itelex.Dropped += DroppedHandler;
			_itelex.Update += UpdatedHandler;
			_itelex.Message += MessageHandler;

			_subscriberServer = new SubscriberServer();
			_subscriberServer.Message += SubcribeServerMessageHandler;

			_clockTimer = new System.Timers.Timer(500);
			_clockTimer.Elapsed += ClockTimer_Elapsed;
			_clockTimer.Start();

			_outputBuffer = new Queue<ScreenChar>();
			_outputTimer = new System.Timers.Timer();
			_outputTimer.Elapsed += _outputTimer_Elapsed;
			SetOutputTimer(_config.OutputSpeed);

			_schedulerManager = SchedulerManager.Instance;
			_schedulerManager.Schedule += SchedulerManager_Schedule;
			_schedulerManager.LoadScheduler();

			ClearScreen();

			SetConnectState();
			UpdatedHandler();

			SetFocus();
			SearchTb.Focus();

		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			SearchLbl.Text = LngText(LngKeys.MainForm_SearchText);
			MemberLbl.Text = LngText(LngKeys.MainForm_SearchResult);
			QueryBtn.Text = LngText(LngKeys.MainForm_SearchButton);
			AnswerbackLbl.Text = LngText(LngKeys.MainForm_Answerback);
			AddressLbl.Text = LngText(LngKeys.MainForm_Address);
			PortLbl.Text = LngText(LngKeys.MainForm_Port);
			ExtensionLbl.Text = LngText(LngKeys.MainForm_Extension);
			ProtocolItelexRb.Text = LngText(LngKeys.MainForm_Itelex);
			ProtocolAsciiRb.Text = LngText(LngKeys.MainForm_ASCII);
			ConnectBtn.Text = LngText(LngKeys.MainForm_ConnectButton);
			DisconnectBtn.Text = LngText(LngKeys.MainForm_DisconnectButton);
			LocalBtn.Text = LngText(LngKeys.MainForm_LocalButton);
			SendWruBtn.Text = LngText(LngKeys.MainForm_SendWruButton);
			SendHereIsBtn.Text = LngText(LngKeys.MainForm_SendHereisButton);
			SendLettersBtn.Text = LngText(LngKeys.MainForm_SendLettersButton);
			SendFiguresBtn.Text = LngText(LngKeys.MainForm_SendFiguresButton);
			SendCarriageReturnBtn.Text = LngText(LngKeys.MainForm_SendReturnButton);
			SendLineFeedBtn.Text = LngText(LngKeys.MainForm_SendLinefeedButton);
			SendBellBtn.Text = LngText(LngKeys.MainForm_SendBellButton);
			SendNullBtn.Text = LngText(LngKeys.MainForm_SendNullButton);
			SendTimeBtn.Text = LngText(LngKeys.MainForm_SendTimeButton);
			SendRyBtn.Text = LngText(LngKeys.MainForm_SendRyButton);
			SendFoxBtn.Text = LngText(LngKeys.MainForm_SendPanButton);
			ClearBtn.Text = LngText(LngKeys.MainForm_ClearButton);
			SendFileBtn.Text = LngText(LngKeys.MainForm_SendfileButton);
			RecvOnCb.Text = LngText(LngKeys.MainForm_RecvOnButton);
			UpdateIpAddressBtn.Text = LngText(LngKeys.MainForm_UpdateIpAddressButton);
			TapePunchBtn.Text = LngText(LngKeys.MainForm_TapePunchButton);
			EyeballCharCb.Text = LngText(LngKeys.MainForm_EyeBallCharsButton);
			ConfigBtn.Text = LngText(LngKeys.MainForm_ConfigButton);
			AboutBtn.Text = LngText(LngKeys.MainForm_AboutButton);
			ExitBtn.Text = LngText(LngKeys.MainForm_ExitButton);
			SchedulerBtn.Text = LngText(LngKeys.Scheduler_Scheduler);
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		#region form events

		private void MainForm_Load(object sender, EventArgs e)
		{
			MainForm_Resize(null, null);
			SetFocus();
			SearchTb.Focus();

#if !DEBUG
			string text = $"{Helper.GetVersion()}\r\r" + "by *dg* Detlef Gerhardt\r\r" + LngText(LngKeys.Start_Text);
			MessageBox.Show(
				text,
				$"{Constants.PROGRAM_NAME}",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button1);

#endif
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			//Debug.WriteLine(this.Height);

			// prevent width change
			this.Width = _fixedWidth;

			TerminalPb.Height = this.Height - 310 + 50;
			_screenHeight = TerminalPb.Height / CHAR_HEIGHT;


			_screenEditPos0 = _screen.Count - _screenHeight;
			if (_screenEditPos0 < 0)
				_screenEditPos0 = 0;
			_screenShowPos0 = _screenEditPos0;

			_screenY = _screen.Count - _screenEditPos0 - 1;

			if (_screenY < 0)
				_screenY = 0;

			ShowScreen();

			_tapePunchForm?.SetPosition(this.Bounds);
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Disconnect();
		}

		#endregion

		#region keyboard handling

		/// <summary>
		/// Catch cursor up/cursor down/return
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			// check for controls without special input handling
			if (SearchTb.Focused || MemberCb.Focused || AddressTb.Focused || PortTb.Focused || ExtensionTb.Focused)
			{
				return base.ProcessCmdKey(ref msg, keyData);
			}

			int oldShowPos0 = _screenShowPos0;
			switch (keyData)
			{
				default:
					break;
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
				case Keys.Return:
					SendAsciiText("\r\n");
					return true;
				case Keys.Return | Keys.Shift:
					SendAsciiText("\n");
					return true;
				case Keys.Return | Keys.Control:
					SendAsciiText("\r");
					return true;
				case Keys.C | Keys.Control:
					CopyAction(null, null);
					return true;
				case Keys.V | Keys.Control:
					PasteAction(null, null);
					return true;
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
				return true;
			}

			// all other inputs are handled by the KeyPress event
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			// check for controls without special input handling
			if (SearchTb.Focused || MemberCb.Focused || AddressTb.Focused || PortTb.Focused || ExtensionTb.Focused)
			{
				return;
			}

			char? chr = CodeConversion.KeyboardCharacters(e.KeyChar);
			if (chr != null)
			{
				switch(chr)
				{
					case '\x07': // ctrl-g: send BEL
						SendBel();
						break;
					case '\x09': // ctrl-i: send HERE IS
						SendHereIs();
						break;
					case '\x05': // ctrl-e: send WRU
					case '\x17': // ctrl-w: send WRU
						SendWhoAreYou();
						break;
					default: // all other keys
						SendAsciiText(chr.ToString());
						break;
				}
			}

			e.Handled = true;
		}

		#endregion

		#region timer events

		private void ClockTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			DateTime dt = DateTime.Now;
			Helper.ControlInvokeRequired(DateTb, () => DateTb.Text = $"{dt:dd.MM.yyyy}");
			Helper.ControlInvokeRequired(TimeTb, () => TimeTb.Text = $"{dt:HH:mm:ss}");
		}

		#endregion

		private void TerminalPb_MouseClick(object sender, MouseEventArgs e)
		{
			SetFocus();
		}

		private void TerminalPb_MouseDown(object sender, MouseEventArgs e)
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

		private void PhoneEntryCb_SelectedIndexChanged(object sender, EventArgs e)
		{
			PeerQueryData entry = (PeerQueryData)MemberCb.SelectedItem;
			AddressTb.Text = entry.Address;
			PortTb.Text = entry.PortNumber != 0 ? entry.PortNumber.ToString() : "";
			ExtensionTb.Text = entry.ExtensionNumber != 0 ? entry.ExtensionNumber.ToString() : "";
		}

		private void AddressTb_Leave(object sender, EventArgs e)
		{
			SetConnectState();
		}

		private void PortTb_Leave(object sender, EventArgs e)
		{
			SetConnectState();
		}

		private async void QueryBtn_Click(object sender, EventArgs e)
		{
			SetFocus();

			SearchTb.Text = SearchTb.Text.Trim();

			Logging.Instance.Log(LogTypes.Info, TAG, nameof(QueryBtn_Click), $"SearchText='{SearchTb.Text}'");

			if (string.IsNullOrWhiteSpace(_config.SubscribeServerAddress) || _config.SubscribeServerPort == 0)
			{
				SubcribeServerMessageHandler(LngText(LngKeys.Message_InvalidSubscribeServerData));
				Logging.Instance.Error(TAG, "QueryBtn_Click",
					$"invalid subscribe server data, address={_config.SubscribeServerAddress} port={_config.SubscribeServerPort}");
				return;
			}

			PeerQueryData[] list = null;

			uint num;
			if (!uint.TryParse(SearchTb.Text, out num))
				num = 0;

			await Task.Run(() =>
			{
				if (!_subscriberServer.Connect(_config.SubscribeServerAddress, _config.SubscribeServerPort))
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

			SubcribeServerMessageHandler($"{list?.Length} {LngText(LngKeys.Message_QueryResult)}");

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

		/*
		private async Task<bool> DoQuery(string searchStr)
		{
			searchStr = searchStr.Trim();

			return true;
		}
		*/

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
			//Debug.WriteLine("LocalBtn_Click");
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

		private void ExitBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void SendWruBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendWhoAreYou();
		}

		private void SendHereIsBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendHereIs();
		}

		private void SendBellBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			_itelex.SendAsciiChar(CodeConversion.ASC_BEL);
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

		private void SendNullBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText(CodeConversion.ASC_NUL.ToString());
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
			//SendAsciiText("the quick brown fox jumps over the lazy dog 1234567890/:,-=()");
			SendAsciiText(LngText(LngKeys.Message_Pangram));
			//SendAsciiText("\r\n");
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

		private void SchedulerBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SchedulerForm schedulerForm = new SchedulerForm();
			schedulerForm.ShowDialog();

			return;
		}

		private void RecvOnCb_Click(object sender, EventArgs e)
		{
			if (!_itelex.RecvOn)
			{
				_itelex.SetRecvOn(_config.IncomingLocalPort);
				UpdateIpAddress();
			}
			else
			{
				_itelex.SetRecvOff();
			}
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

		private void EyeballCharCb_CheckedChanged(object sender, EventArgs e)
		{
			if (EyeballCharCb.Checked)
			{
				_itelex.SendAsciiText($"\r\n{LngText(LngKeys.Message_EyeballCharActive)}\r\n");
			}
			_itelex.EyeballCharActive = EyeballCharCb.Checked;
			SetFocus();
		}

		private void ConfigBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			ConfigForm configForm = new ConfigForm(this.Bounds);
			configForm.ShowDialog();
			if (!configForm.Canceled)
			{
				ConfigManager.Instance.SaveConfig();
				SetOutputTimer(_config.OutputSpeed);
				UpdatedHandler();
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

		private void ReceivedHandler(string asciiText)
		{
			for (int i = 0; i < asciiText.Length; i++)
			{
				switch (asciiText[i])
				{
					case CodeConversion.ASC_BEL:
						SystemSounds.Beep.Play();
						AddText(asciiText, CharAttributes.Recv);
						return;
					case CodeConversion.ASC_WRU:
						SendHereIs();
						AddText(asciiText, CharAttributes.Recv);
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
					case CodeConversion.ASC_NUL:
						continue;
					case CodeConversion.ASC_BEL:
						SystemSounds.Beep.Play();
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
			ShowLocalMessage(LngText(LngKeys.Message_Connected));
			SetConnectState();
		}

		private void DroppedHandler()
		{
			ShowLocalMessage(LngText(LngKeys.Message_Disconnected));
			SetConnectState();
		}

		private void UpdatedHandler()
		{
			Helper.ControlInvokeRequired(SendAckTb, () => SendAckTb.Text = $"Snd {_itelex.CharsToSendCount}  Ack {_itelex.CharsAckCount}");
			Helper.ControlInvokeRequired(IdleTimoutTb, () => IdleTimoutTb.Text = $"Timeout {_itelex.IdleTimer} sec");
			Helper.ControlInvokeRequired(ConnTimeTb, () => ConnTimeTb.Text = $"Conn {_itelex.ConnTimeMin} min");

			Helper.ControlInvokeRequired(AnswerbackTb, () => AnswerbackTb.Text = _config.Answerback);

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

			Helper.ControlInvokeRequired(ConnectionStateTb, () =>
				ConnectionStateTb.Text = _itelex.ConnectionStateStr
			);

			Helper.ControlInvokeRequired(RecvOnCb, () =>
			{
				RecvOnCb.Checked = _itelex.RecvOn;
			});
		}

		private async Task<bool> ConnectOut()
		{
			AddressTb.Text = AddressTb.Text.Trim();
			if (string.IsNullOrWhiteSpace(AddressTb.Text))
			{
				ShowLocalMessage(LngText(LngKeys.Message_ConnectNoAddress));
				return false;
			}

			PortTb.Text = PortTb.Text.Trim();
			int? port = Helper.ToInt(PortTb.Text);
			if (port == null)
			{
				ShowLocalMessage(LngText(LngKeys.Message_ConnectInvalidPort));
				return false;
			}

			ExtensionTb.Text = ExtensionTb.Text.Trim();
			int? extension = null;
			if (!string.IsNullOrWhiteSpace(ExtensionTb.Text))
			{
				extension = Helper.ToInt(ExtensionTb.Text);
				if (extension == null)
				{
					ShowLocalMessage(LngText(LngKeys.Message_ConnectInvalidExtension));
					return false;
				}
			}
			else
			{
				extension = 0;
			}

			bool success = await _itelex.ConnectOut(AddressTb.Text, port.Value, extension.Value, false);
			if (!success)
			{
				ShowLocalMessage(LngText(LngKeys.Message_ConnectionError));
				return false;
			}

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

		private void SendBel()
		{
			SendAsciiText(CodeConversion.ASC_BEL.ToString());
		}

		private void SendWhoAreYou()
		{
			SendAsciiText(CodeConversion.ASC_WRU.ToString());
		}

		private void SendHereIs()
		{
#if DEBUG
			SendAsciiText($"\r\n{AnswerbackTb.Text}");
#else
			SendAsciiText($"\r\n{AnswerbackTb.Text} (wintlx)");
#endif
		}

		private void SendAsciiText(string text)
		{
			_itelex?.SendAsciiText(text);
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
			if (fast || _config.OutputSpeed==0)
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
				// 5 data bits + 1 start bit + 1.5 stop bits = 7.5
				_outputTimer.Interval = 1000D / charSec * 7.5D;
				_outputTimer.Start();
			}
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

		private ContextMenuStrip CreateContextMenu()
		{
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			//bool needSep = false;
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
			//ToolStripItem tsi = e.ClickedItem;
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

		private void UpdateIpAddressBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			UpdateIpAddress();
		}

		private void UpdateIpAddress()
		{
			if (_config.SubscribeServerUpdatePin == 0 || _config.OwnNumber == 0 || _config.IncomingLocalPort == 0)
			{
				return;
			}

			_subscriberServer.Connect(_config.SubscribeServerAddress, _config.SubscribeServerPort);
			SendClientUpdate(_config.OwnNumber, _config.SubscribeServerUpdatePin, _config.IncomingPublicPort);
			_subscriberServer.Disconnect();
		}

		private void LinealPnl_Paint(object sender, PaintEventArgs e)
		{
			Helper.PaintRuler(e.Graphics, SCREEN_WIDTH, 8.98F);
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
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
			g.Clear(Color.White);

			//Debug.WriteLine($"{_screen.Count} {_screenShowPos0} {_screenEditPos0} {_screenX} {_screenY}");

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
						//scrChr = new ScreenChar('_', CharAttributes.Send);
						// draw cursor
						Pen pen = new Pen(Color.Red, 2);
						g.DrawLine(pen, x * CHAR_WIDTH+4, y * CHAR_HEIGHT + CHAR_HEIGHT - 2,
							x * CHAR_WIDTH + CHAR_WIDTH+2, y * CHAR_HEIGHT + CHAR_HEIGHT - 2);
					}
					if (scrChr != null && scrChr.Char != ' ' && scrChr.Char != 0x00)
					{
						Point p = new Point(x * CHAR_WIDTH, y * CHAR_HEIGHT);
						switch(scrChr.Char)
						{
							case CodeConversion.ASC_BEL:
								//g.DrawString("⍾", font, new SolidBrush(scrChr.AttrColor), p);
								g.DrawImage(_specialCharacters.GetBell(scrChr.AttrColor), x * CHAR_WIDTH+3, y * CHAR_HEIGHT+3, CHAR_WIDTH, CHAR_HEIGHT);
								break;
							case CodeConversion.ASC_WRU:
								//g.DrawString("✠", font, new SolidBrush(scrChr.AttrColor), p);
								g.DrawImage(_specialCharacters.GetWru(scrChr.AttrColor), x * CHAR_WIDTH+3, y * CHAR_HEIGHT+3, CHAR_WIDTH, CHAR_HEIGHT);
								break;
							default:
								g.DrawString(scrChr.Char.ToString(), font, new SolidBrush(scrChr.AttrColor), p);
								break;
						}
					}
				}
			}

		}

		private void SchedulerManager_Schedule(ScheduleEventArgs args)
		{
			SchedulerItem scheduleItem = args.Item;
			ShowLocalMessage($"schedule {scheduleItem.Destination}");
			Logging.Instance.Warn(TAG, nameof(SchedulerManager_Schedule), $"Schedule {scheduleItem}");
			bool success = DoSchedule(scheduleItem);
			if (!success)
			{
				Logging.Instance.Warn(TAG, nameof(SchedulerManager_Schedule), $"Schedule failed {scheduleItem}");
				ShowLocalMessage($"schedule failed {scheduleItem.Destination}");
			}
		}

		private bool DoSchedule(SchedulerItem scheduleItem)
		{
			if (_itelex.IsConnected)
			{
				return true;
			}

			// get destination address

			if (scheduleItem.DestAddress == null)
			{
				// query number
				if (string.IsNullOrWhiteSpace(scheduleItem.Destination))
				{
					// invalid distination
					Logging.Instance.Warn(TAG, nameof(DoSchedule), $"Invalid destination {scheduleItem.Destination}");
					scheduleItem.Error = true;
					return false;
				}

				uint num;
				string dest = scheduleItem.Destination.Replace(" ", "");
				if (!uint.TryParse(dest, out num))
				{
					Logging.Instance.Warn(TAG, nameof(DoSchedule), $"Invalid destination number {dest}");
					scheduleItem.Error = true;
					return false;
				}

				_subscriberServer.Connect(_config.SubscribeServerAddress, _config.SubscribeServerPort);
				PeerQueryReply queryReply = _subscriberServer.SendPeerQuery(dest);
				_subscriberServer.Disconnect();
				if (!queryReply.Valid || queryReply.Data == null)
				{
					Logging.Instance.Warn(TAG, nameof(DoSchedule), $"Subscribe server query failed");
					SubcribeServerMessageHandler(queryReply.Error);
					return false;
				}
				if (string.IsNullOrEmpty(queryReply.Data?.Address))
				{
					// not host or ip-address
					Logging.Instance.Warn(TAG, nameof(DoSchedule), $"No host or ip-address");
					return false;
				}

				scheduleItem.DestAddress = queryReply.Data.Address;
				scheduleItem.DestPort = queryReply.Data.PortNumber;
				scheduleItem.DestExtension = queryReply.Data.ExtensionNumber;
			}

			// load text file

			string[] fileData;
			try
			{
				fileData = File.ReadAllLines(scheduleItem.Filename);
			}
			catch(Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(DoSchedule), $"Read file '{scheduleItem.Filename}' failed", ex);
				scheduleItem.Error = true;
				return false;
			}

			// connect to destination

			ShowLocalMessage($"schedule send to {scheduleItem.DestAddress}:{scheduleItem.DestPort} {scheduleItem.DestExtension}");
			Logging.Instance.Info(TAG, nameof(DoSchedule), $"Schedule send to {scheduleItem.DestAddress}:{scheduleItem.DestPort} {scheduleItem.DestExtension}");

			Task<bool> task = _itelex.ConnectOut(scheduleItem.DestAddress, scheduleItem.DestPort, scheduleItem.DestExtension);
			task.Wait();
			if (!task.Result)
			{
				Logging.Instance.Info(TAG, nameof(DoSchedule), $"Connect failed");
				return false;
			}

			if (!_itelex.IsConnected)
			{
				Logging.Instance.Info(TAG, nameof(DoSchedule), $"No connection");
				return false;
			}

			// send text

			try
			{
				// wait 30 seconds for greeting message to finish
				WaitRecv(5000);
				//ShowLocalMessage("wait greeting ok");

				_itelex.SendAsciiText("\r\n");
				WaitSend(5000);

				ScheduleWru();

				_itelex.SendAsciiText("\r\n");
				WaitSend(5000);

				foreach (string line in fileData)
				{
					int col = 0;
					for (int i=0; i<line.Length; i++)
					{
						if (line[i]==CodeConversion.ASC_CR)
						{
							col = 0;
						}
						if (col>68)
						{
							_itelex.SendAsciiText("\r\n");
							col = 0;
						}
						_itelex.SendAsciiChar(line[i]);
						col++;
					}
					_itelex.SendAsciiText("\r\n");
					WaitSend(5000);
				}

				_itelex.SendAsciiText("\r\n");
				WaitSend(5000);

				//ShowLocalMessage("wait text ok");

				ScheduleWru();

				_itelex.SendAsciiText("\r\n\r\n\r\n\r\n");
				WaitSend(5000);

				Logging.Instance.Debug(TAG, nameof(DoSchedule), $"Success");
				scheduleItem.Success = true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(DoSchedule), $"Error sending text", ex);
				ShowLocalMessage("recv timeout");
				return false;
			}
			finally
			{
				if (_itelex.IsConnected)
				{
					Disconnect();
				}
			}

			return true;
		}

		private void ScheduleWru()
		{
			_itelex.SendAsciiChar(CodeConversion.ASC_WRU);
			WaitSend(5000);
			WaitRecv(5000);
			//ShowLocalMessage("wait WRU ok");

			SendAsciiText($"\r\n{AnswerbackTb.Text}\r\n");
			WaitSend(5000);
			//ShowLocalMessage("wait here is ok");
		}

		private void WaitSend(int millis)
		{
			WaitSendRecv(millis, () => { return _itelex.CharsToSendCount == 0 && _outputBuffer.Count == 0; });
		}

		private void WaitRecv(int millis)
		{
			WaitSendRecv(millis, () => { return _itelex.CharsAckCount == 0 && _outputBuffer.Count == 0; });
		}

		private void WaitSendRecv(int millis, Func<bool> chr0)
		{
			Thread.Sleep(2000);
			int lastSendCnt = -1;
			int lastSend0Cnt = 0;

			long ticks = Helper.MilliTicks();
			while (true)
			{
				Thread.Sleep(100);

				bool lastSend0 = chr0();
				if (lastSend0)
				{
					lastSend0Cnt++;
					if (lastSend0Cnt > 10)
					{
						return;
					};
					continue;
				}

				if (_itelex.CharsToSendCount == lastSendCnt)
				{
					// no change
					if (Helper.MilliTicks() - ticks > millis)
					{
						// timeout
						throw new Exception("recv timeout");
					}
				}
				else
				{
					lastSendCnt = _itelex.CharsAckCount;
					ticks = Helper.MilliTicks();
				}
			}
		}
	}
}
