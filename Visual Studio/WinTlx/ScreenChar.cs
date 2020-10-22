using System.Collections.Generic;
using System.Drawing;

namespace WinTlx
{
	public enum CharAttributes { Message, Send, Recv }

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
					//if (value!=' ')
					//{
					//	Debug.Write("");
					//}
					Chars.Add(value);
				}
			}
		}

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
			Chars = new List<char>();
			Char = ' ';
			Attr = CharAttributes.Message;
		}

		public ScreenChar(char chr)
		{
			Chars = new List<char>();
			Char = chr;
			Attr = CharAttributes.Message;
		}

		public ScreenChar(char chr, CharAttributes attr)
		{
			Chars = new List<char>();
			Char = chr;
			Attr = attr;
		}

		public override string ToString()
		{
			return Char.ToString();
		}
	}
}
