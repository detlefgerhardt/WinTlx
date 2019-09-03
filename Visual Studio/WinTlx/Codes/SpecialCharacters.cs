using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	class SpecialCharacters
	{
		private static SpecialCharacters instance;

		public static SpecialCharacters Instance => instance ?? (instance = new SpecialCharacters());

		private SpecialCharacters()
		{
		}

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

		private static string[] _bellChar =
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

		private static string[] _wruChar =
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

		private static string[] _shiftF =
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

		private static string[] _shiftG =
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

		private static string[] _shiftH =
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


		private static string[] _scrollStartChar =
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

		private static string[] _scrollUpChar =
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

		private static string[] _scrollDownChar =
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

		private static string[] _scrollEndChar =
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
}
