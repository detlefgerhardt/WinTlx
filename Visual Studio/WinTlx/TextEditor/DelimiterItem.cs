using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.TextEditor
{
	public enum WrapMode { Before, After, Both };

	class DelimiterItem
	{
		public char Char { get; set; }

		public WrapMode WrapMode { get; set; }
		// true = before

		public DelimiterItem(char chr, WrapMode wrapMode)
		{
			Char = chr;
			WrapMode = wrapMode;
		}
	}
}
