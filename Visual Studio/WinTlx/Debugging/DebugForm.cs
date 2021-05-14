using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinTlx.Debugging
{
	public partial class DebugForm : Form
	{
		public delegate void CloseEventHandler();
		public event CloseEventHandler CloseEvent;

		private ItelexProtocol _itelex;
		private BufferManager _bufferManager;
		// private DebugManager _debugManager;

		public bool ShowHeartbeat => ShowHeartbeatCb.Checked;

		public bool ShowAck => ShowAckCb.Checked;

		private Rectangle? _parentWindowsPosition;

		public DebugForm(Rectangle? position=null)
		{
			_parentWindowsPosition = position;

			InitializeComponent();

			_itelex = ItelexProtocol.Instance;
			_itelex.Update += Itelex_Update;

			_bufferManager = BufferManager.Instance;

			ScrollCb.Checked = true;
			ShowHeartbeatCb.Checked = true;
			ShowAckCb.Checked = true;
			SendAckTb.Text = "";
			RecvBufTb.Text = "";
		}

		private void DebugForm_Load(object sender, EventArgs e)
		{
			if (_parentWindowsPosition != null)
			{
				Point pos = Helper.CenterForm(this, _parentWindowsPosition.Value);
				SetBounds(pos.X, pos.Y, Bounds.Width, Bounds.Height);
			}
		}

		private void DebugForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			CloseEvent?.Invoke();
		}

		public void ClearText()
		{
			Helper.ControlInvokeRequired(TextRtb, () => { TextRtb.Text = ""; });
		}

		private void Itelex_Update()
		{
			Helper.ControlInvokeRequired(SendAckTb, () => SendAckTb.Text = $"Itelex LB:{_itelex.SendBufferCount:D03} RB:{_itelex.RemoteBufferCount:D03}");
			Helper.ControlInvokeRequired(RecvBufTb, () => RecvBufTb.Text = $"Buf L:{_bufferManager.LocalOutputBufferCount} S:{_bufferManager.SendBufferCount}");

			Helper.ControlInvokeRequired(ConnectionStateTb, () =>
				ConnectionStateTb.Text = _itelex.ConnectionStateString
			);
		}

		/*
		public void DebugText(string text, Modes mode)
		{
			Color textColor = GetModeColor(mode);
			DebugText(text, textColor);
		}
		*/

		public void ShowDebugText(string text, Color textColor)
		{
			Helper.ControlInvokeRequired(TextRtb, () =>
			{
				int pos = TextRtb.TextLength;
				TextRtb.AppendText(text);
				TextRtb.Select(pos, text.Length);
				TextRtb.SelectionColor = textColor;
				TextRtb.Select();
				TextRtb.HideSelection = true;
				if (ScrollCb.Checked)
				{
					ScrollToBottom(TextRtb);
					//TextRtb.HideSelection = false; // hide selection so that AppendText will auto scroll to the end
				}
			});
		}

		public void ClearDebugText()
		{
			Helper.ControlInvokeRequired(TextRtb, () => { TextRtb.Text = ""; });
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
		private const int WM_VSCROLL = 277;
		private const int SB_PAGEBOTTOM = 7;

		internal static void ScrollToBottom(RichTextBox richTextBox)
		{
			SendMessage(richTextBox.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
			richTextBox.SelectionStart = richTextBox.Text.Length;
		}
	}
}
