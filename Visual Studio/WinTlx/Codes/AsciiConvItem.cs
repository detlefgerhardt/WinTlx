using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	class AsciiConvItem
	{
		public byte FromAscii { get; set; }

		public byte ToAscii { get; set; }

		public byte? NewAsciiOffset { get; set; }

		public string NewAscii { get; set; }

		public AsciiConvItem(byte fromAscii, byte? toAscii, char newAsciiOffset)
		{
			FromAscii = fromAscii;
			ToAscii = toAscii != null ? toAscii.Value : fromAscii;
			NewAsciiOffset = (byte)newAsciiOffset;
		}

		public AsciiConvItem(byte fromAscii, char newAscii)
		{
			FromAscii = fromAscii;
			ToAscii = fromAscii;
			NewAscii = newAscii.ToString();
		}

		public AsciiConvItem(byte fromAscii, string newAscii)
		{
			FromAscii = fromAscii;
			ToAscii = fromAscii;
			NewAscii = newAscii;
		}

		public string GetCodeInRange(char code)
		{
			if (code<FromAscii || code>ToAscii)
			{
				return null;
			}

			if (NewAsciiOffset!=null)
			{
				int offset = (int)NewAsciiOffset - (int)FromAscii;
				return ((char)(code + offset)).ToString();
			}
			else
			{
				return NewAscii;
			}
		}
	}
}
