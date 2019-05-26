﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

/// <summary>
/// Todo:
/// - eingehende Anrufe testen (funktioniert prinzipiell, verhaspelt sich ab und zu)
/// 
/// Version history
/// 1.0.0.1 - logging, improved error handling, inactivity timer
/// 1.0.0.2 - direct entry of peer address, port and extension is now possible
///         - fixed error on incoming connection (there still seems to be a problem)
///         - last line was not shown in terminal window
///         - Terminal windows is now scrollable
/// 1.0.0.3 - copy/paste implemented (context menu and ctrl-c/ctrl-v)
/// 1.0.0.4 - Removed expire date
///         - The size of the terminal window can now be changed dynamically.
///         - First GitHub release
/// 1.0.0.5 - Show messages when connecting to subscribe server
/// 
/// </summary>

namespace WinTelex
{
	public partial class MainForm : Form
	{
		private const string TAG = nameof(MainForm);

		private System.Timers.Timer _clockTimer;

		private SubscriberServer _subscriberServer;
		private ItelexProtocol _itelex;
		//private PeerQueryData _queryData;
		//private string _kennung = "\r\n123456 test d";
		//private bool ctrlKey = false;

		public const int SCREEN_WIDTH = 68;
		//public const int SCREEN_HEIGHT = 25;
		private int _screenHeight = 25;
		//private char[,] _screen = new char[SCREEN_WIDTH, SCREEN_HEIGHT];
		private List<ScreenLine> _screen = new List<ScreenLine>();
		private int _screenX = 0;
		private int _screenY = 0;
		private int _screenEditPos0 = 0;
		private int _screenShowPos0 = 0;

		public MainForm()
		{
			InitializeComponent();

			Logging.Instance.Log(LogTypes.Info, TAG, "Start", $"{Helper.GetVersion()}");

			this.Text = Helper.GetVersion();

			KeyPreview = true;

			MemberCb.DataSource = null;
			MemberCb.DisplayMember = "DisplayName";

			KennungTb.Text = Constants.DEFAULT_KENNUNG;
			SendLineFeedBtn.Text = "\u2261";
			//TimeBtn.Text = "\u2299";

			InactivityTimoutTb.Text = "";
			ConnTimeTb.Text = "";
			LnColTb.Text = "";
			SendAckTb.Text = "";

			this.KeyPreview = true;
			this.KeyDown += Form_KeyDown;
			this.KeyPress += Form_KeyPress;

			_itelex = new ItelexProtocol();
			_itelex.Received += ReceivedHandler;
			_itelex.Send += SendHandler;
			_itelex.Connected += ConnectedHandler;
			_itelex.Dropped += DroppedHandler;
			_itelex.Update += UpdatedHandler;
			_itelex.Error += ErrorHandler;

			_subscriberServer = new SubscriberServer();
			_subscriberServer.Message += SubcribeServerMessageHandler;

			_clockTimer = new System.Timers.Timer(500);
			_clockTimer.Elapsed += ClockTimer_Elapsed;
			_clockTimer.Start();

			RichTextTb.ContextMenuStrip = CreateContextMenu();

			SetConnectState();
			UpdatedHandler();

			ClearScreen();
			ShowScreen();


			_itelex.Start();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			/*
			if (Expired())
			{
				Close();
				return;
			}
			*/
			MainForm_Resize(null, null);
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

		private void ConnectBtn_Click(object sender, EventArgs e)
		{
			if (_itelex.IsConnected)
			{
				return;
			}
			ConnectOut();
			SetConnectState();
			SetFocus();
		}

		private void ConnectBtn_DoClick()
		{
			if (_itelex.IsConnected)
			{
				return;
			}
			ConnectOut();
			SetConnectState();
		}

		private void DisconnectBtn_Click(object sender, EventArgs e)
		{
			Disconnect();
		}

		private void LocalBtn_Click(object sender, EventArgs e)
		{
			LocalBtn_DoClick();
			SetFocus();
		}

		private void LocalBtn_DoClick()
		{
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
			_itelex.Disconnect();
			Close();
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
			//AddText("\u2629");
			SendWerDa();
			SetFocus();
		}

		private void SendHereIsBtn_Click(object sender, EventArgs e)
		{
			SendHereIs();
			SetFocus();
		}

		private void SendBellBtn_Click(object sender, EventArgs e)
		{
			SendAsciiText("\x7");
			//SystemSounds.Beep.Play();
			//AddText("\u04E8");
			SetFocus();
		}

		private void SendLettersBtn_Click(object sender, EventArgs e)
		{
			_itelex.SendBaudotChar(CodeConversion.LTR_SHIFT);
			SetFocus();
		}

		private void SendFiguresBtn_Click(object sender, EventArgs e)
		{
			_itelex.SendBaudotChar(CodeConversion.FIG_SHIFT);
			SetFocus();
		}

		private void SendCarriageReturnBtn_Click(object sender, EventArgs e)
		{
			//_itelex.SendBaudotChar(CodeConversion.BAU_CR);
			_itelex.SendAsciiText("\r");
			SetFocus();
		}

		private void SendLineFeedBtn_Click(object sender, EventArgs e)
		{
			//_itelex.SendBaudotChar(CodeConversion.BAU_LF);
			_itelex.SendAsciiText("\n");
			SetFocus();
		}

		private void SendTimeBtn_Click(object sender, EventArgs e)
		{
			SendAsciiText($"{DateTime.Now:dd.MM.yyyy  HH:mm}\r\n");
			SetFocus();
		}

		private void SendLineBtn_Click(object sender, EventArgs e)
		{
			SendAsciiText(new string('-', 66));
			SendAsciiText("\r\n");
		}

		private void SendRyBtn_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 33; i++)
			{
				SendAsciiText("ry");
			}
			SendAsciiText("\r\n");
			SetFocus();
		}

