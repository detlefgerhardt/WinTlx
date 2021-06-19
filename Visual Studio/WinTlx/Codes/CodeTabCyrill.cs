using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	class CodeTabCyrill : ICodeTab
	{
		public bool HasThirdLevel => true;

		public AsciiConvItem[] AsciiTab => new AsciiConvItem[]
		{
			new AsciiConvItem(0x00, CodeManager.ASC_NUL),
			new AsciiConvItem(0x05, CodeManager.ASC_WRU),
			new AsciiConvItem(0x07, CodeManager.ASC_BEL),
			new AsciiConvItem(0x0A, CodeManager.ASC_LF),
			new AsciiConvItem(0x0D, CodeManager.ASC_CR),
			new AsciiConvItem(0x1E, CodeManager.ASC_LTRS),
			new AsciiConvItem(0x1F, CodeManager.ASC_FIGS),
			new AsciiConvItem(0x20, ' '),
			new AsciiConvItem(0x21, '!'),
			new AsciiConvItem(0x22, '"'),
			new AsciiConvItem(0x23, '#'),
			new AsciiConvItem(0x24, '$'),
			new AsciiConvItem(0x26, '&'),
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
			new AsciiConvItem(0x40, '@'),
			new AsciiConvItem(0x5B, "(:"), // [
			new AsciiConvItem(0x5D, ":)"), // ]
			new AsciiConvItem(0x5F, ' '), // _
			new AsciiConvItem(0x60, '\''), // `
			new AsciiConvItem(0x61, 0x7A, 'a'), // a..z
			new AsciiConvItem(0x7B, "(,"), // {
			new AsciiConvItem(0x7C, '/'),
			new AsciiConvItem(0x7D, ",)"), // }
			new AsciiConvItem(0x7E, '-'), // ~
			new AsciiConvItem(0x410, '\u0410'), // A
			new AsciiConvItem(0x430, '\u0410'), // a
			new AsciiConvItem(0x411, '\u0411'), // B
			new AsciiConvItem(0x431, '\u0411'), // b
			new AsciiConvItem(0x426, '\u0426'), // C
			new AsciiConvItem(0x446, '\u0426'), // c
			new AsciiConvItem(0x414, '\u0414'), // D
			new AsciiConvItem(0x434, '\u0414'), // d
			new AsciiConvItem(0x415, '\u0415'), // E
			new AsciiConvItem(0x435, '\u0415'), // e
			new AsciiConvItem(0x424, '\u0424'), // F
			new AsciiConvItem(0x444, '\u0424'), // f
			new AsciiConvItem(0x413, '\u0413'), // G
			new AsciiConvItem(0x433, '\u0413'), // g
			new AsciiConvItem(0x425, '\u0425'), // H
			new AsciiConvItem(0x445, '\u0425'), // h
			new AsciiConvItem(0x418, '\u0418'), // I
			new AsciiConvItem(0x438, '\u0418'), // i
			new AsciiConvItem(0x419, '\u0419'), // J
			new AsciiConvItem(0x439, '\u0419'), // j
			new AsciiConvItem(0x41A, '\u041A'), // K
			new AsciiConvItem(0x43A, '\u041A'), // k
			new AsciiConvItem(0x41B, '\u041B'), // L
			new AsciiConvItem(0x43B, '\u041B'), // l
			new AsciiConvItem(0x41C, '\u041C'), // M
			new AsciiConvItem(0x43C, '\u041C'), // m
			new AsciiConvItem(0x41D, '\u041D'), // N
			new AsciiConvItem(0x43D, '\u041D'), // n
			new AsciiConvItem(0x41E, '\u041E'), // O
			new AsciiConvItem(0x43E, '\u041E'), // o
			new AsciiConvItem(0x41F, '\u041F'), // P
			new AsciiConvItem(0x43F, '\u041F'), // p
			new AsciiConvItem(0x42F, '\u042F'), // Q
			new AsciiConvItem(0x44F, '\u042F'), // q
			new AsciiConvItem(0x420, '\u0420'), // R
			new AsciiConvItem(0x440, '\u0420'), // r
			new AsciiConvItem(0x421, '\u0421'), // S
			new AsciiConvItem(0x441, '\u0421'), // s
			new AsciiConvItem(0x422, '\u0422'), // T
			new AsciiConvItem(0x442, '\u0422'), // t
			new AsciiConvItem(0x423, '\u0423'), // U
			new AsciiConvItem(0x443, '\u0423'), // u
			new AsciiConvItem(0x416, '\u0416'), // V
			new AsciiConvItem(0x436, '\u0436'), // v
			new AsciiConvItem(0x412, '\u0412'), // W
			new AsciiConvItem(0x432, '\u0412'), // w
			new AsciiConvItem(0x42C, '\u042C'), // X
			new AsciiConvItem(0x44C, '\u042C'), // x
			new AsciiConvItem(0x42B, '\u042B'), // Y
			new AsciiConvItem(0x44B, '\u042B'), // y
			new AsciiConvItem(0x417, '\u0417'), // Z
			new AsciiConvItem(0x437, '\u0417'), // z
			new AsciiConvItem(0x429, '\u0429'),
			new AsciiConvItem(0x449, '\u0429'),
			new AsciiConvItem(0x42D, '\u042D'),
			new AsciiConvItem(0x44D, '\u044D'),
			new AsciiConvItem(0x427, '\u0427'),
			new AsciiConvItem(0x447, '\u0427'),
			new AsciiConvItem(0x42E, '\u042E'), // ü
			new AsciiConvItem(0x428, '\u0428'), // Ö
		};

		public CodeItem[] CodeTab => new CodeItem[32]
		{
			new CodeItem(
				0x00,
				CodeManager.ASC_NUL, "PYC"
			),
			new CodeItem(
				0x01,
				't', "t",
				'5', "5",
				'\u0422', "\u0422" // TE
			),
			new CodeItem(
				0x02,
				'\r', "WR"
			),
			new CodeItem(
				0x03,
				'o', "o",
				'9', "9",
				'\u041E', "\u041E" // O
			),
			new CodeItem(
				0x04,
				' ', "ZWR"
			),
			new CodeItem(
				0x05,
				'h', "h",
				'\u0429', "\u0429", // SHCHA
				'\u0425', "\u0425"  // HA
			),
			new CodeItem(
				0x06,
				'n', "n",
				',', ",",
				'\u041D', "\u041D" // EN
			),
			new CodeItem(
				0x07,
				'm', "m",
				'.', ".",
				'\u041C', "\u041C" // EM
			),
			new CodeItem(
				0x08,
				'\n', "ZL"
			),
			new CodeItem(
				0x09,
				'l', "l",
				')', ")",
				'\u041B', "\u041B" // EL
			),
			new CodeItem(
				0x0A,
				'r', "r",
				'4', "4",
				'\u0420', "\u0420" // ER
			),
			new CodeItem(
				0x0B,
				'g', "g",
				'\u0428', "\u0428", // SHA
				'\u0413', "\u0413"  // GHE
			),
			new CodeItem(
				0x0C,
				'i', "i",
				'8', "8",
				'\u0418', "\u0418" // I
			),
			new CodeItem(
				0x0D,
				'p', "p",
				'0', "0",
				'\u041F', "\u041F" // PE
			),
			new CodeItem(
				0x0E,
				'c', "c",
				':', ":",
				'\u0426', "\u0426" // TSE
			),
			new CodeItem(
				0x0F,
				'v', "v",
				'=', "=",
				'\u0416', "\u0416" // ZHE
			),
			new CodeItem(
				0x10,
				'e', "e",
				'3', "3",
				'\u0415', "\u0415" // IE
			),
			new CodeItem(
				0x11,
				'z', "z",
				'+', "+",
				'\u0417', "\u0417" // ZE
			),
			new CodeItem(
				0x12,
				'd', "d",
				CodeManager.ASC_WRU, "WRU",
				'\u0414', "\u0414" // DE
			),
			new CodeItem(
				0x13,
				'b', "b",
				'?', "?",
				'\u0411', "\u0411" // BE
			),
			new CodeItem(
				0x14,
				's', "s",
				'\'', "'",
				'\u0421', "\u0421" // ES
			),
			new CodeItem(
				0x15,
				'y', "y",
				'6', "6",
				'\u042B', "\u042B" // YERU
			),
			new CodeItem(
				0x16,
				'f', "f",
				'\u042D', "\u042D", // E
				'\u0424', "\u0424"  // EF
			),
			new CodeItem(
				0x17,
				'x', "x",
				'/', "/",
				'\u042C', "\u042C" // SOFT SIGN?
			),
			new CodeItem(
				0x18,
				'a', "a",
				'-', "-",
				'\u0410', "\u0410" // A
			),
			new CodeItem(
				0x19,
				'w', "w",
				'2', "2",
				'\u0412', "\u0412" // WE
			),
			new CodeItem(
				0x1A,
				'j', "j",
				//CodeManager.ASC_BEL, "KL",
				'\u042E', "\u042E", // YU
				'\u0419', "\u0419" // SHORT I
			),
			new CodeItem(
				0x1B,
				CodeManager.ASC_FIGS, "1..." // Fig
			),
			new CodeItem(
				0x1C,
				'u', "u",
				'7', "7",
				'\u0423', "\u0423" // U
			),
			new CodeItem(
				0x1D,
				'q', "q",
				'1', "1",
				'\u042F', "\u042F" // YA
			),
			new CodeItem(
				0x1E,
				'k', "k",
				'(', "(",
				'\u041A', "\u041A" // KA
			),
			new CodeItem(
				0x1F,
				CodeManager.ASC_LTRS, "LAT" // Ltr
			)
		};

		public static string ToUpperString(string str)
		{
			string newStr = "";
			foreach(char chr in str)
			{
				newStr += ToUpper(chr);
			}
			return newStr;
		}

		public static char ToUpper(char chr)
		{
			foreach(UpLoItem item in _cyrillTab)
			{
				if (item.Lower == chr || item.Upper == chr) return item.Upper;
			}
			return chr;
		}

		public static string ToLowerString(string str)
		{
			string newStr = "";
			foreach (char chr in str)
			{
				newStr += ToLower(chr);
			}
			return newStr;
		}

		public static char ToLower(char chr)
		{
			foreach (UpLoItem item in _cyrillTab)
			{
				if (item.Lower == chr || item.Upper == chr) return item.Lower;
			}
			return chr;
		}

		private static UpLoItem[] _cyrillTab = new UpLoItem[]
		{
			new UpLoItem('\u0410','\u0430', 'a'),
			new UpLoItem('\u0411','\u0431', 'b'),
			new UpLoItem('\u0426','\u0446', 'c'),
			new UpLoItem('\u0414','\u0434', 'd'),
			new UpLoItem('\u0415','\u0435', 'e'),
			new UpLoItem('\u0424','\u0444', 'f'),
			new UpLoItem('\u0413','\u0433', 'g'),
			new UpLoItem('\u0425','\u0445', 'h'),
			new UpLoItem('\u0418','\u0438', 'i'),
			new UpLoItem('\u0419','\u0439', 'j'),
			new UpLoItem('\u041A','\u043A', 'k'),
			new UpLoItem('\u041B','\u043B', 'l'),
			new UpLoItem('\u041C','\u043C', 'm'),
			new UpLoItem('\u041D','\u043D', 'n'),
			new UpLoItem('\u041E','\u043E', 'o'),
			new UpLoItem('\u041F','\u043F', 'p'),
			new UpLoItem('\u042F','\u044F', 'q'),
			new UpLoItem('\u0420','\u0440', 'r'),
			new UpLoItem('\u0421','\u0441', 's'),
			new UpLoItem('\u0422','\u0442', 't'),
			new UpLoItem('\u0423','\u0443', 'u'),
			new UpLoItem('\u0416','\u0436', 'v'),
			new UpLoItem('\u0412','\u0432', 'w'),
			new UpLoItem('\u042C','\u044C', 'x'),
			new UpLoItem('\u042B','\u044B', 'y'),
			new UpLoItem('\u0417','\u0437', 'z'),
			new UpLoItem('\u042E','\u042E', 'ü'),
			new UpLoItem('\u0429','\u0449', 'ß'),
			new UpLoItem('\u042D','\u044D', 'ö'),
			new UpLoItem('\u0427','\u0447', 'ä'),
		};

		public static char? CyrillicKeyToUnicode(char key)
		{
			if (key >= 'a' && key <= 'z') key = char.ToUpper(key);

			switch (key)
			{
				case 'A':
					return '\u0410'; // A
				case 'a':
					return '\u0430'; // a
				case 'B':
					return '\u0411'; // BE
				case 'b':
					return '\u0431'; // be
				case 'C':
					return '\u0426'; // TSE
				case 'c':
					return '\u0446'; // tse
				case 'D':
					return '\u0414'; // DE
				case 'd':
					return '\u0434'; // de
				case 'E':
					return '\u0415'; // IE
				case 'e':
					return '\u0435'; // ie
				case 'F':
					return '\u0424'; // EF
				case 'f':
					return '\u0444'; // ef
				case 'G':
					return '\u0413'; // GHE
				case 'g':
					return '\u0433'; // ghe
				case 'H':
					return '\u0425'; // HA
				case 'h':
					return '\u0445'; // ha
				case 'I':
					return '\u0418'; // I
				case 'i':
					return '\u0438'; // i
				case 'J':
					return '\u0419'; // short I
				case 'j':
					return '\u0439'; // short i
				case 'K':
					return '\u041A'; // KA
				case 'k':
					return '\u043A'; // ka
				case 'L':
					return '\u041B'; // EL
				case 'l':
					return '\u043B'; // el
				case 'M':
					return '\u041C'; // EM
				case 'm':
					return '\u043C'; // em
				case 'N':
					return '\u041D'; // EN
				case 'n':
					return '\u043D'; // en
				case 'O':
					return '\u041E'; // O
				case 'o':
					return '\u043E'; // o
				case 'P':
					return '\u041F'; // PE
				case 'p':
					return '\u043F'; // pe
				case 'Q':
					return '\u042F'; // YA
				case 'q':
					return '\u044F'; // ya
				case 'R':
					return '\u0420'; // ER
				case 'r':
					return '\u0440'; // er
				case 'S':
					return '\u0421'; // ES
				case 's':
					return '\u0441'; // es
				case 'T':
					return '\u0422'; // TE
				case 't':
					return '\u0442'; // te
				case 'U':
					return '\u0423'; // U
				case 'u':
					return '\u0443'; // u
				case 'V':
					return '\u0416'; // ZHE
				case 'v':
					return '\u0436'; // zhe
				case 'W':
					return '\u0412'; // VE
				case 'w':
					return '\u0432'; // ve
				case 'X':
					return '\u042C'; // SOFT SIGN
				case 'x':
					return '\u044C'; // soft sign
				case 'Y':
					return '\u042B'; // YERU
				case 'y':
					return '\u044B'; // yeru
				case 'Z':
					return '\u0417'; // ZE
				case 'z':
					return '\u0437'; // ze
				case 'Ü':
					return '\u044E'; // yu
				case 'ü':
					return '\u042E'; // YU
				case 'Ö':
					//return '\u042D'; // E
					return '\u0428'; // SHA
				case 'ö':
					//return '\u044D'; // e
					return '\u042D'; // E
				case 'Ä':
					//return '\u0447'; // che
					return '\u0429'; // SHCHA
				case 'ä':
					return '\u0427'; // CHE
				default:
					return key;
			}
		}

	}

}
