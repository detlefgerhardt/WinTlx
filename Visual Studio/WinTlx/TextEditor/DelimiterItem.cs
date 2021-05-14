using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.TextEditor
{
	class DelimiterItem
	{
		public char Char { get; set; }

		public bool WrapBefore { get; set; }

		public DelimiterItem(char chr, bool wrapBefore)
		{
			Char = chr;
			WrapBefore = wrapBefore;
		}
	}
}
