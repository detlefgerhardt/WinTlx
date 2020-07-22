using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Languages;

namespace WinTlx.TapePunch
{
	class TapePunchManager
	{
		//public const int BUFFER_SIZE = 50000;

		private const int HOLE_DIST = 10;
		private const int HOLE_SIZE = 7;
		private const int TRANSPORT_HOLE_SIZE = 3;
		private const int BORDER = 5;

		private const int MAX_UNDO = 10;

		private readonly ItelexProtocol _itelex;

		private List<PunchLine> _buffer;
		private readonly List<List<PunchLine>> _undoBuffer;

		public int VisiblePunchLines { get; set; }
		public int DisplayPos { get; set; }

		public bool PuncherOn { get; set; }

		public bool EditOn { get; set; }
		public bool EditInsert { get; set; }

		public int BufferSize => _buffer.Count;

		public delegate void PunchedEventHandler();
		public event PunchedEventHandler Punched;

		public delegate void ChangedEventHandler();
		public event ChangedEventHandler Changed;

		private ConfigData _config => ConfigManager.Instance.Config;

		private bool _updateActive;

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static TapePunchManager instance;

		public static TapePunchManager Instance => instance ?? (instance = new TapePunchManager());

		private TapePunchManager()
		{
			_itelex = ItelexProtocol.Instance;
			_itelex.BaudotSendRecv += BaudotSendRecvHandler;

			_buffer = new List<PunchLine>();
			_undoBuffer = new List<List<PunchLine>>();
			PuncherOn = true;
			_updateActive = true;
		}

		private void BaudotSendRecvHandler(byte[] code)
		{
			for (int i = 0; i < code.Length; i++)
			{
				PunchCode(code[i], _itelex.ShiftState);
			}
			InvokePunched();
		}

		public void Clear()
		{
			PushUndo();
			_buffer = new List<PunchLine>();
			DisplayPos = 0;
			InvokeChanged();
		}

