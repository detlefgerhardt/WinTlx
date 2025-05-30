﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Debugging;
using WinTlx.Favorites;
using WinTlx.Languages;
using WinTlx.Prueftexte;
using WinTlx.Scheduler;
using WinTlx.TapePunch;
using WinTlx.TextEditor;

namespace WinTlx
{
	public partial class MainForm : Form
	{
		private const string TAG = nameof(MainForm);

		private readonly System.Timers.Timer _clockTimer;
		private readonly System.Timers.Timer _focusTimer;

		private readonly int _fixedWidth;

		private readonly SubscriberServer _subscriberServer;
		private readonly ItelexProtocol _itelex;
		private readonly TapePunchManager _tapePunch;
		private readonly SpecialCharacters _specialCharacters = SpecialCharacters.Instance;

		private TapePunchForm _tapePunchForm;
		private TextEditorForm _textEditorForm;
		private FavoritesForm _favoritesForm;
		private readonly ConfigManager _configManager;
		private readonly ConfigData _configData;
		private readonly SchedulerManager _schedulerManager;

		public const int SCREEN_WIDTH = 68;
		//public const int CHAR_HEIGHT = 19;
		public const int CHAR_HEIGHT = 16;
		public const int CHAR_WIDTH = 9;

		private string _screenBuffer = "";

		private int _screenHeight = 25;
		private readonly List<ScreenLine> _screen = new List<ScreenLine>();
		private int _screenX = 0;
		private int _screenY = 0;
		private int _screenEditPos0 = 0;
		private int _screenShowPos0 = 0;

		private readonly BufferManager _bufferManager;

		private DebugManager _debugManager;

		private readonly FavoritesManager _favoritesManager;

		private readonly TextEditorManager _textEditorManager;

		private List<string> _searchHistory;

		private TestPatternForm _testPatternForm;

		private SchedulerForm _schedulerForm;

		private string _currentTlnNumber = null;
		private string _currentTlnName = null;

		private MainStripMenu _mainMenu = new MainStripMenu();
		private MenuStrip _mainMenuStrip;

		private bool _recvOn;

		private bool _answerbackActive;

		private PeerTypeItem[] _tlnTypes;
		
		public MainForm()
		{
			InitializeComponent();

			_fixedWidth = this.Width;

			TlnTypeCb.Enabled = true;

			_specialCharacters.Init(CHAR_WIDTH, CHAR_HEIGHT);

			//string x = "✠";

			// order is import, logging needs Logfile path from config !!!
			_configManager = ConfigManager.Instance;
			_configManager.LoadConfig();
			_configData = _configManager.Config;

			Logging.Instance.LogfilePath = _configData.LogfilePath;

			_textEditorManager = TextEditorManager.Instance;
			_textEditorManager.Disconnect += TextEditorManager_Disconnect;
			_textEditorManager.Dial += TextEditorManager_Dial;

			string logStartStr = $"---------- Start {Helper.GetVersion()} ----------";
			Logging.Instance.Info(TAG, nameof(MainForm), logStartStr);
			CommLog(logStartStr + "\r\n");

			this.Text = Helper.GetVersion();
			this.KeyPreview = true;
			this.Enter += MainForm_Enter;
			this.Deactivate += MainForm_Deactivate;

			TlnNameCb.DataSource = null;
			TlnNameCb.DisplayMember = "DisplayName";

			SendLineFeedBtn.Text = "\u2261";

			//ConnectedLbl.Visible = false;

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

			_recvOn = false;

			_answerbackActive = true;

			_itelex = ItelexProtocol.Instance;
			_itelex.Send += SendHandler;
			//_itelex.Connected += ConnectedHandler;
			_itelex.Dropped += Itelex_DroppedHandler;
			_itelex.Update += Itelex_UpdateHandler;
			//_itelex.Message += MessageHandler;

			_subscriberServer = new SubscriberServer();

			_clockTimer = new System.Timers.Timer(500);
			_clockTimer.Elapsed += ClockTimer_Elapsed;
			_clockTimer.Start();

			_focusTimer = new System.Timers.Timer(100);
			_focusTimer.Elapsed += FocusTimer_Elapsed;
			_focusTimer.Start();

			_debugManager = DebugManager.Instance;

			_bufferManager = BufferManager.Instance;
			_bufferManager.SetLocalOutputSpeed(_configData.OutputSpeed);
			_bufferManager.UpdateSend += BufferManager_UpdateSend;
			_bufferManager.Output += BufferManager_Output;

			_tapePunch = TapePunchManager.Instance;
			_tapePunch.ShowBufferEvt += TapePunch_ShowBufferEvt;

			_schedulerManager = SchedulerManager.Instance;
			_schedulerManager.Schedule += SchedulerManager_Schedule;
			_schedulerManager.LoadScheduler();

			_favoritesManager = FavoritesManager.Instance;
			_favoritesManager.DialFavorite += FavoritesManager_DialFavorite;

			LoadSearchHistory();

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageManager.Instance.ChangeLanguage(_configData.Language);

			UpdateMainMenu();

			Itelex_UpdateHandler();
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

			TlnPeerTypeLbl.Text = LngText(LngKeys.MainForm_PeerType);
			TlnAddressLbl.Text = LngText(LngKeys.MainForm_Address);
			TlnPortLbl.Text = LngText(LngKeys.MainForm_Port);
			TlnExtensionLbl.Text = LngText(LngKeys.MainForm_Extension);
			ConnectBtn.Text = LngText(LngKeys.MainForm_ConnectButton);
			DisconnectBtn.Text = LngText(LngKeys.MainForm_DisconnectButton);

			DeactivateAnswerbackBtn.Text = _answerbackActive ? 
				LngText(LngKeys.MainForm_AnswerbackActiveButton) : 
				LngText(LngKeys.MainForm_AnswerbackDeactiveButton);

			SendWruBtn.Text = LngText(LngKeys.MainForm_SendWruButton);
			SendHereIsBtn.Text = LngText(LngKeys.MainForm_SendHereisButton);
			SendLettersBtn.Text = LngText(LngKeys.MainForm_SendLettersButton);
			SendFiguresBtn.Text = LngText(LngKeys.MainForm_SendFiguresButton);
			SendCarriageReturnBtn.Text = LngText(LngKeys.MainForm_SendReturnButton);
			SendLineFeedBtn.Text = LngText(LngKeys.MainForm_SendLinefeedButton);
			SendBellBtn.Text = LngText(LngKeys.MainForm_SendBellButton);

			LineLbl.Text = LngText(LngKeys.MainForm_LineLabel);
			ColumnLbl.Text = LngText(LngKeys.MainForm_ColumnLabel);

			UpdateMainMenu();
			ShowScreen();
			Itelex_UpdateHandler();
			SetCod32BtnText();
			SetPeerTypes();
		}