		private void SendFoxBtn_Click(object sender, EventArgs e)
		{
			SendAsciiText("the quick brown fox jumps over the lazy dog 1234567890/:,-=()\r\n");
			SetFocus();
		}

		private void RichTextTb_KeyPress(object sender, KeyPressEventArgs e)
		{
			char chr = e.KeyChar;

			string chrs = "";
			switch (chr)
			{
				default:
					chrs = chr.ToString();
					break;
				case '\r':
					chrs = "\r\n";
					break;
			}
			SendAsciiText(chrs);
			e.Handled = true;
		}

		private void RichTextTb_Click(object sender, EventArgs e)
		{
			//ShowScreen();
		}

		private void RichTextTb_KeyDown(object sender, KeyEventArgs e)
		{
			if (!e.Control)
			{
				return;
			}

			Debug.WriteLine($"{e.KeyCode} {e.KeyData} {e.KeyValue:X2}");
			switch (e.KeyCode)
			{
				default:
					//e.Handled = false;
					break;
				case Keys.A:
					ConnectOut();
					break;
				case Keys.S:
					Disconnect();
					break;
				case Keys.W:
					SendWerDa();
					break;
				case Keys.N: // letter switch
					_itelex.SendBaudotChar(CodeConversion.LTR_SHIFT);
					break;
				case Keys.O: // figures switch
					_itelex.SendBaudotChar(CodeConversion.FIG_SHIFT);
					break;
				case Keys.G: // bell
					_itelex.SendBaudotChar(CodeConversion.BAU_BEL);
					AddText("%");
					break;
				case Keys.I: // inquire, eigene kennung senden
					_itelex.SendBaudotChar(CodeConversion.BAU_WRU);
					AddText("@");
					break;
			}

			e.SuppressKeyPress = true;
			e.Handled = true;

		}

