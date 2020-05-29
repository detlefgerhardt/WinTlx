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
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Languages;
using WinTlx.Scheduler;
using WinTlx.TapePunch;
using WinTlx.TextEditor;

namespace WinTlx
{
	public partial class MainForm : Form
	{
		private const string TAG = nameof(MainForm);

		private System.Timers.Timer _clockTimer;
		private System.Timers.Timer _focusTimer;

		private int _fixedWidth;

		private SubscriberServer _subscriberServer;
		private ItelexProtocol _itelex;
		TapePunchForm _tapePunchForm;
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

		/// <summary>
		/// Timer for screen output buffer
		/// </summary>
		private System.Timers.Timer _outputTimer;
		private Queue<ScreenChar> _outputBuffer;

		/// <summary>
		/// Timer for send buffer
		/// </summary>
		private bool _sendTimerActive;
		private System.Timers.Timer _sendTimer;
		private Queue<char> _sendBuffer;

		private ConfigManager _configManager;

		private ConfigData _configData;

		private SchedulerManager _schedulerManager;

		private TapePunchManager _tapePunch;

		public MainForm()
		{
			InitializeComponent();

			_fixedWidth = this.Width;
			TerminalPb.ContextMenuStrip = CreateContextMenu();

			//string x = "✠";

			// order is import, logging needs Logfile path from config !!!
			_configManager = ConfigManager.Instance;
			_configManager.LoadConfig();
			_configData = _configManager.Config;
			Logging.Instance.LogfilePath = _configData.LogfilePath;

			TextEditorManager.Instance.Send += TextEditor_Send;

			Logging.Instance.Info(TAG, nameof(MainForm), $"---------- Start {Helper.GetVersion()} ----------");

			this.Text = Helper.GetVersion();
			this.KeyPreview = true;
			this.Enter += MainForm_Enter;
			this.Deactivate += MainForm_Deactivate;

			TlnNameCb.DataSource = null;
			TlnNameCb.DisplayMember = "DisplayName";

			SendLineFeedBtn.Text = "\u2261";

			ScrollStartBtn.Text = "";
			ScrollStartBtn.BackgroundImage = SpecialCharacters.Instance.GetScrollStart(Color.Black);
			ScrollUpBtn.Text = "";
			ScrollUpBtn.BackgroundImage = SpecialCharacters.Instance.GetScrollUp(Color.Black);
			ScrollDownBtn.Text = "";
			ScrollDownBtn.BackgroundImage = SpecialCharacters.Instance.GetScrollDown(Color.Black);
			ScrollEndBtn.Text = "";
			ScrollEndBtn.BackgroundImage = SpecialCharacters.Instance.GetScrollEnd(Color.Black);

			//ScrollStartBtn.Text = "\u22BC";
			//ScrollUpBtn.Text = "\u2227";
			//ScrollDownBtn.Text = "\u2228";
			//ScrollEndBtn.Text = "\u22BB";

			IdleTimoutTb.Text = "";
			ConnTimeTb.Text = "";
			LnColTb.Text = "";
			SendAckTb.Text = "";
			RecvBufTb.Text = "";

			RecvOnCb.Enabled = true;

//#if !DEBUG
			ProtocolItelexRb.Enabled = false;
			ProtocolAsciiRb.Enabled = false;
//#endif

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageManager.Instance.ChangeLanguage(_configData.Language);

			_itelex = ItelexProtocol.Instance;
			_itelex.Received += ReceivedHandler;
			_itelex.Send += SendHandler;
			_itelex.Connected += ConnectedHandler;
			_itelex.Dropped += DroppedHandler;
			_itelex.Update += UpdateHandler;
			_itelex.Message += MessageHandler;

			_subscriberServer = new SubscriberServer();
			_subscriberServer.Message += SubcribeServerMessageHandler;

			_clockTimer = new System.Timers.Timer(500);
			_clockTimer.Elapsed += ClockTimer_Elapsed;
			_clockTimer.Start();

			_focusTimer = new System.Timers.Timer(100);
			_focusTimer.Elapsed += FocusTimer_Elapsed;
			_focusTimer.Start();

			_outputBuffer = new Queue<ScreenChar>();
			_outputTimer = new System.Timers.Timer();
			_outputTimer.Elapsed += OutputTimer_Elapsed;
			SetOutputTimer(_configData.OutputSpeed);

			_sendBuffer = new Queue<char>();
			_sendTimerActive = false;
			_sendTimer = new System.Timers.Timer(100);
			_sendTimer.Elapsed += SendTimer_Elapsed;
			_sendTimer.Start();

			_tapePunch = TapePunchManager.Instance;

			_schedulerManager = SchedulerManager.Instance;
			_schedulerManager.Schedule += SchedulerManager_Schedule;
			_schedulerManager.LoadScheduler();

			ClearScreen();

			//SetConnectState();
			UpdateHandler();

			//SetFocus();
			//SearchTb.Focus();
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			SearchLbl.Text = LngText(LngKeys.MainForm_SearchText);
			Helper.SetToolTip(SearchLbl, LngText(LngKeys.MainForm_SearchText_ToolTip));
			TlnMemberLbl.Text = LngText(LngKeys.MainForm_SearchResult);
			QueryBtn.Text = LngText(LngKeys.MainForm_SearchButton);
			Helper.SetToolTip(QueryBtn, LngText(LngKeys.MainForm_SearchButton_ToolTip));
			AnswerbackLbl.Text = LngText(LngKeys.MainForm_Answerback);
			Helper.SetToolTip(AnswerbackLbl, LngText(LngKeys.MainForm_Answerback_ToolTip));
			TlnAddressLbl.Text = LngText(LngKeys.MainForm_Address);
			TlnPortLbl.Text = LngText(LngKeys.MainForm_Port);
			TlnExtensionLbl.Text = LngText(LngKeys.MainForm_Extension);
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
			Helper.SetToolTip(SendNullBtn, LngText(LngKeys.MainForm_SendNullButton_ToolTip));
			SendTimeBtn.Text = LngText(LngKeys.MainForm_SendTimeButton);
			SendRyBtn.Text = LngText(LngKeys.MainForm_SendRyButton);
			SendFoxBtn.Text = LngText(LngKeys.MainForm_SendPanButton);
			ClearBtn.Text = LngText(LngKeys.MainForm_ClearButton);
			Helper.SetToolTip(ClearBtn, LngText(LngKeys.MainForm_ClearButton_ToolTip));
			TextEditorBtn.Text = LngText(LngKeys.MainForm_EditorButton);
			Helper.SetToolTip(TextEditorBtn, LngText(LngKeys.MainForm_EditorButton_ToolTip));
			RecvOnCb.Text = LngText(LngKeys.MainForm_RecvOnButton);
			Helper.SetToolTip(RecvOnCb, LngText(LngKeys.MainForm_RecvOnButton_ToolTip));
			UpdateIpAddressBtn.Text = LngText(LngKeys.MainForm_UpdateIpAddressButton);
			Helper.SetToolTip(UpdateIpAddressBtn, LngText(LngKeys.MainForm_UpdateIpAddressButton_ToolTip));
			TapePunchBtn.Text = LngText(LngKeys.MainForm_TapePunchButton);
			Helper.SetToolTip(TapePunchBtn, LngText(LngKeys.MainForm_TapePunchButton_ToolTip));
			EyeballCharCb.Text = LngText(LngKeys.MainForm_EyeBallCharsButton);
			Helper.SetToolTip(EyeballCharCb, LngText(LngKeys.MainForm_EyeBallCharsButton_ToolTip));
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

		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			Debug.WriteLine(nameof(MainForm_Deactivate));
			TimeTb.Focus();
			TerminalPb.Refresh();
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			SetFocus();
			//SearchTb.Focus();
		}

