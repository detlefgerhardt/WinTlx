using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx
{
	class PeerTypeItem
	{
		public int PeerCode { get; set; }

		public string ShortDesc { get; set; }

		public string Description { get; set; }

		public PeerTypeItem(int code, string shortDesc, string desc)
		{
			PeerCode = code;
			ShortDesc = shortDesc;
			Description = desc;
		}
	}
}