		private void SetCod32BtnText()
		{
			Helper.ControlInvokeRequired(SendCod32Btn, () =>
			{
				if (_configData.CodeSet != CodeSets.CYRILL)
				{
					SendCod32Btn.Text = LngText(LngKeys.MainForm_SendCod32Button);
					Helper.SetToolTip(SendCod32Btn, LngText(LngKeys.MainForm_SendCod32Button_ToolTip));
				}
				else
				{
					SendCod32Btn.Text = LngText(LngKeys.MainForm_SendThirdLevelButton);
					Helper.SetToolTip(SendCod32Btn, LngText(LngKeys.MainForm_SendThirdLevelButton_ToolTip));
				}
			});
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		#region form events

		private void MainForm_Load(object sender, EventArgs e)
		{
			MainForm_Resize(null, null);
			SetTlnData();

#if !DEBUG
			/*
			string text = $"{Helper.GetVersion()}\r\r" + "by *dg* Detlef Gerhardt\r\r" + LngText(LngKeys.Start_Text);
			MessageBox.Show(
				text,
				$"{Constants.PROGRAM_NAME}",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button1);
			*/
#endif
		}

		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			TimeTb.Focus();
			TerminalPb.Refresh();
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			SetFocus();
		}

		private void MainForm_Enter(object sender, EventArgs e)
		{
		}

		private void MainForm_Leave(object sender, EventArgs e)
		{
			TerminalPb.Refresh();
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			// prevent width change
			this.Width = _fixedWidth;

			TerminalPb.Height = this.Height - 310 + 95;
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

		#region menu handling

		private void MainMenuHandler(MainStripMenu.MenuTypes menuType)
		{
			switch (menuType)
			{
				case MainStripMenu.MenuTypes.SaveBufferAsText:
					SaveBufferAsText();
					break;
				case MainStripMenu.MenuTypes.SaveBufferAsImage:
					SaveBufferAsImage();
					break;
				case MainStripMenu.MenuTypes.ClearBuffer:
					SetFocus();
					ClearScreen();
					break;
				case MainStripMenu.MenuTypes.Config:
					ShowConfig();
					break;
				case MainStripMenu.MenuTypes.OpenFavorites:
					OpenFavorites();
					break;
				case MainStripMenu.MenuTypes.OpenTextEditor:
					OpenTextEditor();
					break;
				case MainStripMenu.MenuTypes.OpenTapePunchEditor:
					OpenTapePunch();
					break;
				case MainStripMenu.MenuTypes.EyeballCharOnOff:
					EyeballCharOnOff();
					break;
				case MainStripMenu.MenuTypes.TestPattern:
					OpenTestPattern();
					break;
				case MainStripMenu.MenuTypes.OpenScheduler:
					OpenScheduler();
					break;
				case MainStripMenu.MenuTypes.ReceiveOnOff:
					RecvOnOff();
					break;
				case MainStripMenu.MenuTypes.UpdateSubscribeServer:
					SetFocus();
					UpdateIpAddress();
					break;
				case MainStripMenu.MenuTypes.OpenDebugForm:
					DebugManager.Instance.OpenDebugForm(this.Bounds);
					break;
				case MainStripMenu.MenuTypes.About:
					ShowAbout();
					break;
				case MainStripMenu.MenuTypes.Exit:
					Close();
					break;
			}
		}

		private void UpdateMainMenu()
		{
			if (_mainMenuStrip != null) Controls.Remove(_mainMenuStrip);
			_mainMenuStrip = _mainMenu.GetMenu(MainMenuHandler);
			Controls.Add(_mainMenuStrip);
		}

		private ContextMenuStrip CreateContextMenu()
		{
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			List<ToolStripMenuItem> endItems = new List<ToolStripMenuItem>
			{
				new ToolStripMenuItem("Clear"),
				new ToolStripMenuItem("Copy"),
				new ToolStripMenuItem("Paste")
			};
			contextMenu.ItemClicked += ConnectMenu_ItemClicked;
			return contextMenu;
		}

		private void ConnectMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			// hide context menu
			ContextMenuStrip cms = (ContextMenuStrip)sender;
			cms.Hide();

			// get menu item
			//ToolStripItem tsi = e.ClickedItem;
		}


		#endregion menu handling

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
			//if (SearchCb.Focused || MemberCb.Focused || AddressTb.Focused || PortTb.Focused || ExtensionTb.Focused)
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
				case Keys.C | Keys.Alt:
					SendAsciiChar(CodeManager.ASC_CODE32);
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
			if (SearchCb.Focused || TlnNameCb.Focused || TlnAddressTb.Focused || TlnPortTb.Focused || TlnExtensionTb.Focused)
			{
				return;
			}

			if (!TerminalPb.Focused)
			{
				SetFocus();
			}

			char? chr = e.KeyChar;

			if (chr.HasValue && _configData.CodeSet == CodeSets.CYRILL)
			{
				if (_itelex.ShiftState == ShiftStates.Third || "äöüÄÖ".Contains(chr.Value.ToString()))
				{
					chr = CodeTabCyrill.CyrillicKeyToUnicode(chr.Value);
				}
				else if (chr >= 'A' && chr <= 'Z')
				{
					chr = CodeTabCyrill.CyrillicKeyToUnicode(char.ToLower(chr.Value));
				}
			}

			chr = CodeManager.KeyboardCharacters(chr);
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

			Helper.ControlInvokeRequired(ConnTimeLbl, () =>
			{
				ConnTimeLbl.Text = $"{LngText(LngKeys.MainForm_ConnTimeLabel)}: {_itelex.ConnTimeMinSek}";
			});

			Helper.ControlInvokeRequired(TimeoutLbl, () =>
			{
				string valStr;
				TimeoutLbl.ForeColor = Color.Black;
				if (!_itelex.IsConnected || _configData.IdleTimeout == 0)
				{
					valStr = "-";
				}
				else
				{
					int timeout = _configData.IdleTimeout - _itelex.IdleTimerMs / 1000;
					valStr = $"{timeout} s";
					if (timeout < 10)
					{
						ConnTimeLbl.ForeColor = Color.Red;
					}
				}
				TimeoutLbl.Text = $"{LngText(LngKeys.MainForm_IdleTimeLabel)}: {valStr}";

				// trigger change of forecolor
				//IdleTimoutTb.BackColor = IdleTimoutTb.BackColor;
			});