		private void MainForm_Enter(object sender, EventArgs e)
		{
			Debug.WriteLine(nameof(MainForm_Enter));
		}

		private void MainForm_Leave(object sender, EventArgs e)
		{
			Debug.WriteLine(nameof(MainForm_Leave));
			TerminalPb.Refresh();
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

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// make shure that editor text is saved
			e.Cancel = !TextEditorManager.Instance.Closing();
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
			//if (SearchTb.Focused || MemberCb.Focused || AddressTb.Focused || PortTb.Focused || ExtensionTb.Focused)
			if (!TerminalPb.Focused)
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
					SendAsciiChar('\n');
					return true;
				case Keys.Return | Keys.Control:
					SendAsciiChar('\r');
					return true;
				case Keys.C | Keys.Control:
					CopyAction(null, null);
					return true;
				case Keys.V | Keys.Control:
					DoPaste();
					return true;
				case Keys.F | Keys.Alt:
					SendAsciiChar(CodeManager.ASC_SHIFTF);
					break;
				case Keys.G | Keys.Alt:
					SendAsciiChar(CodeManager.ASC_SHIFTG);
					break;
				case Keys.H | Keys.Alt:
					SendAsciiChar(CodeManager.ASC_SHIFTH);
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
				return true;
			}

			// all other inputs are handled by the KeyPress event
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			// check for controls without special input handling
			if (SearchTb.Focused || TlnNameCb.Focused || TlnAddressTb.Focused || TlnPortTb.Focused || TlnExtensionTb.Focused)
			{
				return;
			}

