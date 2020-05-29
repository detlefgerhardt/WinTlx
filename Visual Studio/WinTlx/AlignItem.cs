using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx
{
	class AlignItem
	{
		public string Text { get; set; }

		public char Delimiter { get; set; }

		public AlignItem(string text, char delim)
		{
			Text = text;
			Delimiter = delim;
		}

		public string TextWithDelim => Text + Delimiter;

		public override string ToString()
		{
			return $"{Text} '{Delimiter}'";
		}
	}
}
