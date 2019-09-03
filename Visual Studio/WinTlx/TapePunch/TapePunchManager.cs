using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Languages;

namespace WinTlx.TapePunch
{
	class TapePunchManager
	{
		public const int BUFFER_SIZE = 50000;

		private const int HOLE_DIST = 10;
		private const int HOLE_SIZE = 7;
		private const int TRANSPORT_HOLE_SIZE = 3;
		private const int BORDER = 5;

		private PunchLine[] _buffer;

		public int PunchLines { get; set; }
		public int BufferPos { get; set; }
		public int DisplayPos { get; set; }

		private ItelexProtocol _itelex;

		public delegate void PunchedEventHandler();
		public event PunchedEventHandler Punched;

		public delegate void ChangedEventHandler();
		public event ChangedEventHandler Changed;

		private ConfigData _config => ConfigManager.Instance.Config;

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static TapePunchManager instance;

		public static TapePunchManager Instance => instance ?? (instance = new TapePunchManager());

		private TapePunchManager()
		{
			_itelex = ItelexProtocol.Instance;
			_itelex.BaudotSendRecv += BaudotSendRecvHandler;

			_buffer = new PunchLine[BUFFER_SIZE];
			PuncherOn = true;
		}

		private void BaudotSendRecvHandler(byte[] code)
		{
			for (int i = 0; i < code.Length; i++)
			{
				PunchCode(code[i], _itelex.ShiftState);
			}
			Punched?.Invoke();
		}

		private bool _puncherOn;
		public bool PuncherOn
		{
			get { return _puncherOn; }
			set { _puncherOn = value; }
		}

		public void Clear()
		{
			BufferPos = 0;
			DisplayPos = 0;
			Punched?.Invoke();
		}

		public void SetBuffer(byte[] buffer)
		{
			ShiftStates shiftState = ShiftStates.Ltr;
			Clear();
			for (int i = 0; i < buffer.Length; i++)
			{
				PunchCode(buffer[i], shiftState);
				switch (buffer[i])
				{
					case CodeManager.BAU_LTRS:
						shiftState = ShiftStates.Ltr;
						break;
					case CodeManager.BAU_FIGS:
						shiftState = ShiftStates.Figs;
						break;
				}
			}
			Changed?.Invoke();
		}

		public byte[] GetBuffer()
		{
			byte[] buffer = new byte[BufferPos];
			for (int i = 0; i < BufferPos; i++)
			{
				buffer[i] = _buffer[i].Code;
			}
			return buffer;
		}

		/// <summary>
		/// Punch a baudot code character
		/// </summary>
		/// <param name="baudotCode"></param>
		/// <param name="shiftState"></param>
		public void PunchCode(byte baudotCode, ShiftStates shiftState)
		{
			if (!PuncherOn)
			{
				return;
			}

			string text = CodeManager.BaudotCodeToPuncherText(baudotCode, shiftState, _config.CodeSet);
			switch (text)
			{
				case "CR":
					text = LngText(LngKeys.TapePunch_CodeCarriageReturn);
					break;
				case "NL":
					text = LngText(LngKeys.TapePunch_CodeLinefeed);
					break;
				case "LTR":
					text = LngText(LngKeys.TapePunch_CodeLetters);
					break;
				case "FIG":
					text = LngText(LngKeys.TapePunch_CodeFigures);
					break;
			}

			_buffer[BufferPos] = new PunchLine(baudotCode, text);
			BufferPos++;
			DisplayPos++;
			if (BufferPos >= _buffer.Length)
			{
				BufferPos = 0;
				DisplayPos = 0;
			}
			Changed?.Invoke();
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
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
			int th = (int)(HOLE_DIST * 6 + BORDER * 2 - 2);
			int y0 = height - th;
			g.FillRectangle(tapeBrush, 0, y0, width, th);

			//int pos = BufferPos - 1;
			int pos = DisplayPos - 1;

			//Debug.WriteLine($"pos={pos}");

			// draw from left to tight
			for (int line = 0; line < PunchLines; line++)
			{
				if (pos < 0)
				{
					break;
				}
				//int xp = line * HOLE_DIST;
				int xp = (int)(width - line * HOLE_DIST - HOLE_DIST);
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
				if (line < PunchLines - 1)
				{
					Font font = new Font("Arial", 8);
					string text = punchLine.Text;
					for (int i = 0; i < text.Length; i++)
					{
						Point p = new Point(xp - 2, y0 - 5 - 10 * (text.Length - i));
						g.DrawString(text.Substring(i, 1), font, Brushes.Black, p);
					}
				}
			}

			// draw tip on the left side
			int len = 20;
			PointF[] pol = new PointF[4];
			pol[0] = new PointF(0, y0);
			pol[1] = new PointF(len, y0);
			pol[2] = new PointF(0, y0 + th / 7 * 3);
			pol[3] = new PointF(0, y0);
			g.FillPolygon(backBrush, pol);
			pol[0] = new PointF(0, y0 + th);
			pol[1] = new PointF(len, y0 + th);
			pol[2] = new PointF(0, y0 + th / 7 * 3);
			pol[3] = new PointF(0, y0 + th);
			g.FillPolygon(backBrush, pol);

		}

		public void SetPuncherLinesVertical(int height)
		{
			PunchLines = (int)(height / HOLE_DIST);
			Changed?.Invoke();
		}

		public void SetPuncherLinesHorizontal(int width)
		{
			PunchLines = (int)(width / HOLE_DIST);
			Changed?.Invoke();
		}

		public void ScrollLeft(int positions)
		{
			int newPos = DisplayPos;
			if (BufferPos <= PunchLines)
			{
				// fits to windows
				newPos = 0;
			}
			else
			{
				newPos += positions;
				if (newPos > BufferPos)
				{
					newPos = BufferPos;
				}
				else if (newPos > BUFFER_SIZE - PunchLines)
				{
					newPos = BUFFER_SIZE - PunchLines;
				}
			}

			if (newPos != DisplayPos)
			{
				DisplayPos = newPos;
				Changed?.Invoke();
			}
		}

		public void ScrollRight(int positions)
		{
			int newPos = DisplayPos;
			if (BufferPos <= PunchLines)
			{
				// fits to windows
				newPos = 0;
			}
			else
			{
				newPos -= positions;
				if (newPos < 0)
				{
					newPos = 0;
				}
				else if (newPos < PunchLines - 2)
				{
					newPos = PunchLines - 2;
				}
			}

			if (newPos != DisplayPos)
			{
				DisplayPos = newPos;
				Changed?.Invoke();
			}
		}
	}
}