		public void SetBuffer(byte[] buffer)
		{
			ShiftStates shiftState = ShiftStates.Ltr;
			_updateActive = false;
			Clear();
			for (int i = 0; i < buffer.Length; i++)
			{
				InternPunchCode(buffer[i], shiftState);
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
			_updateActive = true;
			InvokeChanged();
		}

		public byte[] GetBufferFromCurrentPos()
		{
			byte[] buffer = new byte[_buffer.Count-DisplayPos];
			for (int i = DisplayPos; i < _buffer.Count; i++)
			{
				buffer[i-DisplayPos] = _buffer[i].Code;
			}
			return buffer;
		}

		public byte[] GetBuffer()
		{
			byte[] buffer = new byte[_buffer.Count];
			for (int i = 0; i < _buffer.Count; i++)
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
			if (PuncherOn)
			{
				InternPunchCode(baudotCode, shiftState);
			}
		}

		private void InternPunchCode(byte baudotCode, ShiftStates shiftState)
		{
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

			PunchLine newLine = new PunchLine(baudotCode, text);

			if (!EditOn)
			{
				_buffer.Add(newLine);
				DisplayPos = _buffer.Count;
			}
			else
			{
				if (EditInsert)
				{
					if (DisplayPos >= _buffer.Count)
					{
						_buffer.Add(newLine);
					}
					else
					{
						_buffer.Insert(DisplayPos, newLine);
					}
					DisplayPos++;
				}
				else
				{
					if (DisplayPos >= _buffer.Count)
					{
						_buffer.Add(newLine);
					}
					else
					{
						_buffer[DisplayPos] = newLine;
					}
					DisplayPos++;
				}
				if (DisplayPos > _buffer.Count)
				{
					DisplayPos = _buffer.Count;
				}
			}

			InvokeChanged();
		}

		private void PushUndo()
		{
			_undoBuffer.Add(_buffer);
			if (_undoBuffer.Count>MAX_UNDO)
			{
				_undoBuffer.RemoveAt(0);
			}
		}

		public void PullUndo()
		{
			if (_undoBuffer.Count > 0)
			{
				_buffer = _undoBuffer[_undoBuffer.Count - 1];
				_undoBuffer.RemoveAt(_undoBuffer.Count - 1);
				InvokeChanged();
			}
		}

		public void DeleteCode()
		{
			if (!EditOn|| _buffer.Count==0 || DisplayPos >= _buffer.Count)
			{
				return;
			}

			PushUndo();

			_buffer.RemoveAt(DisplayPos);
			if (DisplayPos > _buffer.Count)
			{
				DisplayPos = _buffer.Count;
			}

			InvokeChanged();
		}

		public void CropStart()
		{
			Debug.WriteLine($"CropStart {_buffer.Count} {DisplayPos} {_buffer.Count - DisplayPos}");
			if (DisplayPos >= _buffer.Count - 1)
			{
				PushUndo();
				_buffer = new List<PunchLine>();
				DisplayPos = 0;
				InvokeChanged();
				return;
			}

			if (DisplayPos < _buffer.Count)
			{
				PushUndo();
				_buffer = _buffer.GetRange(DisplayPos + 1, _buffer.Count - DisplayPos - 1);
				DisplayPos = 0;
				InvokeChanged();
			}
		}

		public void CropEnd()
		{
			if (DisplayPos < _buffer.Count)
			{
				PushUndo();
				_buffer = _buffer.GetRange(0, DisplayPos);
				DisplayPos = _buffer.Count;
				InvokeChanged();
			}
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
			int y0 = height - th - 20;
			g.FillRectangle(tapeBrush, 0, y0, width, th);

			//int pos = DisplayPos - 1;
			int pos = DisplayPos - VisiblePunchLines / 2 - 2 + 1;
			/*
			//if (EditOn)
			if (true)
			{
					pos = DisplayPos - VisiblePunchLines / 2 - 2 + 1;
			}
			else
			{
				pos = DisplayPos - VisiblePunchLines + 1;
			}
			*/

			// draw from left to right
			for (int line = 0; line < VisiblePunchLines; line++)
			{
				if (pos < 0 || pos > _buffer.Count - 1)
				{
					pos++;
					continue;
				}
				int xp = line * HOLE_DIST + HOLE_DIST - 2;
				int bit = 16;
				PunchLine punchLine = _buffer[pos++];
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
				//if (line < VisiblePunchLines-1)
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

			// draw position marker
			int mx = (VisiblePunchLines / 2 + 2) * HOLE_DIST + 1;
			/*
			if (true)
			//if (EditOn)
			{
			}
			else
			{
				mx = VisiblePunchLines * HOLE_DIST + 1;
			}
			*/
			Brush posBrush = new SolidBrush(Color.Red);
			int my = y0 + BORDER + HOLE_DIST * 6 + 5;
			g.FillPolygon(posBrush, PosPolygon(mx, my));
		}

		private PointF[] PosPolygon(int x, int y)
		{
			PointF[] pol = new PointF[4];
			pol[0] = new PointF(x, y);
			pol[1] = new PointF(x, y+10);
			pol[2] = new PointF(x+2, y + 10);
			pol[3] = new PointF(x + 2, y);
			return pol;
		}

		public void SetPuncherLinesVertical(int height)
		{
			VisiblePunchLines = height / HOLE_DIST;
			InvokeChanged();
		}

		public void SetPuncherLinesHorizontal(int width)
		{
			VisiblePunchLines = (width - 8) / HOLE_DIST;
			InvokeChanged();
		}

		public void ScrollLeft(int positions)
		{
			int newPos = DisplayPos + positions;
			if (newPos > _buffer.Count)
			{
				newPos = _buffer.Count;
			}

			if (newPos != DisplayPos)
			{
				DisplayPos = newPos;
				InvokeChanged();
			}
		}

		public void ScrollRight(int positions)
		{
			int newPos = SetMinPosition(DisplayPos - positions);

			if (newPos != DisplayPos)
			{
				DisplayPos = newPos;
				InvokeChanged();
			}
		}

		public int SetMinPosition(int position)
		{
			if (position<0)
			{
				position = 0;
			}

			/*
			if (_buffer.Count > 0)
			{
				if (position < 1)
				{
					position = 1;
				}
			}
			else
			{
				position = 0;
			}
			*/
			return position;
		}

		private void InvokeChanged()
		{
			if (_updateActive)
			{
				Changed?.Invoke();
			}
		}

		private void InvokePunched()
		{
			if (_updateActive)
			{
				Punched?.Invoke();
			}
		}

	}
}
