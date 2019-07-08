using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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

		private Rectangle _parentWindowsPosition;

		private TapePunch _tapePunch;

		public TapePunchHorizontalForm(Rectangle position)
		{
			InitializeComponent();

			_parentWindowsPosition = position;

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageChanged();

			_tapePunch = new TapePunch();
			_tapePunch.Init();
			_tapePunch.SetPuncherLinesHorizontal(PunchedTapePb.Width);
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			this.Text = $"{Constants.PROGRAM_NAME} {LngText(LngKeys.TapePunch_TapePunch)}";
			OnCb.Text = LngText(LngKeys.TapePunch_OnButton);
			OffCb.Text = LngText(LngKeys.TapePunch_OffButton);
			ClearBtn.Text = LngText(LngKeys.TapePunch_ClearButton);
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
			PunchedTapePb.Refresh();
		}

		private void OnCb_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void OffCb_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void OnCb_Click(object sender, EventArgs e)
		{
			SetOnOff(true);
		}

		private void OffCb_Click(object sender, EventArgs e)
		{
			SetOnOff(false);
		}

		private void SetOnOff(bool on)
		{
			_tapePunch.SetOnOff(on);
			if (on)
			{
				OnCb.Checked = true;
				OffCb.Checked = false;
			}
			else
			{
				OnCb.Checked = false;
				OffCb.Checked = true;
			}
		}
	}

}
