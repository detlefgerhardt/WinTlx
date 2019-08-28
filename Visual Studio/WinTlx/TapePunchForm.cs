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
	public partial class TapePunchForm : Form
	{
		private Rectangle _parentWindowsPosition;

		private TapePunch _tapePunch;

		public TapePunchForm(Rectangle position)
		{
			InitializeComponent();

			_parentWindowsPosition = position;

			_tapePunch = new TapePunch();
			_tapePunch.Init();
			_tapePunch.SetPuncherLinesVertical(PunchedTapePb.Height);
		}

		private void BuildForm_Vertical()
		{
		}

		private void PunchTapeForm_Load(object sender, EventArgs e)
		{
			SetPosition(_parentWindowsPosition);
		}

		private void PunchTapeForm_ResizeEnd(object sender, EventArgs e)
		{
			_tapePunch.SetPuncherLinesVertical(PunchedTapePb.Height);
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
			_tapePunch.DrawTapeVertical(g, PunchedTapePb.Height);
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
			_tapePunch.PuncherOn = on;
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
