using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WinTlx.Favorites
{
	[DataContract(Namespace = "")]
	class FavoriteItem
	{
		[DataMember]
		public string Number { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public int DirectDial { get; set; }

		public bool IsEqual(FavoriteItem item)
		{
			return item.Number == Number && item.Name == Name && item.Address == Address && item.Port == Port && item.DirectDial == DirectDial;
		}

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(Number);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(Number) && string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Address) && Port == 0 && DirectDial == 0;
			}
		}

		public FavoriteItem()
		{
		}

		public FavoriteItem(string number, string name)
		{
			Number = number;
			Name = name;
			Address = null;
		}

		public override string ToString()
		{
			return $"{Number} {Name} {Address} {Port} {DirectDial}";
		}
	}

	class FavoritesItemSorter : IComparer<FavoriteItem>
	{
		public enum SortIndex { Number, Name }

		private SortIndex _colIndex;
		private bool _ascending;

		public FavoritesItemSorter(SortIndex columnIndex, bool ascending)
		{
			_colIndex = columnIndex;
			_ascending = ascending;
		}

		public int Compare(FavoriteItem item1, FavoriteItem item2)
		{

			switch (_colIndex)
			{
				case SortIndex.Number:
					string number1 = FormatItem(item1.Number);
					string number2 = FormatItem(item2.Number);
					if (_ascending)
					{
						return number1.CompareTo(number2);
					}
					else
					{
						return number2.CompareTo(number1);
					}
				case SortIndex.Name:
					string name1 = FormatItem(item1.Name);
					string name2 = FormatItem(item2.Name);
					if (_ascending)
					{
						return name1.CompareTo(name2);
					}
					else
					{
						return name2.CompareTo(name1);
					}
			}
			return 0;
		}

		private string FormatItem(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				if (_ascending) return "x";
				return "!";
			}
			return value.ToLower();
		}
	}
}
