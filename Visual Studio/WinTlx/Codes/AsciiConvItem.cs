using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	class AsciiConvItem
	{
		public int FromAscii { get; set; }

		public int ToAscii { get; set; }

		public int? NewAsciiOffset { get; set; }

		public string NewAscii { get; set; }

		public AsciiConvItem(int fromAscii, int? toAscii, char newAsciiOffset)
		{
			FromAscii = fromAscii;
			ToAscii = toAscii != null ? toAscii.Value : fromAscii;
			NewAsciiOffset = (int)newAsciiOffset;
		}

		public AsciiConvItem(int fromAscii, char newAscii)
		{
			FromAscii = fromAscii;
			ToAscii = fromAscii;
			NewAscii = newAscii.ToString();
		}

		public AsciiConvItem(int fromAscii, string newAscii)
		{
			FromAscii = fromAscii;
			ToAscii = fromAscii;
			NewAscii = newAscii;
		}

		public string GetCodeInRange(char code)
		{
			if (code < FromAscii || code > ToAscii)
			{
				return null;
			}

			if (NewAsciiOffset != null)
			{
				int offset = (int)NewAsciiOffset - (int)FromAscii;
				return ((char)(code + offset)).ToString();
			}
			else
			{
				return NewAscii;
			}
		}

		public override string ToString()
		{
			return $"{FromAscii} {ToAscii} {NewAsciiOffset} {NewAscii}";
		}
	}
}
