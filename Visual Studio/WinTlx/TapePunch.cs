using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx
{
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

	class TapePunch
	{
		private const int BUFFER_SIZE = 5000;

		private const int HOLE_DIST = 10;
		private const int HOLE_SIZE = 7;
		private const int TRANSPORT_HOLE_SIZE = 3;
		private const int BORDER = 5;

		private PunchLine[] _buffer;

		private int _bufferPos = 0;

		private int _puncherLines;

		private bool _puncherOn;

		public void Init()
		{
			_buffer = new PunchLine[BUFFER_SIZE];
			SetOnOff(true);

		}

		public void Clear()
		{
			_bufferPos = 0;
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
		}

		/// <summary>
		/// Draw current buffer to puncher graphic
		/// </summary>
		/// <param name="g"></param>
		public void DrawTapeVertical(Graphics g, int height)
		{
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			//Color tapeColor = Color.FromArgb(0xFF, 0xFF, 0xA0);
			Brush tapeBrush = new SolidBrush(Color.FromArgb(0xFF, 0xFF, 0xA0));
			Brush holeBrush = new SolidBrush(Color.FromArgb(0x40, 0x40, 0x40));
			Color backColor = Color.FromArgb(240, 240, 240);
			g.Clear(backColor);
			g.FillRectangle(tapeBrush, 0, 0, HOLE_DIST * 6 + BORDER * 2 - 2, height);


			int pos = _bufferPos - 1;
			for (int line = 0; line < _puncherLines; line++)
			{
				if (pos < 0)
				{
					break;
				}
				int yp = line * HOLE_DIST;
				int bit = 16;
				PunchLine punchLine = _buffer[pos--];
				for (int col = 0; col < 6; col++)
				{
					if (col == 2)
					{
						int d = (HOLE_SIZE - TRANSPORT_HOLE_SIZE) / 2;
						g.FillEllipse(holeBrush, BORDER + HOLE_DIST * col + d, yp + d + 6, TRANSPORT_HOLE_SIZE, TRANSPORT_HOLE_SIZE);
						continue;
					}

					if ((punchLine.Code & bit) != 0)
					{
						g.FillEllipse(holeBrush, BORDER + HOLE_DIST * col, yp + 6, HOLE_SIZE, HOLE_SIZE);
					}
					bit >>= 1;
				}

				// show char
				Point p = new Point(BORDER + HOLE_DIST * 6 + 9, yp + 3);
				g.DrawString(punchLine.Text, new Font("Arial", 8), new SolidBrush(Color.Black), p);
			}

		}

		/// <summary>
		/// Draw current buffer to puncher graphic
		/// </summary>
		/// <param name="g"></param>
		public void DrawTapeHorizontal(Graphics g, int width, int height)
		{
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			Brush tapeBrush = new SolidBrush(Color.FromArgb(0xFF, 0xFF, 0xA0));
			Brush holeBrush = new SolidBrush(Color.FromArgb(0x40, 0x40, 0x40));
			Color backColor = Color.FromArgb(240, 240, 240);
			Brush backBrush = new SolidBrush(backColor);

			g.Clear(backColor);
			int th = HOLE_DIST * 6 + BORDER * 2 - 2;
			int y0 = height - th;
			g.FillRectangle(tapeBrush, 0, y0, width, th);

			int pos = _bufferPos - 1;
			for (int line = 0; line < _puncherLines; line++)
			{
				if (pos < 0)
				{
					break;
				}
				//int xp = line * HOLE_DIST;
				int xp = width - line * HOLE_DIST - HOLE_DIST;
				int bit = 16;
				PunchLine punchLine = _buffer[pos--];
				for (int col = 0; col < 6; col++)
				{
					if (col == 2)
					{
						int d = (HOLE_SIZE - TRANSPORT_HOLE_SIZE) / 2;
						g.FillEllipse(holeBrush, xp + d, y0 + BORDER + HOLE_DIST * col + d, TRANSPORT_HOLE_SIZE, TRANSPORT_HOLE_SIZE);
						continue;
					}

					if ((punchLine.Code & bit) != 0)
					{
						g.FillEllipse(holeBrush, xp, y0 + BORDER + HOLE_DIST * col, HOLE_SIZE, HOLE_SIZE);
					}
					bit >>= 1;
				}

				// draw char, skip left most line
				if (line < _puncherLines - 1)
				{
					Font font = new Font("Arial", 8);
					string text = punchLine.Text;
					for (int i = 0; i < text.Length; i++)
					{
						Point p = new Point(xp - 0, y0 - 5 - 10 * (text.Length - i));
						g.DrawString(text.Substring(i, 1), font, Brushes.Black, p);
					}
				}
			}

			// draw tip on the left side
			int len = 20;
			PointF[] pol = new PointF[4];
			pol[0] = new PointF(0, y0);
			pol[1] = new PointF(len, y0);
			pol[2] = new PointF(0, y0+th/7*3);
			pol[3] = new PointF(0, y0);
			g.FillPolygon(backBrush, pol);
			pol[0] = new PointF(0, y0+th);
			pol[1] = new PointF(len, y0+th);
			pol[2] = new PointF(0, y0 + th / 7*3);
			pol[3] = new PointF(0, y0+th);
			g.FillPolygon(backBrush, pol);

		}

		public void SetPuncherLinesVertical(int height)
		{
			_puncherLines = height / HOLE_DIST;
		}

		public void SetPuncherLinesHorizontal(int width)
		{
			_puncherLines = width / HOLE_DIST;
		}

		public void SetOnOff(bool on)
		{
			_puncherOn = on;
		}


	}
}
