using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	class UpLoItem
	{
		public char Upper { get; set; }

		public char Lower { get; set; }

		public char Latin { get; set; }

		public UpLoItem(char upper, char lower, char latin)
		{
			Upper = upper;
			Lower = lower;
			Latin = latin;
		}
	}
}
