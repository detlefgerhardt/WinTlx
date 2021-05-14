using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	class SpecialCharacters
	{
		private List<SpecialCharacterItem> _specialChrs;

		private static SpecialCharacters instance;

		public static SpecialCharacters Instance => instance ?? (instance = new SpecialCharacters());

		private int _charWidth;
		private int _charHeight;

		private SpecialCharacters()
		{
		}

		public void Init(int charWidth, int charHeight)
		{
			_charWidth = charWidth;
			_charHeight = charHeight;

			_specialChrs = new List<SpecialCharacterItem>();
			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_BEL, _bellChar, Color.Black));
			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_BEL, _bellChar, Color.Red));

			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_WRU, _wruChar, Color.Black));
			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_WRU, _wruChar, Color.Red));

			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_SHIFTF, _shiftF, Color.Black));
			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_SHIFTF, _shiftF, Color.Red));

			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_SHIFTG, _shiftG, Color.Black));
			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_SHIFTG, _shiftG, Color.Red));

			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_SHIFTH, _shiftH, Color.Black));
			_specialChrs.Add(GetSpecialCharacter(CodeManager.ASC_SHIFTH, _shiftH, Color.Red));
		}

		public Bitmap GetSpecialChrBmp(char code, Color color)
		{
			SpecialCharacterItem item = (from s in _specialChrs where s.Code == code && s.Color == color select s).FirstOrDefault();
			if (item == null)
			{
				item = (from s in _specialChrs where s.Code == code && s.Color == Color.Black select s).FirstOrDefault();
				if (item == null)
				{
					return new Bitmap(_charWidth, _charHeight);
				}
			}
			return item.Bitmap;
		}

		private SpecialCharacterItem GetSpecialCharacter(char code, string[] charDef, Color color)
		{
			return new SpecialCharacterItem()
			{
				Code = code,
				Color = color,
				Bitmap = GetCharBitmap(charDef, color)
			};
		}

		/*
		private void InitChrBmp()
		{
			_chrSet = new Bitmap[64];

			Font font = new Font("Consolas", 12);
			Brush brush = new SolidBrush(Color.Black);
			for (int i=0; i<_chrSet.Length; i++)
			{
				Bitmap chrBmp = new Bitmap(9, 16);
				Graphics g = Graphics.FromImage(chrBmp);
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
				g.Clear(Color.White);
				char?[] code = i < 32 ? CodeManager.GetCodeItem(i).CharLtr : CodeManager.GetCodeItem(i - 32).CharFig;
				//char?[] code = i < 32 ? codeItem.CharLtr : codeItem.CharFig;
				if (code[0].HasValue)
				{
					g.DrawString(code[0].Value.ToString(), font, brush, -3, -3);
				}

				chrBmp.MakeTransparent();
				_chrSet[i] = chrBmp;

				string fileName = $"char{i:D02}.png";
				File.Delete(fileName);
				chrBmp.Save(fileName, ImageFormat.Png);
			}
		}

		public Bitmap GetChrBmp(int code, ShiftStates shiftState)
		{
			switch(shiftState)
			{
				case ShiftStates.Ltr:
					return _chrSet[code];
				case ShiftStates.Figs:
					return _chrSet[code+32];
				default:
					return null;
			}
		}
		*/

		public Bitmap GetBell(Color color)
		{
			return GetCharBitmap(_bellChar, color);
		}

		public Bitmap GetWru(Color color)
		{
			return GetCharBitmap(_wruChar, color);
		}

		public Bitmap GetShiftF(Color color)
		{
			return GetCharBitmap(_shiftF, color);
		}

		public Bitmap GetShiftG(Color color)
		{
			return GetCharBitmap(_shiftG, color);
		}

		public Bitmap GetShiftH(Color color)
		{
			return GetCharBitmap(_shiftH, color);
		}

		public Bitmap GetScrollStart(Color color)
		{
			return GetCharBitmap(_scrollStartChar, color);
		}

		public Bitmap GetScrollUp(Color color)
		{
			return GetCharBitmap(_scrollUpChar, color);
		}

		public Bitmap GetScrollDown(Color color)
		{
			return GetCharBitmap(_scrollDownChar, color);
		}

		public Bitmap GetScrollEnd(Color color)
		{
			return GetCharBitmap(_scrollEndChar, color);
		}

		private Bitmap GetCharBitmap(string[] charDef, Color color)
		{
			int width = 0;
			for (int y = 0; y < charDef.Length; y++)
			{
				if (charDef[y].Length > width)
				{
					width = charDef[y].Length;
				}
			}

			//Bitmap bmp = new Bitmap(width, 30);
			Bitmap bmp = new Bitmap(width, 45);
			for (int y=0; y<charDef.Length; y++)
			{
				for (int x = 0; x < charDef[y].Length; x++)
				{
					if (charDef[y][x] != ' ')
					{
						bmp.SetPixel(x, y, color);
					}
				}
			}

			// scale
			//bmp = new Bitmap(bmp, new Size(10, 19));

			return bmp;
		}

		private static readonly string[] _bellChar =
		{
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"        ************* ",
			"      *****************",
			"     ***             ***",
			"    **                 **",
			"    **                 **",
			"   **                   **",
			"   **                   **",
			"   **                   **",
			"  **                     **",
			"  **                     **",
			"  *************************",
			"  *************************",
			"        **         **",
			"        **         **",
			"        **         **",
			"        **         **",
			"    ******         ******",
			"    ******         ******",
		};

		private static readonly string[] _wruChar =
		{
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"         **********",
			"           ******",
			"            ****",
			"             ** ",
			"             **",
			"  *          **          *",
			"  *          **          *",
			"  **         **         **",
			"  ****       **       ****",
			"  ************************",
			"  ************************",
			"  ****       **       ****",
			"  **         **         **",
			"  *          **          *",
			"  *          **          *",
			"             **",
			"             ** ",
			"            ****",
			"           ******",
			"         **********",
		};

		private static readonly string[] _shiftF =
		{
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"  *********************",
			"  *********************",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  *********************",
			"  *********************",
		};

		private static readonly string[] _shiftG =
		{
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"  *********************",
			"  *********************",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  *********************",
			"  *********************",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  **                 **",
			"  *********************",
			"  *********************",
		};

		private static readonly string[] _shiftH =
		{
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"  *********************",
			"  *********************",
			"  **               ****",
			"  **              ** **",
			"  **             **  **",
			"  **            **   **",
			"  **           **    **",
			"  **          **     **",
			"  **         **      **",
			"  **        **       **",
			"  **       **        **",
			"  **      **         **",
			"  **     **          **",
			"  **    **           **",
			"  **   **            **",
			"  **  **             **",
			"  ** **              **",
			"  ****               **",
			"  *********************",
			"  *********************",
		};


		private static readonly string[] _scrollStartChar =
		{
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"     ************",
			"",
			"          **",
			"         *  *",
			"        *    *",
			"       *      *",
			"      *        *",
			"     *          *",
			"",
		};

		private static readonly string[] _scrollUpChar =
		{
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"          **",
			"         *  *",
			"        *    *",
			"       *      *",
			"      *        *",
			"     *          *",
			"",
		};

		private static readonly string[] _scrollDownChar =
		{
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"     *          *",
			"      *        *",
			"       *      *",
			"        *    *",
			"         *  *",
			"          **",
			"",
		};

		private static readonly string[] _scrollEndChar =
		{
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"     *          *",
			"      *        *",
			"       *      *",
			"        *    *",
			"         *  *",
			"          **",
			"",
			"     ************"
		};
	}

	class SpecialCharacterItem
	{
		public char Code { get; set; }

		public Bitmap Bitmap { get; set; }

		public Color Color { get; set; }
	}
}