			if (!TerminalPb.Focused)
			{
				SetFocus();
			}

			char? chr = CodeManager.KeyboardCharacters(e.KeyChar);
			if (chr != null)
			{
				switch(chr)
				{
					case CodeManager.ASC_BEL: // ctrl-g: send BEL
						SendBel();
						break;
					case CodeManager.ASC_HEREIS: // ctrl-i
						SendHereIs();
						break;
					case CodeManager.ASC_WRU: // ctrl-e
					case '\x17': // ctrl-w: send WRU
						SendWhoAreYou();
						break;
					default: // all other keys
						SendAsciiChar(chr.Value);
						break;
				}
			}

			e.Handled = true;
		}

#endregion

#region timer events

		private void ClockTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			// clock

			DateTime dt = DateTime.Now;
			Helper.ControlInvokeRequired(DateTb, () => DateTb.Text = $"{dt:dd.MM.yyyy}");
			Helper.ControlInvokeRequired(TimeTb, () => TimeTb.Text = $"{dt:HH:mm:ss}");

			// timeout

			Helper.ControlInvokeRequired(IdleTimoutTb, () =>
			{
				IdleTimoutTb.ForeColor = Color.Black;
				if (!_itelex.IsConnected || _configData.IdleTimeout == 0)
				{
					IdleTimoutTb.Text = $"Timeout -";
				}
				else
				{
					int timeout = _configData.IdleTimeout - _itelex.IdleTimerMs / 1000;
					IdleTimoutTb.Text = $"Timeout {timeout} s";
					if (timeout<10)
					{
						IdleTimoutTb.ForeColor = Color.Red;
					}
				}
				// trigger change of forecolor
				IdleTimoutTb.BackColor = IdleTimoutTb.BackColor;
			});

			if (_itelex.IsConnected && _configData.IdleTimeout>0 && IdleTimeout()==0)
			{
				ShowLocalMessage("inactivity timeout");
				_itelex.Disconnect();
			}
		}

		private int IdleTimeout()
		{
			if (!_itelex.IsConnected)
			{
				return 0;
			}
			int timeout = _configData.IdleTimeout - _itelex.IdleTimerMs / 1000;
			if (timeout<0)
			{
				timeout = 0;
			}
			return timeout;
		}

		private void FocusTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			//bool focus = !SearchTb.Focused && !MemberCb.Focused && !AddressTb.Focused && !PortTb.Focused && ExtensionTb.Focused;
		}

#endregion

#region TerminalPb-Events

		private void TerminalPb_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			TerminalDraw(g, TerminalPb.Focused);
		}

		private void TerminalPb_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{   //click event
				ContextMenu contextMenu = new ContextMenu();
				MenuItem menuItem = new MenuItem("Clear");
				menuItem.Click += ClearAction;
				contextMenu.MenuItems.Add(menuItem);
				menuItem = new MenuItem("Copy");
				menuItem.Click += CopyAction;
				contextMenu.MenuItems.Add(menuItem);
				menuItem = new MenuItem("Paste");
				menuItem.Click += PasteAction;
				contextMenu.MenuItems.Add(menuItem);
				TerminalPb.ContextMenu = contextMenu;
			}
		}

