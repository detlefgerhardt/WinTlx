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

namespace WinTlx
{
	public partial class TapePunchForm : Form
	{
		private const int BUFFER_SIZE = 5000;

		private const int HOLE_DIST = 10;

		private Rectangle _parentWindowsPosition;

		private PunchLine[] _buffer;

		private int _bufferPos = 0;

		private int _puncherLines;

		private bool _puncherOn;

		public TapePunchForm(Rectangle position)
		{
			InitializeComponent();

			_parentWindowsPosition = position;
			_buffer = new PunchLine[BUFFER_SIZE];
			SetPuncherLines();
			SetOnOff(true);
		}

		private void PunchTapeForm_Load(object sender, EventArgs e)
		{
			SetPosition(_parentWindowsPosition);
		}

		private void PunchTapeForm_ResizeEnd(object sender, EventArgs e)
		{
			SetPuncherLines();
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
			if (!_puncherOn)
			{
				return;
			}

			string text = CodeConversion.BaudotCodeToPuncherText(baudotCode, shiftState);
			_buffer[_bufferPos++] = new PunchLine(baudotCode, text);
			if (_bufferPos >= _buffer.Length)
			{
				_bufferPos = 0;
			}

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
			DrawTape(g);
		}

		private void SetPuncherLines()
		{
			_puncherLines = PunchedTapePb.Height / HOLE_DIST;
		}

		/// <summary>
		/// Draw current buffer to puncher graphic
		/// </summary>
		/// <param name="g"></param>
		private void DrawTape(Graphics g)
		{
			int holeSize = 7;
			int indexSize = 4;
			int border = 5;

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			//Color tapeColor = Color.FromArgb(0xFF, 0xFF, 0xA0);
			Brush tapeBrush = new SolidBrush(Color.FromArgb(0xFF, 0xFF, 0xA0));
			Brush holeBrush = new SolidBrush(Color.FromArgb(0x40, 0x40, 0x40));
			Color backColor = Color.FromArgb(240,240,240);
			g.Clear(backColor);
			g.FillRectangle(tapeBrush, 0, 0, HOLE_DIST * 6 + border * 2-2, PunchedTapePb.Height);


			int pos = _bufferPos - 1;
			for (int line = 0; line < _puncherLines; line++)
			{
				if (pos < 0)
				{
					break;
				}
				int y0 = line * HOLE_DIST;
				int bit = 16;
				PunchLine punchLine = _buffer[pos--];
				for (int col = 0; col < 6; col++)
				{
					if (col == 2)
					{
						int d = (holeSize - indexSize) / 2;
						g.FillEllipse(holeBrush, border + HOLE_DIST * col + d, y0 + d + 6, indexSize, indexSize);
						continue;
					}

					if ((punchLine.Code & bit) != 0)
					{
						g.FillEllipse(holeBrush, border + HOLE_DIST * col, y0 + 6, holeSize, holeSize);
					}
					bit >>= 1;
				}

				// show char
				Point p = new Point(border + HOLE_DIST * 6 + 9, y0 + 3);
				g.DrawString(punchLine.Text, new Font("Arial", 8), new SolidBrush(Color.Black), p);
			}
		}

		private void ClearBtn_Click(object sender, EventArgs e)
		{
			_bufferPos = 0;
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
			_puncherOn = on;
			if (_puncherOn)
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

	class PunchLine
	{
		public byte Code { get; set; }

		public string Text { get; set; }

		public PunchLine(byte code, string text)
		{
			Code = code;
			Text = text;
		}
	}
}
