using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTlx.Languages;

namespace WinTlx
{
	public partial class TapePunchHorizontalForm : Form
	{
		private const string TAG = "TapePunchHorizontalForm";

		public delegate void ClosedEventHandler();
		public event ClosedEventHandler Closed;

		private Rectangle _parentWindowsPosition;

		private ItelexProtocol _itelex;

		private TapePunch _tapePunch;

		public TapePunchHorizontalForm(Rectangle position)
		{
			InitializeComponent();

			_parentWindowsPosition = position;

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageChanged();

			_itelex = ItelexProtocol.Instance;

			_tapePunch = TapePunch.Instance;
			_tapePunch.Punched += TapePunch_Punched;
			_tapePunch.Changed += TapePunch_Changed;
			_tapePunch.SetPuncherLinesHorizontal(PunchedTapePb.Width);
			RecvCb.Checked = _tapePunch.PuncherOn;
		}

		private void TapePunch_Punched()
		{
			//Helper.ControlInvokeRequired(PunchedTapePb, () => PunchedTapePb.Refresh());
		}

		private void TapePunch_Changed()
		{
			Debug.WriteLine(nameof(TapePunch_Changed));
			Helper.ControlInvokeRequired(PunchedTapePb, () => PunchedTapePb.Refresh());
			BufferLbl.Text = $"{_tapePunch.BufferPos} / {TapePunch.BUFFER_SIZE} / {_tapePunch.DisplayPos}";
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			this.Text = $"{Constants.PROGRAM_NAME} {LngText(LngKeys.TapePunch_TapePunch)}";
			RecvCb.Text = LngText(LngKeys.TapePunch_RecvButton);
			SendBtn.Text = LngText(LngKeys.TapePunch_SendButton);
			ClearBtn.Text = LngText(LngKeys.TapePunch_ClearButton);
			LoadBtn.Text = LngText(LngKeys.TapePunch_LoadButton);
			SaveBtn.Text = LngText(LngKeys.TapePunch_SaveButton);
			CloseBtn.Text = LngText(LngKeys.TapePunch_CloseButton);
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		private void PunchTapeForm_Load(object sender, EventArgs e)
		{
			SetPosition(_parentWindowsPosition);
		}

		private void PunchTapeForm_ResizeEnd(object sender, EventArgs e)
		{
			_tapePunch.SetPuncherLinesHorizontal(PunchedTapePb.Width);
			PunchedTapePb.Refresh();
		}

		private void CloseBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		public void SetPosition(Rectangle position)
		{
			int x = position.X + position.Width;
			int y = position.Y;
			SetBounds(x, y, Bounds.Width, Bounds.Height);
		}

		/*
		/// <summary>
		/// Punch a baudot code character
		/// </summary>
		/// <param name="baudotCode"></param>
		/// <param name="shiftState"></param>
		public void PunchCode(byte baudotCode, CodeConversion.ShiftStates shiftState)
		{
			_tapePunch.PunchCode(baudotCode, shiftState);
			Helper.ControlInvokeRequired(PunchedTapePb, () => PunchedTapePb.Refresh());
		}
		*/

		/// <summary>
		/// Update puncher graphic
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PunchedTapePb_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			_tapePunch.DrawTapeHorizontal(g, PunchedTapePb.Width, PunchedTapePb.Height);
		}

		private void ClearBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.Clear();
		}

		private void RecvCb_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void RecvCb_Click(object sender, EventArgs e)
		{
			SetRecv(RecvCb.Checked);
		}

		private void SetRecv(bool on)
		{
			RecvCb.Checked = on;
			_tapePunch.PuncherOn = on;
		}

		private void SendBtn_Click(object sender, EventArgs e)
		{
			SendBtn.Enabled = false;
			byte[] buffer = _tapePunch.GetBuffer();
			for (int i=0; i<buffer.Length; i++)
			{
				_itelex.SendBaudotCode(buffer[i]);
			}
			SendBtn.Enabled = true;
		}


		private void LoadBtn_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			//openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = openFileDialog.FileName;
				byte[] buffer = File.ReadAllBytes(filePath);
				_tapePunch.SetBuffer(buffer);
				PunchedTapePb.Refresh();
			}
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();

			saveFileDialog.Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 1;
			saveFileDialog.RestoreDirectory = true;

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				byte[] buffer = _tapePunch.GetBuffer();
				File.WriteAllBytes(saveFileDialog.FileName, buffer);
			}
		}

		private void TapePunchHorizontalForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Closed?.Invoke();
		}

		private void ScrollLeftBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.ScrollLeft(5);
		}

		private void ScrollRightBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.ScrollRight(5);
		}
	}
}