#endregion

		private void PhoneEntryCb_SelectedIndexChanged(object sender, EventArgs e)
		{
			PeerQueryData entry = (PeerQueryData)TlnNameCb.SelectedItem;
			if (entry == null)
			{
				return;
			}
			TlnAddressTb.Text = entry.Address;
			TlnPortTb.Text = entry.PortNumber != 0 ? entry.PortNumber.ToString() : "";
			TlnExtensionTb.Text = entry.ExtensionNumber != 0 ? entry.ExtensionNumber.ToString() : "";
			TlnTypeTb.Text = entry.PeerType.ToString();
		}

		private void AddressTb_Leave(object sender, EventArgs e)
		{
			//SetConnectState();
			UpdateHandler();
		}

		private void PortTb_Leave(object sender, EventArgs e)
		{
			//SetConnectState();
			UpdateHandler();
		}

		/// <summary>
		/// Use Validated event instead of Leave event, because Leave event fires twice
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void SearchTb_Validated(object sender, EventArgs e)
		{
			//if (!string.IsNullOrWhiteSpace(SearchTb.Text))
			//{
			//	await ActiveQuery();
			//}
		}

		private async void SearchTb_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar=='\r')
			{
				await ActiveQuery();
				//QueryBtn.Focus();
			}
		}

		private async void QueryBtn_Click(object sender, EventArgs e)
		{
			await ActiveQuery();
		}

		private bool _queryActive = false;

		private async Task ActiveQuery()
		{
			if (_queryActive)
			{
				return;
			}

			SearchTb.Enabled = false;
			QueryBtn.Enabled = false;

			_queryActive = true;
			await Query();
			_queryActive = false;

			SearchTb.Enabled = true;
			QueryBtn.Enabled = true;
		}

		private async Task Query()
		{
			SetFocus();

			TlnNameCb.DataSource = null;
			TlnNameCb.Text = "";
			TlnAddressTb.Text = "";
			TlnPortTb.Text = "";
			TlnExtensionTb.Text = "";
			TlnTypeTb.Text = "";

			SearchTb.Text = SearchTb.Text.Trim();
			if (string.IsNullOrWhiteSpace(SearchTb.Text))
			{
				return;
			}

			Logging.Instance.Log(LogTypes.Info, TAG, nameof(QueryBtn_Click), $"SearchText='{SearchTb.Text}'");

			if (string.IsNullOrWhiteSpace(_configData.SubscribeServerAddress) || _configData.SubscribeServerPort == 0)
			{
				SubcribeServerMessageHandler(LngText(LngKeys.Message_InvalidSubscribeServerData));
				Logging.Instance.Error(TAG, "QueryBtn_Click",
					$"invalid subscribe server data, address={_configData.SubscribeServerAddress} port={_configData.SubscribeServerPort}");
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

			SubcribeServerMessageHandler($"{list?.Length} {LngText(LngKeys.Message_QueryResult)}");

			TlnNameCb.DataSource = list;
			TlnNameCb.DisplayMember = "Display";
			if (list == null)
			{
			}
			else if (list.Length == 0)
			{
				TlnNameCb.Text = "";
				TlnAddressTb.Text = "";
				TlnPortTb.Text = "";
				TlnExtensionTb.Text = "";
				TlnTypeTb.Text = "";
			}
			else
			{
				TlnNameCb.SelectedIndex = 0;
			}

			//SetConnectState();
			UpdateHandler();
		}

		private async void ConnectBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			if (_itelex.IsConnected || string.IsNullOrEmpty(TlnPortTb.Text))
			{
				return;
			}

			ConnectBtn.Enabled = false;
			await ConnectOut();
			//ConnectBtn.Enabled = true;
			//SetConnectState();
			UpdateHandler();
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
				_itelex.Local = false;
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
			//SetConnectState();
			UpdateHandler();
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
			SendAsciiChar(CodeManager.ASC_BEL);
		}

		private void SendLettersBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiChar(CodeManager.ASC_LTRS);
		}

		private void SendFiguresBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiChar(CodeManager.ASC_FIGS);
		}

		private void SendCarriageReturnBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiChar('\r');
		}

		private void SendLineFeedBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiChar('\n');
		}

		private void SendTimeBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText($"{DateTime.Now:dd.MM.yyyy  HH:mm}\r\n");
		}

		private void SendNullBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiChar(CodeManager.ASC_NUL);
		}

		private void SendLineBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText("\r\n");
			SendAsciiText(new string('-', 68));
			//SendAsciiText("\r\n");
		}

		private void SendRyBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText("\r\n");
			for (int i = 0; i < 34; i++)
			{
				SendAsciiText("ry");
			}

			/*
			for (int l = 0; l < 6; l++)
			{
				for (int i = 0; i < 34; i++)
				{
					SendAsciiText("ry");
				}
				SendAsciiText("\r\n");
			}
			*/
		}

		private void SendFoxBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiText("\r\n");
			SendAsciiText(LngText(LngKeys.Message_Pangram));
		}

		private void ClearBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			ClearScreen();
		}

		private void ScrollStartBtn_Click(object sender, EventArgs e)
		{
			SetScreenPos(0);
			SetFocus();
		}

		private void ScrollUpBtn_Click(object sender, EventArgs e)
		{
			SetScreenPos(_screenShowPos0 - _screenHeight - 1);
			SetFocus();
		}

		private void ScrollDownBtn_Click(object sender, EventArgs e)
		{
			SetScreenPos(_screenShowPos0 + _screenHeight - 1);
			SetFocus();
		}

		private void ScrollEndBtn_Click(object sender, EventArgs e)
		{
			SetScreenPos(_screen.Count - _screenHeight);
			SetFocus();
		}

		private void SetScreenPos(int newPos)
		{
			if (newPos > _screen.Count - _screenHeight)
			{
				_screenShowPos0 = _screen.Count - _screenHeight;
			}
			else if (newPos < 0)
			{
				_screenShowPos0 = 0;
			}
			else
			{
				_screenShowPos0 = newPos;
			}
			ShowScreen();
		}

		private void SchedulerBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SchedulerForm schedulerForm = new SchedulerForm();
			schedulerForm.ShowDialog();
			return;
		}

		private async void RecvOnCb_Click(object sender, EventArgs e)
		{
			if (!_itelex.RecvOn)
			{
				if (!_configData.LimitedClient)
				{
					_itelex.SetRecvOn(_configData.IncomingLocalPort);
					UpdateIpAddress();
				}
				else
				{
					if (await _itelex.CentralexConnectAsync(_configData.RemoteServerAddress, _configData.RemoteServerPort))
					{
						RecvOnCb.Checked = true;
					}
					else
					{
						RecvOnCb.Checked = false;
					}
				}
			}
			else
			{
				if (!_configData.LimitedClient)
				{
					_itelex.SetRecvOff();
				}
				else
				{
					_itelex.Disconnect();
					_itelex.CentralexDisconnect();
				}
			}
			UpdateHandler();
		}

		private void TapePunchBtn_Click(object sender, EventArgs e)
		{
			if (_tapePunchForm == null)
			{
				_tapePunchForm = new TapePunchForm(this.Bounds);
				_tapePunchForm.Closed += TapePunchForm_Closed;
				_tapePunchForm.Click += TapePunchForm_Click;
				_tapePunchForm.Show();
			}
			else
			{
				_tapePunchForm.Close();
			}
			SetFocus();
		}

		private void TapePunchForm_Click()
		{
			throw new NotImplementedException();
		}

		private void TapePunchForm_Closed()
		{
			_tapePunchForm.Closed -= TapePunchForm_Closed;
			_tapePunchForm.Click -= TapePunchForm_Click;
			_tapePunchForm = null;
		}

		private void EyeballCharCb_CheckedChanged(object sender, EventArgs e)
		{
			if (EyeballCharCb.Checked)
			{
				SendAsciiText($"\r\n{LngText(LngKeys.Message_EyeballCharActive)}\r\n");
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
				SetOutputTimer(_configData.OutputSpeed);
				Logging.Instance.LogfilePath = _configData.LogfilePath;
				UpdateHandler();
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
			//Debug.WriteLine(asciiText);
			for (int i = 0; i < asciiText.Length; i++)
			{
				switch (asciiText[i])
				{
					case CodeManager.ASC_BEL:
						SystemSounds.Beep.Play();
						AddText(asciiText, CharAttributes.Recv);
						return;
					case CodeManager.ASC_WRU:
						SendHereIs();
						AddText(asciiText, CharAttributes.Recv);
						return;
				}
				AddText(asciiText[i].ToString(), CharAttributes.Recv);
			}
		}

		private void SendHandler(string asciiText)
		{
			string dispText = "";
			for (int i = 0; i < asciiText.Length; i++)
			{
				char c = asciiText[i];
				switch (c)
				{
					case CodeManager.ASC_NUL:
						continue;
					case CodeManager.ASC_BEL:
						SystemSounds.Beep.Play();
						break;
					case CodeManager.ASC_LTRS:
					case CodeManager.ASC_FIGS:
						continue;
				}
				dispText += c;
			}
			Debug.WriteLine(dispText);
			AddText(dispText, CharAttributes.Send);
		}

		private void SubcribeServerMessageHandler(string message)
		{
			ShowLocalMessage(message);
		}

		private void ConnectedHandler()
		{
			ShowLocalMessage(LngText(LngKeys.Message_Connected));
			//SetConnectState();
			UpdateHandler();
		}

		private void DroppedHandler()
		{
			Debug.WriteLine($"DroppedHandler() ConnectionState={_itelex.ConnectionState}");

			ShowLocalMessage(LngText(LngKeys.Message_Disconnected));
			//Helper.ControlInvokeRequired(ConnectBtn, () => ConnectBtn.Enabled = true);
			//Helper.ControlInvokeRequired(DisconnectBtn, () => DisconnectBtn.Enabled = false);

			//SetConnectState();
			UpdateHandler();
		}

		/*
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
		*/

		private void UpdateHandler()
		{
			Helper.ControlInvokeRequired(ConnectBtn, () =>
			{
				Debug.WriteLine($"UpdateHandler() ConnectionState={_itelex.ConnectionState}");
				if (!_itelex.IsConnected)
				{
					ConnectBtn.Enabled = true;
					ConnectBtn.ForeColor = Color.Black;
				}
				else
				{
					ConnectBtn.Enabled = false;
					ConnectBtn.ForeColor = Color.Green;
				}
			});

			Helper.ControlInvokeRequired(DisconnectBtn, () =>
			{
				if (!_itelex.IsConnected)
				{
					DisconnectBtn.Enabled = false;
				}
				else
				{
					DisconnectBtn.Enabled = true;
				}
			});

			Helper.ControlInvokeRequired(LocalBtn, () =>
			{
				LocalBtn.ForeColor = _itelex.Local ? Color.Green : Color.Black;
				if (!_itelex.IsConnected)
				{
					LocalBtn.Enabled = false;
				}
				else
				{
					LocalBtn.Enabled = true;
				}
			});

			Helper.ControlInvokeRequired(RecvOnCb, () =>
			{
				RecvOnCb.Enabled = !_itelex.IsConnected;
				RecvOnCb.Checked = _itelex.RecvOn;
			});

			Helper.ControlInvokeRequired(UpdateIpAddressBtn, () =>
			{
				UpdateIpAddressBtn.Enabled = !_itelex.IsConnected;
			});

			Helper.ControlInvokeRequired(ProtocolItelexRb, () =>
			{
				if (_itelex.IsConnected)
				{
					ProtocolItelexRb.Checked = _itelex.ConnectionState == ItelexProtocol.ConnectionStates.ItelexTexting;
					ProtocolAsciiRb.Checked = _itelex.ConnectionState == ItelexProtocol.ConnectionStates.AsciiTexting;
				}
			});

			Helper.ControlInvokeRequired(SendAckTb, () => SendAckTb.Text = $"Itx Buf={_itelex.CharsToSendCount} Ack={_itelex.CharsAckCount}");
			Helper.ControlInvokeRequired(RecvBufTb, () => RecvBufTb.Text = $"I={_outputBuffer.Count} S={_sendBuffer.Count}");

			Helper.ControlInvokeRequired(ConnTimeTb, () => ConnTimeTb.Text = $"Conn {_itelex.ConnTimeMin} min");

			Helper.ControlInvokeRequired(AnswerbackTb, () => AnswerbackTb.Text = _configData.Answerback);

			Helper.ControlInvokeRequired(SendLettersBtn, () =>
			{
				if (_itelex.ShiftState == ShiftStates.Unknown || _itelex.ShiftState == ShiftStates.Figs)
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
				if (_itelex.ShiftState == ShiftStates.Unknown || _itelex.ShiftState == ShiftStates.Ltr)
				{
					SendFiguresBtn.ForeColor = Color.Black;
				}
				else
				{
					SendFiguresBtn.ForeColor = Color.Green;
				}
			});

			Helper.ControlInvokeRequired(ConnectionStateTb, () =>
				ConnectionStateTb.Text = _itelex.ConnectionStateString
			);

		}

		private async Task<bool> ConnectOut()
		{
			TlnAddressTb.Text = TlnAddressTb.Text.Trim();
			if (string.IsNullOrWhiteSpace(TlnAddressTb.Text))
			{
				ShowLocalMessage(LngText(LngKeys.Message_ConnectNoAddress));
				return false;
			}

			TlnPortTb.Text = TlnPortTb.Text.Trim();
			int? port = Helper.ToInt(TlnPortTb.Text);
			if (port == null)
			{
				ShowLocalMessage(LngText(LngKeys.Message_ConnectInvalidPort));
				return false;
			}

			TlnExtensionTb.Text = TlnExtensionTb.Text.Trim();
			int? extension = null;
			if (!string.IsNullOrWhiteSpace(TlnExtensionTb.Text))
			{
				extension = Helper.ToInt(TlnExtensionTb.Text);
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

			bool success = await _itelex.ConnectOut(TlnAddressTb.Text, port.Value, extension.Value, false);
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
				//SetConnectState();
				//UpdatedHandler();
			}
		}

		private void SendBel()
		{
			SendAsciiChar(CodeManager.ASC_BEL);
		}

		private void SendWhoAreYou()
		{
			SendAsciiChar(CodeManager.ASC_WRU);
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
			foreach(char chr in text)
			{
				SendAsciiChar(chr);
			}
		}

		private void SendAsciiChar(char chr)
		{
			_sendBuffer.Enqueue(chr);
			UpdateHandler();
		}

		private void SendTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_sendTimerActive)
			{
				return;
			}

			try
			{
				_sendTimerActive = true;
				if (_sendBuffer.Count == 0)
				{
					return;
				}
				while (_itelex.GetSendBufferCount() < 10 && _sendBuffer.Count > 0)
				{
					_itelex.SendAsciiChar(_sendBuffer.Dequeue());
				}
				UpdateHandler();
			}
			finally
			{
				_sendTimerActive = false;
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
				// 5 data bits + 1 start bit + 1.5 stop bits = 7.5
				_outputTimer.Interval = 1000D / charSec * 7.5D;
				_outputTimer.Start();
			}
		}

		private void OutputTimer_Elapsed(object sender, ElapsedEventArgs e)
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
						//_screen[_screenEditPos0 + _screenY].Line[_screenX] = new ScreenChar(asciiText[i], attr);
						_screen[_screenEditPos0 + _screenY].Line[_screenX].Char = asciiText[i];
						_screen[_screenEditPos0 + _screenY].Line[_screenX].Attr = attr;
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
			TerminalPb.Refresh();
		}

		private object _commLogLock = new object();

		private void CommLog(string text)
		{
			lock (_commLogLock)
			{
				string fullName = "";
				try
				{
					string path = string.IsNullOrWhiteSpace(_configData.LogfilePath) ? Helper.GetExePath() : _configData.LogfilePath;
					fullName = Path.Combine(path, Constants.CONSOLE_LOG);
					File.AppendAllText(fullName, text);
				}
				catch (Exception ex)
				{
					string newName = Path.Combine(Helper.GetExePath(), Constants.CONSOLE_LOG);
					File.AppendAllText(newName, text);
					Logging.Instance.Error(TAG, nameof(CommLog), $"Error writing console log to {fullName}", ex);
				}
			}
		}

		private ContextMenuStrip CreateContextMenu()
		{
			ContextMenuStrip contextMenu = new ContextMenuStrip();
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
			DoPaste();
		}

		private void DoPaste()
		{
			if (Clipboard.ContainsText(TextDataFormat.Text))
			{
				string text = Clipboard.GetText();
				SendAsciiText(text);
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

		private void SendClientUpdate(int number, int pin, int publicPort)
		{
			ClientUpdateReply reply = _subscriberServer.SendClientUpdate(number, pin, publicPort);
			if (reply.Success)
			{
				SubcribeServerMessageHandler($"update {number} ok / {reply.IpAddress}:{publicPort}");
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
			if (_configData.SubscribeServerUpdatePin == 0 || _configData.OwnNumber == 0 || _configData.IncomingLocalPort == 0)
			{
				return;
			}

			_subscriberServer.Connect(_configData.SubscribeServerAddress, _configData.SubscribeServerPort);
			SendClientUpdate(_configData.OwnNumber, _configData.SubscribeServerUpdatePin, _configData.IncomingPublicPort);
			_subscriberServer.Disconnect();
		}

		private void LinealPnl_Paint(object sender, PaintEventArgs e)
		{
			Helper.PaintRuler(e.Graphics, SCREEN_WIDTH, 8.98F);
		}

		private void TerminalDraw(Graphics g, bool focus)
		{
			Font font = new Font("Consolas", 12);
			//g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
			if (focus)
			{
				g.Clear(Color.White);
			}
			else
			{
				g.Clear(Color.LightGray);
			}

			// Debug.WriteLine($"{_screen.Count} {_screenShowPos0} {_screenEditPos0} {_screenX} {_screenY}");

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
						// draw cursor
						Pen pen = new Pen(Color.Red, 2);
						g.DrawLine(pen, x * CHAR_WIDTH + 4, y * CHAR_HEIGHT + CHAR_HEIGHT - 2,
									x * CHAR_WIDTH + CHAR_WIDTH + 2, y * CHAR_HEIGHT + CHAR_HEIGHT - 2);
					}
					if (scrChr != null)
					{
						TerminalDrawChar(g, font, x, y, scrChr);
					}
				}
			}
		}

		private void TerminalDrawChar(Graphics g, Font font, int x, int y, ScreenChar scrChr)
		{
			for (int c = 0; c < scrChr.Chars.Count; c++)
			{
				char chr = scrChr.Chars[c];
				if (chr != ' ' && chr != 0x00)
				{
					Point p = new Point(x * CHAR_WIDTH, y * CHAR_HEIGHT);
					switch (chr)
					{
						case CodeManager.ASC_BEL:
							//g.DrawString("⍾", font, new SolidBrush(scrChr.AttrColor), p);
							g.DrawImage(_specialCharacters.GetBell(scrChr.AttrColor), x * CHAR_WIDTH + 3, y * CHAR_HEIGHT + 3, CHAR_WIDTH, CHAR_HEIGHT);
							break;
						case CodeManager.ASC_WRU:
							//g.DrawString("✠", font, new SolidBrush(scrChr.AttrColor), p);
							g.DrawImage(_specialCharacters.GetWru(scrChr.AttrColor), x * CHAR_WIDTH + 3, y * CHAR_HEIGHT + 3, CHAR_WIDTH, CHAR_HEIGHT);
							break;
						case CodeManager.ASC_SHIFTF:
							g.DrawImage(_specialCharacters.GetShiftF(scrChr.AttrColor), x * CHAR_WIDTH + 3, y * CHAR_HEIGHT + 3, CHAR_WIDTH, CHAR_HEIGHT);
							break;
						case CodeManager.ASC_SHIFTG:
							g.DrawImage(_specialCharacters.GetShiftG(scrChr.AttrColor), x * CHAR_WIDTH + 3, y * CHAR_HEIGHT + 3, CHAR_WIDTH, CHAR_HEIGHT);
							break;
						case CodeManager.ASC_SHIFTH:
							g.DrawImage(_specialCharacters.GetShiftH(scrChr.AttrColor), x * CHAR_WIDTH + 3, y * CHAR_HEIGHT + 3, CHAR_WIDTH, CHAR_HEIGHT);
							break;
						default:
							g.DrawString(chr.ToString(), font, new SolidBrush(scrChr.AttrColor), p);
							break;
					}
				}
			}
		}

