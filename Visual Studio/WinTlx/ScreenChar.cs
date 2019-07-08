using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx
{
	public enum CharAttributes { Message, Send, Recv }

	class ScreenChar
	{
		public char Char { get; set; }

		public CharAttributes Attr { get; set; }

		public Color AttrColor
		{
			get
			{
				switch(Attr)
				{
					default:
					case CharAttributes.Message:
						return Color.Blue;
					case CharAttributes.Send:
						return Color.Red;
					case CharAttributes.Recv:
						return Color.Black;
				}
			}
		}

		public ScreenChar()
		{
			Char = ' ';
			Attr = CharAttributes.Message;
		}

		public ScreenChar(char chr)
		{
			Char = chr;
			Attr = CharAttributes.Message;
		}

		public ScreenChar(char chr, CharAttributes attr)
		{
			Char = chr;
			Attr = attr;
		}

	}
}