			if (_itelex.IsConnected && _configData.IdleTimeout>0 && IdleTimeout()==0)
			{
				_bufferManager.LocalOutputMessage(LngText(LngKeys.Message_IdleTimeout), false);
				_itelex.Disconnect();
			}
		}

		private int IdleTimeout()
		{
			if (!_itelex.IsConnected)
			{
				return -1;
			}
			//Debug.WriteLine($"{_configData.IdleTimeout} {_itelex.IdleTimerMs}");
			int timeout = _configData.IdleTimeout - _itelex.IdleTimerMs / 1000;
			if (timeout < 0)
			{
				timeout = 0;
			}
			return timeout;
		}

		private void FocusTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			//bool focus = !SearchCb.Focused && !MemberCb.Focused && !AddressTb.Focused && !PortTb.Focused && ExtensionTb.Focused;
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

		private void TlnNameCb_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetTlnData();
		}

		private void SetTlnData()
		{
			PeerQueryData entry = (PeerQueryData)TlnNameCb.SelectedItem;
			if (entry == null)
			{
				TlnNameCb.Text = "";
				TlnAddressTb.Text = "";
				TlnPortTb.Text = "";
				TlnExtensionTb.Text = "";
				TlnTypeCb.Text = "";
				return;
			}

			_currentTlnNumber = entry.Number;
			_currentTlnName = entry.LongName;
			TlnAddressTb.Text = entry.Address;
			TlnPortTb.Text = entry.PortNumber != 0 ? entry.PortNumber.ToString() : "";
			TlnExtensionTb.Text = entry.ExtensionNumber != 0 ? entry.ExtensionNumber.ToString() : "";

			TlnTypeCb.SelectedIndex = 0;
			TlnTypeCb.Refresh();
			TlnTypeCb.SelectedIndex = entry.PeerType;
			TlnTypeCb.Refresh();
			//TlnTypeCb.Text = entry.PeerType.ToString();
		}

		private void AddressTb_Leave(object sender, EventArgs e)
		{
			//SetConnectState();
			Itelex_UpdateHandler();
		}

		private void PortTb_Leave(object sender, EventArgs e)
		{
			//SetConnectState();
			Itelex_UpdateHandler();
		}

		private async void SearchCb_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
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

			SearchCb.Enabled = false;
			QueryBtn.Enabled = false;

			_queryActive = true;
			await Query();
			_queryActive = false;

			SearchCb.Enabled = true;
			QueryBtn.Enabled = true;
		}

		private async Task Query()
		{
			SetFocus();

			TlnNameCb.DataSource = null;
			SetTlnData();
			/*
			TlnNameCb.Text = "";
			TlnAddressTb.Text = "";
			TlnPortTb.Text = "";
			TlnExtensionTb.Text = "";
			TlnTypeCb.Text = "";
			*/

			SearchCb.Text = SearchCb.Text.Trim();
			if (string.IsNullOrWhiteSpace(SearchCb.Text)) return;

			string searchStr = SearchCb.Text;

			AddToSearchHistory(searchStr);

			Logging.Instance.Log(LogTypes.Info, TAG, nameof(QueryBtn_Click), $"SearchText='{searchStr}'");

			if (!_configData.SubscribeServerAddressExists || _configData.SubscribeServerPort == 0)
			{
				_bufferManager.LocalOutputMessage(LngText(LngKeys.Message_SubscribeServerError), false);
				Logging.Instance.Error(TAG, "QueryBtn_Click", "no valid subscribe server data");
				return;
			}

			PeerQueryData[] list = null;

			if (!uint.TryParse(searchStr, out uint num))
			{
				num = 0;
			}

			await Task.Run(() =>
			{
				if (num > 0)
				{
					PeerQueryReply queryReply = _subscriberServer.DoPeerQuery(
						_configData.SubscribeServerAddresses, _configData.SubscribeServerPort, num.ToString());
					if (!queryReply.Valid)
					{
						_bufferManager.LocalOutputMessage(queryReply.Error, true);
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
					PeerSearchReply searchReply = _subscriberServer.DoPeerSearch(
						_configData.SubscribeServerAddresses, _configData.SubscribeServerPort, searchStr);
					if (!searchReply.Valid)
					{
						_bufferManager.LocalOutputMessage(searchReply.Error, true);
						return;
					}
					list = searchReply.List;
				}
			});

			int count = list != null ? list.Length : 0;
			_bufferManager.LocalOutputMessage($"{count} {LngText(LngKeys.Message_QueryResult)}", false);

			TlnNameCb.DataSource = list;
			TlnNameCb.DisplayMember = "Display";
			//if (list.Length>0) TlnNameCb.SelectedIndex = 0;

			/*
			if (list == null)
			{
			}
			else if (list.Length == 0)
			{
				TlnNameCb.Text = "";
				TlnAddressTb.Text = "";
				TlnPortTb.Text = "";
				TlnExtensionTb.Text = "";
				TlnTypeCb.Text = "";
			}
			else
			{
				TlnNameCb.SelectedIndex = 0;
			}
			*/
			SetTlnData();
			Itelex_UpdateHandler();
		}

		private async void ConnectBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			if (_itelex.IsConnected) return;

			ConnectBtn.Enabled = false;
			_bufferManager.LocalOutputBufferClear(true);
			await ConnectOut();
			//ConnectBtn.Enabled = true;
			//SetConnectState();
			Itelex_UpdateHandler();
		}

		private void DisconnectBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			Disconnect();
			_bufferManager.LocalOutputBufferClear(false);
		}

		private void DeactivateAnswerbackBtn_Click(object sender, EventArgs e)
		{
			if (_answerbackActive)
			{
				_answerbackActive = false;
				DeactivateAnswerbackBtn.ForeColor = Color.Red;
				DeactivateAnswerbackBtn.Text = LngText(LngKeys.MainForm_AnswerbackDeactiveButton);
			}
			else
			{
				_answerbackActive = true;
				DeactivateAnswerbackBtn.ForeColor = Color.Black;
				DeactivateAnswerbackBtn.Text = LngText(LngKeys.MainForm_AnswerbackActiveButton);
			}
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

		private void SendFBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendFigFGH(CodeManager.ASC_SHIFTF);
		}

		private void SendGBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendFigFGH(CodeManager.ASC_SHIFTG);
		}

		private void SendHBtn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendFigFGH(CodeManager.ASC_SHIFTH);
		}

		private void SendFigFGH(char chr)
		{
			if (_configData.CodeSet == CodeSets.CYRILL)
			{
				int code = 0;
				switch(chr)
				{
					case CodeManager.ASC_SHIFTF:
						code = 0x16;
						break;
					case CodeManager.ASC_SHIFTG:
						code = 0x0B;
						break;
					case CodeManager.ASC_SHIFTH:
						code = 0x05;
						break;
				}
				ICodeTab tab = new CodeTabCyrill();
				SendAsciiChar(tab.CodeTab[code].Char3rd);
			}
			else
			{
				SendAsciiChar(chr);
			}
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

		private void OpenTestPattern()
		{
			if (_testPatternForm == null)
			{
				_testPatternForm = new TestPatternForm(this.Bounds);
				_testPatternForm.Show();
				_testPatternForm.ClosedEvent += TestPatternForm_ClosedEvent;
				_testPatternForm.SendEvent += TestPatternForm_SendEvent;
			}
			else
			{
				_testPatternForm.BringToFront();
			}
		}

		private void TestPatternForm_ClosedEvent()
		{
			_testPatternForm.ClosedEvent -= TestPatternForm_ClosedEvent;
			_testPatternForm.SendEvent -= TestPatternForm_SendEvent;
			_testPatternForm = null;
		}

		private void TestPatternForm_SendEvent(string asciiText)
		{
			SendAsciiText(asciiText);
		}

		private void SendCod32Btn_Click(object sender, EventArgs e)
		{
			SetFocus();
			SendAsciiChar(CodeManager.ASC_CODE32);
		}

		private void ClearBtn_Click(object sender, EventArgs e)
		{
			ClearScreen();
			SetFocus();
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
				if (_screenShowPos0 < 0)
				{
					_screenShowPos0 = 0;
				}
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

		private void OpenScheduler()
		{
			SetFocus();
			if (_schedulerForm == null)
			{
				_schedulerForm = new SchedulerForm(this.Bounds);
				_schedulerForm.ClosedEvent += SchedulerForm_ClosedEvent;
				_schedulerForm.ShowDialog();
			}
			else
			{
				_schedulerForm.BringToFront();
			}
			return;
		}

		private void SchedulerForm_ClosedEvent()
		{
			_schedulerForm.ClosedEvent -= SchedulerForm_ClosedEvent;
			_schedulerForm = null;
		}

		private async void RecvOnOff()
		{
			if (!_recvOn)
			{
				if (!_configData.LimitedClient)
				{
					_itelex.SetRecvOn(_configData.IncomingLocalPort);
					UpdateIpAddress();
					_recvOn = true;
					_mainMenu.SetChecked(MainStripMenu.MenuTypes.ReceiveOnOff, true);
				}
				else
				{
					if (await _itelex.CentralexConnectAsync(_configData.RemoteServerAddress, _configData.RemoteServerPort))
					{
						_recvOn = true;
						_mainMenu.SetChecked(MainStripMenu.MenuTypes.ReceiveOnOff, true);
						SubscriberServer subSrv = new SubscriberServer();
						PeerQueryReply reply = subSrv.DoPeerQuery(_configData.SubscribeServerAddresses, _configData.SubscribeServerPort,
							_configData.OwnNumber.ToString());
						if (reply.Valid)
						{
							_bufferManager.LocalOutputMessage($"server-data: {reply.Data.Address}:{reply.Data?.PortNumber} extention={reply.Data.ExtensionNumber}", true);
						}
						else
						{
							_bufferManager.LocalOutputMessage($"DoPeerQuery: {reply.Error}", true);
						}
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
					_itelex.CentralexDisconnect();
					_bufferManager.LocalOutputMessage("centralex end received", true);
				}
				_recvOn = false;
				_mainMenu.SetChecked(MainStripMenu.MenuTypes.ReceiveOnOff, false);
			}
			Itelex_UpdateHandler();
		}

		private void UpdateIpAddress()
		{
			//if (_configData.SubscribeServerUpdatePin == 0 || _configData.OwnNumber == 0 || _configData.IncomingLocalPort == 0) return;
			if (_configData.OwnNumber == 0 || _configData.IncomingLocalPort == 0) return;
			SendClientUpdate(_configData.OwnNumber, _configData.SubscribeServerUpdatePin, _configData.IncomingPublicPort);
		}

		private void OpenTapePunch()
		{
			if (_tapePunchForm == null)
			{
				_tapePunchForm = new TapePunchForm(this.Bounds);
				_tapePunchForm.ClosedEvt += TapePunchForm_Closed;
				_tapePunchForm.ClickEvt += TapePunchForm_Click;
				_tapePunchForm.Show();
			}
			else
			{
				_tapePunchForm.BringToFront();
			}
			SetFocus();
		}

		private void TapePunchForm_Click()
		{
			//throw new NotImplementedException();
		}

		private void TapePunchForm_Closed()
		{
			_tapePunchForm.ClosedEvt -= TapePunchForm_Closed;
			_tapePunchForm.ClickEvt -= TapePunchForm_Click;
			_tapePunchForm = null;
		}

		private void EyeballCharOnOff()
		{
			if (!_itelex.EyeballCharActive)
			{
				_bufferManager.LocalOutputMessage($"{LngText(LngKeys.Message_EyeballCharActive)}", false);
				_itelex.EyeballCharActive = true;
				_mainMenu.SetChecked(MainStripMenu.MenuTypes.EyeballCharOnOff, true);
			}
			else
			{
				_itelex.EyeballCharActive = false;
				_mainMenu.SetChecked(MainStripMenu.MenuTypes.EyeballCharOnOff, false);
			}
			SetFocus();
		}

		private void ShowConfig()
		{
			SetFocus();
			ConfigForm configForm = new ConfigForm(this.Bounds);
			configForm.ShowDialog();
			if (!configForm.Canceled)
			{
				ConfigManager.Instance.SaveConfig();
				_bufferManager.SetLocalOutputSpeed(_configData.OutputSpeed);
				Logging.Instance.LogfilePath = _configData.LogfilePath;
				Itelex_UpdateHandler();
			}
		}

		private void ShowAbout()
		{
			SetFocus();
			string text;
			if (_configData.Language == "de")
			{
				text =
					$"{Helper.GetVersion()}\r\r" +
					"von *dg* Detlef Gerhardt\r\r" +
					"Feedback bitte an\r" +
					"feedback@dgerhardt.de oder i-telex 7822222\r\r" +
					"Da WinTlx als Diagnose-Tool gedacht ist, werden dafür keine Telex-Nummern im i-Telex-Netzwerk vergeben.\r" +
					"Die Teilnahme am i-Telex-Netzwerk erfordert mindestens einen realen Fernschreiber. Eine Teilnahme nur mit WinTlx ist nicht möglich.\r" +
					"Bitte rufe mit diesem Tool keine i-Telex-Nummern an, wenn du selbst kein Mitglied im i-Telex-Netzwerk bist!";
			}
			else
			{
				text =
					$"{Helper.GetVersion()}\r\r" +
					"by *dg* Detlef Gerhardt\r\r" +
					"Send feedback to\r" +
					"feedback@dgerhardt.de or i-telex 7822222\r\r" +
					"Since WinTlx is intended exclusively as a diagnostic tool, no i-Telex number is assigned for it in the i-Telex network.\r" +
					"Participation in the i-Telex network requires at least one real teleprinter. Participation only with WinTlx is not possible.\r" +
					"Please do not use this tool to call i-Telex numbers if you are not a member of the i-Telex network!";
			}
			MessageBox.Show(
				text,
				$"About {Constants.PROGRAM_NAME}",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button1);
		}

		private void SendHandler(string asciiText)
		{
			string dispText = "";
			for (int i = 0; i < asciiText.Length; i++)
			{
				char c = asciiText[i];
				switch (c)
				{
					case CodeManager.ASC_CODE32:
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
			_bufferManager.LocalOutputSend(dispText);
		}

		private void Itelex_DroppedHandler(string rejectReason)
		{
			_bufferManager.LocalOutputBufferClear(false);
			_bufferManager.LocalOutputMessage(LngText(LngKeys.Message_Disconnected), false);
			Itelex_UpdateHandler();
		}

		private void Itelex_UpdateHandler()
		{
			SetCod32BtnText();

			_itelex.OwnVersion = _configData.OptionVersion;

			Helper.ControlInvokeRequired(ConnectBtn, () =>
			{
				ConnectBtn.Enabled = !_itelex.IsConnected;
			});

			Helper.ControlInvokeRequired(DisconnectBtn, () =>
			{
				DisconnectBtn.Enabled = _itelex.IsConnected;
			});

			Helper.ControlInvokeRequired(ConnectedLbl, () =>
			{
				if (!_itelex.IsConnected)
				{
					ConnectedLbl.Visible = false;
					//ConnectedLbl.ForeColor = Color.Green;
				}
				else
				{
					ConnectedLbl.Visible = true;
					//ConnectedLbl.ForeColor = Color.Black;
				}
			});

			Helper.ControlInvokeRequired(ConnectStatusLbl, () =>
			{
				if (!_itelex.IsConnected)
				{
					ConnectStatusLbl.Text = "Offline";
					ConnectStatusLbl.ForeColor = Color.Black;
				}
				else
				{
					ConnectStatusLbl.Text = _itelex.ConnectionStateString;
					ConnectStatusLbl.ForeColor = Color.Green;
				}
			});

			Helper.ControlInvokeRequired(SendBufferStatusLbl, () =>
			{
				string valueStr = _itelex.IsConnected ? $"{_itelex.SendBufferCount + _bufferManager.SendBufferCount}" : "-";
				SendBufferStatusLbl.Text = $"{LngText(LngKeys.MainForm_SendBufferStatus)}: {valueStr}";
			});

			Helper.ControlInvokeRequired(RemoteBufferStatusLbl, () =>
			{
				string valueStr = _itelex.IsConnected ? $"{_itelex.SendBufferCount}" : "-";
				RemoteBufferStatusLbl.Text = $"{LngText(LngKeys.MainForm_RemoteBufferStatus)}: {valueStr}";
			});

			Helper.ControlInvokeRequired(LocalBufferStatusLbl, () =>
			{
				//string valueStr = _itelex.IsConnected ? $"{_bufferManager.LocalOutputBufferCount}" : "-";
				string valueStr = $"{_bufferManager.LocalOutputBufferCount}";
				LocalBufferStatusLbl.Text = $"{LngText(LngKeys.MainForm_LocalBufferStatus)}: {valueStr}";
			});

			Helper.ControlInvokeRequired(RecvActiveLbl, () =>
			{
				if (_recvOn)
				{
					RecvActiveLbl.Text = _configData.LimitedClient ? LngText(LngKeys.MainForm_CentralexActive) :
							LngText(LngKeys.MainForm_RecvActive);
					RecvActiveLbl.Visible = true;
					RecvActiveLbl.Enabled = true;
				}
				else
				{
					RecvActiveLbl.Text = LngText(LngKeys.MainForm_RecvInactive);
					RecvActiveLbl.Enabled = false;
				}
			});

			/*
			Helper.ControlInvokeRequired(ReceiveStatusLbl, () =>
			{
				ReceiveStatusLbl.Text = _recvOn ? LngText(LngKeys.MainForm_ReceiveStatusOn) : LngText(LngKeys.MainForm_ReceiveStatusOff);
				ReceiveStatusLbl.ForeColor = _recvOn ? Color.Green : Color.Black;
			});
			*/

			Helper.ControlInvokeRequired(CharSetLbl, () =>
			{
				CharSetLbl.Text = _configData.CodeSet.ToString();
			});

			Helper.ControlInvokeRequired(AnswerbackTb, () => AnswerbackTb.Text = _configData.AnswerbackWinTlx);

			Helper.ControlInvokeRequired(SendLettersBtn, () =>
			{
				if (_itelex.ShiftState != ShiftStates.Ltr)
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
				if (_itelex.ShiftState != ShiftStates.Figs)
				{
					SendFiguresBtn.ForeColor = Color.Black;
				}
				else
				{
					SendFiguresBtn.ForeColor = Color.Green;
				}
			});

			Helper.ControlInvokeRequired(SendCod32Btn, () =>
			{
				if (_itelex.ShiftState != ShiftStates.Third)
				{
					SendCod32Btn.ForeColor = Color.Black;
				}
				else
				{
					SendCod32Btn.ForeColor = Color.Green;
				}
			});

		}

		private void BufferManager_UpdateSend()
		{
			Itelex_UpdateHandler();
		}

		private async Task<bool> ConnectOut()
		{
			TlnAddressTb.Text = TlnAddressTb.Text.Trim();
			TlnPortTb.Text = TlnPortTb.Text.Trim();
			TlnExtensionTb.Text = TlnExtensionTb.Text.Trim();

			Logging.Instance.Info(TAG,
				nameof(ConnectOut), $"ip or host address={TlnAddressTb.Text},port number={TlnPortTb.Text},extension number={TlnExtensionTb.Text},peer type={TlnTypeCb.Text}");

			if (string.IsNullOrWhiteSpace(TlnAddressTb.Text))
			{
				_bufferManager.LocalOutputMessage(
					LngText(LngKeys.Message_InvalidConnectionData),
					$"connection error, invalid ip address {TlnAddressTb.Text}",
					true, true);
				return false;
			}

			int? port = Helper.ToInt(TlnPortTb.Text);
			if (port == null)
			{
				_bufferManager.LocalOutputMessage(
					LngText(LngKeys.Message_InvalidConnectionData),
					$"invalid port number {TlnPortTb.Text}",
					true, true);
				return false;
			}

			int? extension;
			if (!string.IsNullOrWhiteSpace(TlnExtensionTb.Text))
			{
				extension = Helper.ToInt(TlnExtensionTb.Text);
				if (extension == null || extension < 0 || extension > 255)
				{
					_bufferManager.LocalOutputMessage(
						LngText(LngKeys.Message_InvalidConnectionData),
						$"invalid extension {TlnExtensionTb.Text}",
						true, true);
					return false;
				}
			}
			else
			{
				extension = null;
			}

			PeerTypeItem peerTypeItem = (PeerTypeItem)TlnTypeCb.SelectedItem;
			bool asciiMode = false;
			if (peerTypeItem != null)
			{
				switch(peerTypeItem.PeerCode)
				{
					case 1:
					case 2:
					case 5:
						asciiMode = false;
						break;
					case 3:
					case 4:
						asciiMode = true;
						break;
					default:
						_bufferManager.LocalOutputMessage(
							LngText(LngKeys.Message_InvalidConnectionData),
							$"invalid peer-type {peerTypeItem.PeerCode}",
							true, true);
						return false;
				}
			}

			bool success = await _itelex.ConnectOut(TlnAddressTb.Text, port.Value, extension, asciiMode);
			if (success)
			{
				_favoritesManager.CallHistoryAddCall(_currentTlnNumber, _currentTlnName, "ok");
			}
			else
			{
				_favoritesManager.CallHistoryAddCall(_currentTlnNumber, _currentTlnName, _itelex.RejectReason);
				return false;
			}

			return true;
		}

		private void Disconnect()
		{
			if (_itelex != null)
			{
				_itelex.SendEndCmd();
				Thread.Sleep(2000);
				_itelex.Disconnect();
				_bufferManager.LocalOutputBufferClear(false);
			}
		}

		private void SendBel()
		{
			SendAsciiChar(CodeManager.ASC_BEL);
		}

		private void SendWhoAreYou()
		{
			SendAsciiChar(CodeManager.ASC_FIGS);
			SendAsciiChar(CodeManager.ASC_WRU);
		}

		private void SendHereIs()
		{
			string answerBack = _configData.AnswerbackWinTlx;
			answerBack = answerBack.Replace(@"\r", "\r");
			answerBack = answerBack.Replace(@"\n", "\n");
			SendAsciiText($"{answerBack}");
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
			_bufferManager.SendBufferEnqueueChr(chr);
			Itelex_UpdateHandler();
		}

		private void TapePunch_ShowBufferEvt(string asciiText)
		{
			ClearScreen();
			if (_configData.UpperCaseChar) asciiText = asciiText.ToUpper();
			OutputText(asciiText, CharAttributes.Recv);
			ShowScreen();
		}

		private void ClearScreen()
		{
			_bufferManager.LocalOutputBufferClear(true);
			_screen.Clear();
			_screenX = 0;
			_screenY = 0;
			_screenEditPos0 = 0;
			_screenShowPos0 = 0;
			_screenBuffer = "";
			ShowScreen();
		}

		private void BufferManager_Output(ScreenChar screenChar)
		{
			if (screenChar == null) return;

			OutputText(screenChar.Char.ToString(), screenChar.Attr);

			switch (screenChar.Char)
			{
				case CodeManager.ASC_BEL:
					SystemSounds.Beep.Play();
					break;
				case CodeManager.ASC_WRU:
					if (_answerbackActive)
					{
						if (screenChar.Attr == CharAttributes.Recv) SendHereIs();
					}
					break;
			}
		}

		private void OutputText(string asciiText, CharAttributes attr)
		{
			if (string.IsNullOrEmpty(asciiText)) return;

			for (int i = 0; i < asciiText.Length; i++)
			{
				_bufferManager.UpdateLastLocalOutputChars(asciiText[i]);

				switch (asciiText[i])
				{
					case '\n':
						IncScreenY();
						_screenBuffer += "\n";
						break;
					case '\r':
						_screenX = 0;
						_screenBuffer += "\r";
						break;
					case '\x0': // code32
						break;
					default:
						if (_screenY + _screenEditPos0 >= _screen.Count)
						{
							_screen.Add(new ScreenLine());
						}
						if (_itelex.ShiftState != ShiftStates.Third)
						{
							_screen[_screenEditPos0 + _screenY].Line[_screenX].Char = asciiText[i];
						}
						else
						{
							_screen[_screenEditPos0 + _screenY].Line[_screenX].Char = asciiText[i];
							//_screen[_screenEditPos0 + _screenY].Line[_screenX].Char = CodeManager.CyrillicKeyToUnicode(asciiText[i]) ?? '.';
						}
						_screen[_screenEditPos0 + _screenY].Line[_screenX].Attr = attr;
						_screenBuffer += asciiText[i];
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

			Helper.ControlInvokeRequired(LineLbl, () =>
			{
				LineLbl.Text = $"{LngText(LngKeys.MainForm_LineLabel)}: {_screenEditPos0 + _screenY + 1:D3}";
				ColumnLbl.Text = $"{LngText(LngKeys.MainForm_ColumnLabel)}: {_screenX + 1:D2}";
			});
		}

		private void SetFocus()
		{
			TerminalPb.Focus();
			TerminalPb.Refresh();
		}

		private readonly object _commLogLock = new object();

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
				string text =  Clipboard.GetText();
				SendAsciiText(text);
			}
		}

		//private void MessageHandler(string asciiText)
		//{
		//	ShowLocalMessage(asciiText);
		//}

		private void MainForm_LocationChanged(object sender, EventArgs e)
		{
			_tapePunchForm?.SetPosition(this.Bounds);
		}

		private void SendClientUpdate(int number, int pin, int publicPort)
		{
			ClientUpdateReply reply = _subscriberServer.DoClientUpdate(
				_configData.SubscribeServerAddresses, _configData.SubscribeServerPort, number, pin, publicPort);
			if (reply.Success)
			{
				_bufferManager.LocalOutputMessage($"update {number} ok / {reply.IpAddress}:{publicPort}", true);
			}
			else
			{
				_bufferManager.LocalOutputMessage($"update {number} {reply.Error}", true);
			}
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
				//char chr = scrChr.Char;
				if (chr != ' ' && chr != 0x00)
				{
					Point p = new Point(x * CHAR_WIDTH, y * CHAR_HEIGHT);
					Bitmap chrBmp;
					switch (chr)
					{
						case CodeManager.ASC_BEL:
							//g.DrawString("⍾", font, new SolidBrush(scrChr.AttrColor), p);
							chrBmp = _specialCharacters.GetSpecialChrBmp(chr, scrChr.AttrColor);
							g.DrawImage(chrBmp, x * CHAR_WIDTH + 3, y * CHAR_HEIGHT + 3, CHAR_WIDTH, CHAR_HEIGHT);
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
			Task.Run(async () =>
			{
				SchedulerItem scheduleItem = args.Item;
				_bufferManager.LocalOutputMessage($"schedule {scheduleItem.Destination}", true);
				Logging.Instance.Warn(TAG, nameof(SchedulerManager_Schedule), $"Schedule {scheduleItem}");
				bool success = await DoSchedule(scheduleItem);
				if (!success)
				{
					Logging.Instance.Warn(TAG, nameof(SchedulerManager_Schedule), $"Schedule failed {scheduleItem}");
					_bufferManager.LocalOutputMessage($"schedule failed {scheduleItem.Destination}", true);
				}
			});
		}

		private async Task<bool> DoSchedule(SchedulerItem scheduleItem)
		{
			if (_itelex.IsConnected)
			{
				// if connected do nothing
				return true;
			}

			// get destination address

			if (scheduleItem.DestAddress == null)
			{
				// query number
				if (string.IsNullOrWhiteSpace(scheduleItem.Destination))
				{
					// invalid destination
					Logging.Instance.Warn(TAG, nameof(DoSchedule), $"Invalid destination {scheduleItem.Destination}");
					scheduleItem.Error = true;
					return false;
				}

				string dest = scheduleItem.Destination.Replace(" ", "");
				if (!uint.TryParse(dest, out uint num))
				{
					Logging.Instance.Warn(TAG, nameof(DoSchedule), $"Invalid destination number {dest}");
					scheduleItem.Error = true;
					return false;
				}

				PeerQueryReply queryReply = _subscriberServer.DoPeerQuery(_configData.SubscribeServerAddresses, _configData.SubscribeServerPort, dest);
				if (!queryReply.Valid || queryReply.Data == null)
				{
					Logging.Instance.Warn(TAG, nameof(DoSchedule), $"Subscribe server query failed");
					_bufferManager.LocalOutputMessage(queryReply.Error, true);
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

			_bufferManager.LocalOutputMessage($"schedule send to {scheduleItem.DestAddress}:{scheduleItem.DestPort} {scheduleItem.DestExtension}", true);
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
				//WaitRecv(5000);
				await _bufferManager.WaitSendBufferEmptyAsync();
				//ShowLocalMessage("wait greeting ok");

				SendAsciiText("\r\n");
				//WaitSend(5000);
				await _bufferManager.WaitSendBufferEmptyAsync();

				await ScheduleWru();

				SendAsciiText("\r\n");
				//WaitSend(5000);
				await _bufferManager.WaitSendBufferEmptyAsync();

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
					//WaitSend(5000);
					await _bufferManager.WaitSendBufferEmptyAsync();
				}

				SendAsciiText("\r\n");
				//WaitSend(5000);
				await _bufferManager.WaitSendBufferEmptyAsync();

				//ShowLocalMessage("wait text ok");

				await ScheduleWru();

				SendAsciiText("\r\n\r\n\r\n\r\n");
				//WaitSend(5000);
				await _bufferManager.WaitSendBufferEmptyAsync();

				Logging.Instance.Debug(TAG, nameof(DoSchedule), $"Success");
				scheduleItem.Success = true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(DoSchedule), $"Error sending text", ex);
				_bufferManager.LocalOutputMessage("recv timeout", true);
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

		private async Task ScheduleWru()
		{
			SendAsciiChar(CodeManager.ASC_WRU);
			await _bufferManager.WaitSendBufferEmptyAsync();
			await _bufferManager.WaitLocalOutpuBufferEmpty();
			//WaitSend(5000);
			//WaitRecv(5000);
			//ShowLocalMessage("wait WRU ok");

			SendAsciiText($"\r\n{AnswerbackTb.Text}\r\n");
			await _bufferManager.WaitSendBufferEmptyAsync();
			//WaitSend(5000);
			//ShowLocalMessage("wait here is ok");
		}

		#endregion

		private void TlnTypeCb_MouseHover(object sender, EventArgs e)
		{
			ToolTip tt = new ToolTip
			{
				IsBalloon = true,
				InitialDelay = 0,
				ShowAlways = true
			};
			tt.SetToolTip(TlnTypeCb, LngText(LngKeys.MainForm_PeerTypeHelp));
		}

		private void OpenFavorites()
		{
			SetFocus();
			if (_favoritesForm == null)
			{
				_favoritesForm = new FavoritesForm(this.Bounds);
				_favoritesForm.CloseEditor += FavoritesForm_CloseEditor;
				_favoritesForm.Show();
			}
			else
			{
				_favoritesForm.BringToFront();
			}
		}

		private void FavoritesForm_CloseEditor()
		{
			_favoritesForm.CloseEditor -= FavoritesForm_CloseEditor;
			_favoritesForm = null;
		}

		private void OpenTextEditor()
		{
			SetFocus();
			if (_textEditorForm == null)
			{
				_textEditorForm = new TextEditorForm(this.Bounds);
				_textEditorForm.CloseEditor += TextEditorForm_CloseEditor;
				_textEditorForm.Show();
			}
			else
			{
				_textEditorForm.BringToFront();
			}
		}

		private void TextEditorForm_CloseEditor()
		{
			_textEditorForm.CloseEditor -= TextEditorForm_CloseEditor;
			_textEditorForm = null;
		}

		private void TextEditorManager_Disconnect()
		{
			Disconnect();
		}

		private void SaveBufferAsText()
		{
			if (_screen.Count == 0) return;

			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true,
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = saveFileDialog.FileName;
				try
				{
					File.WriteAllText(filePath, _screenBuffer);
				}
				catch (Exception)
				{
				}
			}
		}

		private void SaveBufferAsImage()
		{
			if (_screen.Count == 0) return;

			Bitmap image = new Bitmap(CHAR_WIDTH * 68, CHAR_HEIGHT * _screen.Count);
			Graphics g = Graphics.FromImage(image);

			Font font = new Font("Consolas", 12);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
			g.Clear(Color.White);

			for (int y = 0; y < _screen.Count; y++)
			{
				for (int x = 0; x < SCREEN_WIDTH + 1; x++)
				{
					ScreenChar scrChr = _screen[y].Line[x];
					TerminalDrawChar(g, font, x, y, scrChr);
				}
			}

			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "png files (*.png)|*.png|All files (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true,
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = saveFileDialog.FileName;
				try
				{
					image.Save(filePath, ImageFormat.Png);
				}
				catch (Exception)
				{
				}
			}
		}

		private void TextEditorManager_Dial(string number)
		{
			_currentTlnNumber = number;
			_currentTlnName = "";
			Task.Run(async () =>
			{
				PeerQueryReply queryReply = _subscriberServer.DoPeerQuery(
					_configData.SubscribeServerAddresses, _configData.SubscribeServerPort, number);
				if (!queryReply.Valid)
				{
					_bufferManager.LocalOutputMessage(queryReply.Error, true);
					return;
				}

				int? extNum = (queryReply.Data.ExtensionNumber == 0) ? null : (int?)queryReply.Data.ExtensionNumber;
				bool success = await _itelex.ConnectOut(queryReply.Data.Address, queryReply.Data.PortNumber, extNum, false);
				if (!success)
				{
					_bufferManager.LocalOutputMessage(LngText(LngKeys.Message_ConnectionError), false);
				}
			});
			Itelex_UpdateHandler();
		}

		private void FavoritesManager_DialFavorite(FavoriteItem favItem)
		{
			_currentTlnNumber = favItem.Number;
			_currentTlnName = favItem.Name;

			Task.Run(() =>
			{
				if (string.IsNullOrEmpty(favItem.Address))
				{
					PeerQueryReply queryReply = _subscriberServer.DoPeerQuery(
						_configData.SubscribeServerAddresses, _configData.SubscribeServerPort, favItem.Number);
					if (!queryReply.Valid || queryReply.Data == null)
					{
						_bufferManager.LocalOutputMessage(queryReply.Error, true);
						return;
					}
					favItem = new FavoriteItem()
					{
						Number = favItem.Number,
						Name = queryReply.Data.LongName,
						Address = queryReply.Data.Address,
						Port = queryReply.Data.PortNumber,
						DirectDial = queryReply.Data.ExtensionNumber
					};
				}

				Helper.ControlInvokeRequired(SearchCb, () => SearchCb.Text = favItem.Number);
				if (!string.IsNullOrEmpty(favItem.Address))
				{
					Helper.ControlInvokeRequired(TlnNameCb, () => TlnNameCb.Text = favItem.Name);
				}
				else
				{
					Helper.ControlInvokeRequired(TlnNameCb, () => TlnNameCb.Text = "unknown");
				}
				Helper.ControlInvokeRequired(TlnAddressTb, () => TlnAddressTb.Text = favItem.Address);
				Helper.ControlInvokeRequired(TlnPortTb, () => TlnPortTb.Text = favItem.Port.ToString());
				Helper.ControlInvokeRequired(TlnExtensionTb, () => TlnExtensionTb.Text = favItem.DirectDial.ToString());

				/*
				bool success = await _itelex.ConnectOut(favItem.Address, favItem.Port, favItem.DirectDial, false);
				if (!success)
				{
					ShowLocalMessage(LngText(LngKeys.Message_ConnectionError));
				}
				*/
			});
			Itelex_UpdateHandler();
		}

		private void LoadSearchHistory()
		{
			try
			{
				_searchHistory = File.ReadAllLines(Constants.SEARCH_HISTORY).ToList();
			}
			catch
			{
				_searchHistory = new List<string>();
			}
			UpdateSearchHistory();
		}

		private void SaveSearchHistory()
		{
			try
			{
				File.WriteAllLines(Constants.SEARCH_HISTORY, _searchHistory);
			}
			catch
			{
			}
		}

		private void AddToSearchHistory(string searchStr)
		{
			if (!string.IsNullOrWhiteSpace(searchStr))
			{
				if (_searchHistory.Count > 0 && _searchHistory[0] != searchStr)
				{
					_searchHistory.Insert(0, searchStr);
				}
			}

			_searchHistory = _searchHistory.Take(Constants.SEARCH_HISTORY_LENGTH).ToList();
			SaveSearchHistory();
			UpdateSearchHistory();
		}

		private void UpdateSearchHistory()
		{
			SearchCb.DataSource = _searchHistory;
		}

		private void SetPeerTypes()
		{
			_tlnTypes = new PeerTypeItem[]
			{
				new PeerTypeItem(0, "0 deleted", LngText(LngKeys.MainForm_PeerType0)),
				new PeerTypeItem(1, "1 baud hostname", LngText(LngKeys.MainForm_PeerType1)),
				new PeerTypeItem(2, "2 baud fixed ip", LngText(LngKeys.MainForm_PeerType2)),
				new PeerTypeItem(3, "3 ascii hostname", LngText(LngKeys.MainForm_PeerType3)),
				new PeerTypeItem(4, "4 ascii fixed ip", LngText(LngKeys.MainForm_PeerType4)),
				new PeerTypeItem(5, "5 baud dyn ip", LngText(LngKeys.MainForm_PeerType4)),
				new PeerTypeItem(6, "6 email", LngText(LngKeys.MainForm_PeerType6)),
			};

			TlnTypeCb.DataSource = _tlnTypes;
			TlnTypeCb.DisplayMember = "ShortDesc";
		}
	}
}
