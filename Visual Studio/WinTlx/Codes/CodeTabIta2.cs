using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	class CodeTabIta2 : ICodeTab
	{
		public bool HasThirdLevel => false;

		public AsciiConvItem[] AsciiTab => new AsciiConvItem[]
		{
			new AsciiConvItem(0x00, CodeManager.ASC_CODE32),
			new AsciiConvItem(0x05, CodeManager.ASC_WRU),
			new AsciiConvItem(0x07, CodeManager.ASC_BEL),
			new AsciiConvItem(0x0A, CodeManager.ASC_LF),
			new AsciiConvItem(0x0D, CodeManager.ASC_CR),
			new AsciiConvItem(0x10, CodeManager.ASC_SHIFTF),
			new AsciiConvItem(0x11, CodeManager.ASC_SHIFTG),
			new AsciiConvItem(0x12, CodeManager.ASC_SHIFTH),
			new AsciiConvItem(0x1E, CodeManager.ASC_LTRS),
			new AsciiConvItem(0x1F, CodeManager.ASC_FIGS),
			new AsciiConvItem(0x20, ' '),
			new AsciiConvItem(0x22, "''"), // "
			new AsciiConvItem(0x25, "o/o"), // %
			new AsciiConvItem(0x26, '+'), // &
			new AsciiConvItem(0x27, '\''),
			new AsciiConvItem(0x28, '('),
			new AsciiConvItem(0x29, ')'),
			new AsciiConvItem(0x2B, '+'),
			new AsciiConvItem(0x2C, ','),
			new AsciiConvItem(0x2D, '-'),
			new AsciiConvItem(0x2E, '.'),
			new AsciiConvItem(0x2F, '/'),
			new AsciiConvItem(0x30, 0x39, '0'), // 0..9
			new AsciiConvItem(0x3A, ':'),
			new AsciiConvItem(0x3C, "(."), // <
			new AsciiConvItem(0x3D, '='),
			new AsciiConvItem(0x3E, ".)"), // >
			new AsciiConvItem(0x3F, '?'),
			new AsciiConvItem(0x41, 0x5A, 'a'), // A..Z
			new AsciiConvItem(0x5B, "(:"), // [
			//new AsciiConvItem(0x5C, ""), // \
			new AsciiConvItem(0x5D, ":)"), // ]
			//new AsciiConvItem(0x5E, ""), // ^
			new AsciiConvItem(0x5F, ' '), // _
			new AsciiConvItem(0x60, '\''), // `
			new AsciiConvItem(0x61, 0x7A, 'a'), // a..z
			new AsciiConvItem(0x7B, "(,"), // {
			new AsciiConvItem(0x7C, '/'), // |
			new AsciiConvItem(0x7D, ",)"), // }
			new AsciiConvItem(0x7E, '-'), // ~
		};

		public CodeItem[] CodeTab => new CodeItem[32]
		{
			new CodeItem(
				0x00,
				CodeManager.ASC_CODE32, "COD32"
			),
			new CodeItem(
				0x01,
				't', "t",
				'5', "5"
			),
			new CodeItem(
				0x02,
				'\r', "WR" // CR
			),
			new CodeItem(
				0x03,
				'o', "o",
				'9', "9"
			),
			new CodeItem(
				0x04,
				' ', "ZWR" // SP
			),
			new CodeItem(
				0x05,
				'h', "h",
				CodeManager.ASC_SHIFTH, CodeManager.ASC_SHIFTH.ToString() //  '#'
			),
			new CodeItem(
				0x06,
				'n', "n",
				',', ","
			),
			new CodeItem(
				0x07,
				'm', "m",
				'.', "."
			),
			new CodeItem(
				0x08,
				'\n', "ZL" // NL
			),
			new CodeItem(
				0x09,
				'l', "l",
				')', ")"
			),
			new CodeItem(
				0x0A,
				'r', "r",
				'4', "4"
			),
			new CodeItem(
				0x0B,
				'g', "g",
				CodeManager.ASC_SHIFTG, CodeManager.ASC_SHIFTG.ToString() // '&' or @
			),
			new CodeItem(
				0x0C,
				'i', "i",
				'8', "8"
			),
			new CodeItem(
				0x0D,
				'p', "p",
				'0', "0"
			),
			new CodeItem(
				0x0E,
				'c', "c",
				':', ":"
			),
			new CodeItem(
				0x0F,
				'v', "v",
				'=', "="
			),
			new CodeItem(
				0x10,
				'e', "e",
				'3', "3"
			),
			new CodeItem(
				0x11,
				'z', "z",
				'+', "+"
			),
			new CodeItem(
				0x12,
				'd', "d",
				CodeManager.ASC_WRU, "WRU" // $ or Pound
			),
			new CodeItem(
				0x13,
				'b', "b",
				'?', "?"
			),
			new CodeItem(
				0x14,
				's', "s",
				'\'', "'" // ASC_BEL, "BEL"
			),
			new CodeItem(
				0x15,
				'y', "y",
				'6', "6"
			),
			new CodeItem(
				0x16,
				'f', "f",
				CodeManager.ASC_SHIFTF, CodeManager.ASC_SHIFTF.ToString() // '!' or %
			),
			new CodeItem(
				0x17,
				'x', "x",
				'/', "/"
			),
			new CodeItem(
				0x18,
				'a', "a",
				'-', "-"
			),
			new CodeItem(
				0x19,
				'w', "w",
				'2', "2"
			),
			new CodeItem(
				0x1A,
				'j', "j",
				CodeManager.ASC_BEL, "KL" // BEL
			),
			new CodeItem(
				0x1B,
				CodeManager.ASC_FIGS, "Zi" // Fig
			),
			new CodeItem(
				0x1C,
				'u', "u",
				'7', "7"
			),
			new CodeItem(
				0x1D,
				'q', "q",
				'1', "1"
			),
			new CodeItem(
				0x1E,
				'k', "k",
				'(', "("
			),
			new CodeItem(
				0x1F,
				CodeManager.ASC_LTRS, "Bu" // Ltr
			)
		};
	}
}
