using System.Collections.Generic;
using System.Drawing;

namespace WinTlx
{
	public enum CharAttributes { Message, TechMessage, Send, Recv, RecvEmpty }

	class ScreenChar
	{
		public List<char> Chars { get; set; }

		public char Char
		{
			get
			{
				if (Chars.Count==0)
				{
					return ' ';
				}
				return Chars[Chars.Count - 1];
			}
			set
			{
				if (Chars.Count == 1 && Chars[0] == ' ')
				{
					Chars[0] = value;
				}
				else
				{
					Chars.Add(value);
				}
			}
		}

		//public char Char { get; set; }

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
					case CharAttributes.TechMessage:
						return Color.Violet;
					case CharAttributes.Send:
						return Color.Red;
					case CharAttributes.Recv:
						return Color.Black;
				}
			}
		}

		public int AckCount { get; set; }

		public ScreenChar()
		{
			Chars = new List<char>();
			Char = ' ';
			Attr = CharAttributes.Message;
			AckCount = 0;
		}

		public ScreenChar(char chr)
		{
			Chars = new List<char>();
			Char = chr;
			Attr = CharAttributes.Message;
		}

		public ScreenChar(char chr, CharAttributes attr, int ackCount)
		{
			Chars = new List<char>();
			Char = chr;
			Attr = attr;
			AckCount = ackCount;
		}

		public override string ToString()
		{
			return $"{Char} {Attr} {AckCount}";
		}
	}
}
