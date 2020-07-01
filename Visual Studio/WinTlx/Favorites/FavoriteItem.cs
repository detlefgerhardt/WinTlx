using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Favorites
{
	class FavoriteItem
	{
		public int Number { get; set; }

		public string Name { get; set; }

		public FavoriteItem(int number, string name)
		{
			Number = number;
			Name = name;
		}
	}
}