#region Scheduler

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

				_subscriberServer.Connect(_configData.SubscribeServerAddress, _configData.SubscribeServerPort);
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

				SendAsciiText("\r\n");
				WaitSend(5000);

				ScheduleWru();

				SendAsciiText("\r\n");
				WaitSend(5000);

				foreach (string line in fileData)
				{
					int col = 0;
					for (int i=0; i<line.Length; i++)
					{
						if (line[i]==CodeManager.ASC_CR)
						{
							col = 0;
						}
						if (col>68)
						{
							SendAsciiText("\r\n");
							col = 0;
						}
						SendAsciiChar(line[i]);
						col++;
					}
					SendAsciiText("\r\n");
					WaitSend(5000);
				}

				SendAsciiText("\r\n");
				WaitSend(5000);

				//ShowLocalMessage("wait text ok");

				ScheduleWru();

				SendAsciiText("\r\n\r\n\r\n\r\n");
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
			SendAsciiChar(CodeManager.ASC_WRU);
			WaitSend(5000);
			WaitRecv(5000);
			//ShowLocalMessage("wait WRU ok");

			SendAsciiText($"\r\n{AnswerbackTb.Text}\r\n");
			WaitSend(5000);
			//ShowLocalMessage("wait here is ok");
		}

#endregion

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

		private void RecvOnCb_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void TlnTypeTb_MouseHover(object sender, EventArgs e)
		{
			ToolTip tt = new ToolTip();
			tt.IsBalloon = true;
			tt.InitialDelay = 0;
			tt.ShowAlways = true;
			tt.SetToolTip(TlnTypeTb, LngText(LngKeys.MainForm_PeerType));
		}

		private void TextEditorBtn_Click(object sender, EventArgs e)
		{
			try
			{
				TextEditorBtn.Enabled = false;
				SetFocus();
				TextEditorForm editor = new TextEditorForm();
				editor.Show();

			}
			finally
			{
				TextEditorBtn.Enabled = true;
			}

		}

		private void TextEditor_Send(string text)
		{
			SendAsciiText(text);
		}
	}
}
