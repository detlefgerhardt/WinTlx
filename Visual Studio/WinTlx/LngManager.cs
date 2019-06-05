using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx
{
	class Lng
	{
		public string Name { get; set; }

		public List<LngItem> LngList { get; set; }
	}

	class LngItem
	{
		public string Section { get; set; }

		public string Key { get; set; }

		public string Text { get; set; }
	}

	class LngManager
	{
	}
}