		private void RichTextTb_TextChanged(object sender, EventArgs e)
		{
			RichTextTb.SelectionStart = RichTextTb.Text.Length;
			RichTextTb.ScrollToCaret();
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
						AddText("\u04E8");
						return;
					case CodeConversion.ASC_WRU:
						SendHereIs();
						AddText("\u2629");
						return;
				}
			}
			AddText(asciiText);
		}

		private void SendHandler(string asciiText)
		{
			string dispText = "";
			for (int i = 0; i < asciiText.Length; i++)
			{
				char c = asciiText[i];
				switch (c)
				{
					case '\a': // bel
						SystemSounds.Beep.Play();
						c = '\u04E8';
						break;
					case '\t': //
						c = '\u2629';
						break;
				}
				dispText += c;
			}
			AddText(dispText);
		}

		private void SubcribeServerMessageHandler(string message)
		{
			AddText("\r\n" + message.ToUpper() + "\r\n");
		}

		private void ConnectedHandler()
		{
			AddText("\r\nCONNECTED\r\n");
			SetConnectState();
		}

		private void DroppedHandler()
		{
			AddText("\r\nDISCONNECTED\r\n");
			SetConnectState();
		}

		private void UpdatedHandler()
		{
			Helper.ControlInvokeRequired(SendAckTb, () => SendAckTb.Text = $"Snd {_itelex.CharsToSendCount}  Ack {_itelex.CharsAckCount}" );
			Helper.ControlInvokeRequired(InactivityTimoutTb, () => InactivityTimoutTb.Text = $"Timeout {_itelex.InactivityTimer} sec" );
			Helper.ControlInvokeRequired(ConnTimeTb, () => ConnTimeTb.Text = $"Conn {_itelex.ConnTimeMin} min" );

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
		}

		private bool ConnectOut()
		{
			AddressTb.Text = AddressTb.Text.Trim();
			if (string.IsNullOrWhiteSpace(AddressTb.Text))
			{
				AddText("NO ADDRESS");
				return false;
			}

			PortTb.Text = PortTb.Text.Trim();
			int? port = Helper.ToInt(PortTb.Text);
			if (port == null)
			{
				AddText("INVALID PORT\r\n");
				return false;
			}

			ExtensionTb.Text = ExtensionTb.Text.Trim();
			int? extension = null;
			if (!string.IsNullOrWhiteSpace(ExtensionTb.Text))
			{
				extension = Helper.ToInt(ExtensionTb.Text);
				if (extension == null)
				{
					AddText("INVALID EXTENSION NUMBER\r\n");
					return false;
				}
			}

			bool success = _itelex.ConnectOut(AddressTb.Text, port.Value, false);
			if (!success)
			{
				AddText("CONNECTION ERROR\r\n");
				return false;
			}

			_itelex.SendVersionCodeCmd();
			if (extension != null)
			{
				_itelex.SendDirectDialCmd(extension.Value);
			}

			_itelex.StartAck();

			return true;
		}

		private void Disconnect()
		{
			_itelex.SendEndCmd();
			_itelex.Disconnect();
			SetConnectState();
		}

		private void SendWerDa()
		{
			//_itelex.SendBaudotChar(CodeConversion.BAU_WRU);
			SendAsciiText("\x9");
			//AddText("\u2629");
			//AddText("@");
		}

		private void SendHereIs()
		{
			SendAsciiText("\r\n" + KennungTb.Text);
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
			ClearScreen();
		}

		private void SendFileBtn_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;

			if (openFileDialog.ShowDialog() != DialogResult.OK)
				return;


			string[] lines = null;
			try
			{
				string fullName = openFileDialog.FileName;
				lines = File.ReadAllLines(fullName);
				foreach (string line in lines)
				{
					// convert to replacements for real length
					string sendLine = CodeConversion.AsciiStringToTelex(line);
					if (sendLine.Length > 68)
						sendLine = sendLine.Substring(0, 68);
					SendAsciiText(sendLine + "\r\n");
				}
			}
			catch (Exception ex)
			{
			}
		}

		private async void QueryBtn_Click(object sender, EventArgs e)
		{
			SearchTb.Text = SearchTb.Text.Trim();

			//if (string.IsNullOrWhiteSpace(SearchTb.Text))
			//{
			//	return;
			//}

			Logging.Instance.Log(LogTypes.Info, TAG, nameof(QueryBtn_Click), $"SearchText='{SearchTb.Text}'");

			PeerQueryData[] list = null;

			uint num;
			if (!uint.TryParse(SearchTb.Text, out num))
				num = 0;

			await Task.Run(() =>
			{
				if (!_subscriberServer.Connect())
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
					list = new PeerQueryData[] { queryReply.Data };
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

			SubcribeServerMessageHandler($"{list.Length} member(s) found");

			MemberCb.DataSource = list;
			MemberCb.DisplayMember = "Display";
			if (list==null)
			{
			}
			else if (list.Length==0)
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
			Helper.ControlInvokeRequired(RichTextTb, () =>
			{
				RichTextTb.Text = "";
			});

			/*
			for (int y = 0; y < SCREEN_HEIGHT; y++)
			{
				for (int x = 0; x < SCREEN_WIDTH; x++)
				{
					_screen[x, y] = ' ';
				}
			}
			*/

			_screen.Clear();
			_screenX = 0;
			_screenY = 0;
			_screenEditPos0 = 0;
			_screenShowPos0 = 0;
			ShowScreen();
		}

		private void AddText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
					case '\n':
						IncScreenY();
						break;
					case '\r':
						_screenX = 0;
						break;
					default:
						if (_screenY + _screenEditPos0 >= _screen.Count)
						{
							_screen.Add(new ScreenLine());
						}
						_screen[_screenEditPos0 + _screenY].Line[_screenX] = text[i];
						IncScreenX();
						break;
				}
			}

			_screenShowPos0 = _screenEditPos0;

			ShowScreen();
			Log(text);
		}

		private void IncScreenX()
		{
			if (_screenX < SCREEN_WIDTH - 1)
			{
				_screenX++;
				if (_screenX == 60)
					SystemSounds.Beep.Play();
			}
			else
			{
				SystemSounds.Exclamation.Play();
			}


			/*
			if (_screenX >= SCREEN_WIDTH)
			{
				_screenX = 0;
				IncScreenY();
			}
			*/

			Debug.WriteLine($"{_screenX} {_screenY}");
		}

		private void IncScreenY()
		{
			_screen.Add(new ScreenLine());
			_screenY++;
			if (_screenY >= _screenHeight)
			{
				// scroll
				/*
				for (int y = 0; y < SCREEN_HEIGHT - 1; y++)
				{
					for (int x = 0; x < SCREEN_WIDTH; x++)
					{
						_screen[x, y] = _screen[x, y + 1];
					}
				}
				for (int x = 0; x < SCREEN_WIDTH; x++)
				{
					_screen[x, SCREEN_HEIGHT - 1] = ' ';
				}
				*/
				_screenEditPos0++;
				_screenShowPos0 = _screenEditPos0;
				_screenY--;
				ShowScreen();
			}
			Debug.WriteLine($"{_screenX} {_screenY}");
		}

		private void ShowScreen()
		{
			string lines = "";
			int cursorPos = 0;
			int pos = 0;
			for (int y = 0; y < _screenHeight; y++)
			{
				string line = "";
				for (int x = 0; x < SCREEN_WIDTH; x++)
				{
					char chr;
					if (y + _screenShowPos0 < _screen.Count)
					{
						chr = _screen[_screenShowPos0 + y].Line[x];
					}
					else
					{
						chr = ' ';
					}
					if (x == _screenX && y == _screenY && _screenShowPos0 == _screenEditPos0)
					{
						cursorPos = pos;
						if (chr == ' ')
							chr = '_';
					}
					line += chr;
					pos++;
				}
				lines += line.TrimEnd();
				if (y < _screenHeight - 1)
					lines += "\r";
				pos = lines.Length;
			}

			//lines += "*";
			lines = lines.Replace('_', ' ');

			Helper.ControlInvokeRequired(RichTextTb, () =>
			{
				RichTextTb.Text = lines;

				RichTextTb.Focus();
				RichTextTb.SelectionStart = cursorPos;
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
			RichTextTb.Focus();
		}

		private void Log(string text)
		{
			File.AppendAllText($"{Constants.PROGRAM_NAME}.log", text);
		}

		private void AboutBtn_Click(object sender, EventArgs e)
		{
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

		/*
		private bool Expired()
		{
			if (!Helper.IsExpired())
			{
				return false;
			}

			string text =
				$"{Helper.GetVersion()}\r\r" +
				"This test version expired";
			MessageBox.Show(
				text,
				$"Expired",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button1);

			return true;
		}
		*/

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

		private void RichTextTb_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{   //click event
				//MessageBox.Show("you got it!");
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
				RichTextTb.ContextMenu = contextMenu;
			}
		}

		private void ClearAction(object sender, EventArgs e)
		{
			ClearScreen();
		}

		private void CopyAction(object sender, EventArgs e)
		{
			string selectedText = RichTextTb.SelectedText;
			Clipboard.SetData(DataFormats.Text, selectedText);
		}

		private void PasteAction(object sender, EventArgs e)
		{
			if (Clipboard.ContainsText(TextDataFormat.Text))
			{
				SendAsciiText(Clipboard.GetText());
			}
		}

		private void ErrorHandler(string asciiText)
		{
			Helper.ControlInvokeRequired(RichTextTb, () =>
			{
				ShowLocalError(asciiText);
			});
		}

		private void ShowLocalError(string message)
		{
			AddText(message.ToUpper());
			AddText("\r\n");
			SystemSounds.Exclamation.Play();
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			RichTextTb.Height = this.Height - 300 + 50;
			_screenHeight = RichTextTb.Height / 19;
			Debug.WriteLine($"{_screenHeight} {RichTextTb.Height}");

			_screenEditPos0 = _screen.Count - _screenHeight;
			if (_screenEditPos0 < 0)
				_screenEditPos0 = 0;
			_screenShowPos0 = _screenEditPos0;

			_screenY = _screen.Count - _screenEditPos0 - 1;

#warning TODO warum ist _screenY<0
			if (_screenY < 0)
				_screenY = 0;

			ShowScreen();
		}
	}

	class ScreenLine
	{
		public char[] Line { get; set; }

		public ScreenLine()
		{
			Line = new char[MainForm.SCREEN_WIDTH];
			for (int i=0; i<MainForm.SCREEN_WIDTH; i++)
			{
				Line[i] = ' ';
			}
		}
	}
}
