using System.Collections.Generic;
using System.Linq;

namespace WinTlx.Codes
{
	public static class CodeManager
	{
		// F=Quadrat ohne Inhalt., G=Quadrat mit Querstrich, H=Quadrat mit Schrägstrich

		// special ASCII codes
		public const char ASC_INV = '~'; // replace invalid baudot character
		public const char ASC_NUL = '\x00';
		public const char ASC_WRU = '\x05'; // = enquire
		public const char ASC_BEL = '\x07';
		public const char ASC_HEREIS = '\x09';
		public const char ASC_LF = '\x0A';
		public const char ASC_CR = '\x0D';
		public const char ASC_SHIFTF = '\x10';
		public const char ASC_SHIFTG = '\x11';
		public const char ASC_SHIFTH = '\x12';
		public const char ASC_LTRS = '\x1E';
		public const char ASC_FIGS = '\x1F';

		// special ITA2 codes
		public const byte BAU_NUL = 0x00;
		public const byte BAU_CR = 0x02;
		public const byte BAU_LF = 0x08;
		public const byte BAU_WRU = 0x12;
		public const byte BAU_BEL = 0x1A;
		public const byte BAU_FIGS = 0x1B;
		public const byte BAU_LTRS = 0x1F;

		public const byte BAU_SHIFTF = 0x16;
		public const byte BAU_SHIFTG = 0x0B;
		public const byte BAU_SHIFTH = 0x05;

		public enum SendRecv { Send, Recv };

		public static string BaudotStringToAscii(byte[] baudotData, ref ShiftStates shiftState, CodeSets codeSet, SendRecv sendRecv)
		{
			string asciiStr = "";
			for (int i = 0; i < baudotData.Length; i++)
			{
				byte baudotChr = baudotData[i];
				if (baudotChr == BAU_LTRS)
				{
					shiftState = ShiftStates.Ltr;
				}
				else if (baudotChr == BAU_FIGS)
				{
					shiftState = ShiftStates.Figs;
				}
				else
				{
					char asciiChr = BaudotCharToAscii(baudotData[i], shiftState, codeSet, sendRecv);
					asciiStr += asciiChr;
				}
			}
			return asciiStr;
		}

		public static char BaudotCharToAscii(byte baudotCode, ShiftStates shiftState, CodeSets codeSet, SendRecv sendRecv)
		{
			if (baudotCode > 0x1F)
			{
				return (char)ASC_INV;
			}

			char asciiChr = _codeTab[baudotCode].GetCode(shiftState, codeSet);
			/*
			if (asciiChr == ASC_INV && sendRecv==SendRecv.Recv)
			{
				// received an invalid code: try extended code page
				asciiChr = _codeTab[baudotCode].GetCode(shiftState, CodeSets.ITA2EXT);
			}
			*/
			return asciiChr;
		}

		public static string BaudotCodeToPuncherText(byte baudotCode, ShiftStates shiftState, CodeSets codeSet)
		{
			if (baudotCode > 0x1F)
			{
				return ASC_INV.ToString();
			}
			return _codeTab[baudotCode].GetName(shiftState, codeSet);
		}

		public static byte[] AsciiStringToBaudot(string asciiStr, ref ShiftStates shiftState, CodeSets codeSet)
		{
			byte[] baudotData = new byte[0];
			for (int i = 0; i < asciiStr.Length; i++)
			{
				string telexData = AsciiCharToTelex(asciiStr[i], codeSet);
				byte[] data = TelexStringToBaudot(telexData, ref shiftState, codeSet);
				baudotData = baudotData.Concat(data).ToArray();
			}
			return baudotData;
		}

		/// <summary>
		/// convert ASCII string to printable characters and replacement characters
		/// </summary>
		/// <param name="asciiStr"></param>
		/// <returns></returns>
		public static string AsciiStringToTelex(string asciiStr, CodeSets codeSet)
		{
			string telexStr = "";
			for (int i = 0; i < asciiStr.Length; i++)
			{
				telexStr += AsciiCharToTelex(asciiStr[i], codeSet);
			}
			return telexStr;
		}

		/// <summary>
		/// convert ASCII character to baudot printable character and replacement character
		/// </summary>
		/// <param name="asciiChr"></param>
		/// <returns></returns>
		private static string AsciiCharToTelex(char asciiChr, CodeSets codeSet)
		{
			AsciiConvItem[] asciiToTelexTab = null;
			switch (codeSet)
			{
				default:
				case CodeSets.ITA2:
					asciiToTelexTab = _asciiIta2Tab;
					break;
				//case CodeSets.ITA2EXT:
				//	asciiToTelexTab = _asciiIta2ExtTab;
				//	break;
				case CodeSets.USTTY:
					asciiToTelexTab = _asciiUsTtyTab;
					break;
			}

			string asciiData = CodePage437ToPlainAscii(asciiChr);
			string telexData = "";
			for (int i = 0; i < asciiData.Length; i++)
			{
				foreach (AsciiConvItem convItem in asciiToTelexTab)
				{
					string ascii = convItem.GetCodeInRange(asciiData[i]);
					if (!string.IsNullOrEmpty(ascii))
					{
						telexData += ascii;
					}
				}
			}
			return telexData;
		}

		public static byte[] TelexStringToBaudot(string telexStr, ref ShiftStates shiftState, CodeSets codeSet)
		{
			byte[] buffer = new byte[0];
			for (int i = 0; i < telexStr.Length; i++)
			{
				byte[] baudotData = TelexCharToBaudotWithShift(telexStr[i], ref shiftState, codeSet);
				buffer = buffer.Concat(baudotData).ToArray();
			}
			return buffer;
		}

		public static byte[] TelexCharToBaudotWithShift(char telexChr, ref ShiftStates shiftState, CodeSets codeSet)
		{
			byte? ltrCode = FindBaudot(telexChr, ShiftStates.Ltr, codeSet);
			byte? figCode = FindBaudot(telexChr, ShiftStates.Figs, codeSet);
			byte baudCode;
			ShiftStates newShiftState;
			if (ltrCode != null && figCode != null)
			{
				baudCode = ltrCode.Value;
				newShiftState = ShiftStates.Both;
			}
			else if (ltrCode != null)
			{
				baudCode = ltrCode.Value;
				newShiftState = ShiftStates.Ltr;
			}
			else if (figCode != null)
			{
				baudCode = figCode.Value;
				newShiftState = ShiftStates.Figs;
			}
			else
			{
				return new byte[0];
			}

			return BaudotCodeToBaudotWithShift(baudCode, newShiftState, ref shiftState);
		}

		public static byte[] BaudotCodeToBaudotWithShift(byte baudCode, ShiftStates newShiftState, ref ShiftStates shiftState)
		{
			byte[] buffer = new byte[0];

			if (baudCode == BAU_LTRS)
			{
				buffer = Helper.AddByte(buffer, BAU_LTRS);
				shiftState = ShiftStates.Ltr;
				return buffer;
			}

			if (baudCode == BAU_FIGS)
			{
				buffer = Helper.AddByte(buffer, BAU_FIGS);
				shiftState = ShiftStates.Figs;
				return buffer;
			}

			if (shiftState == ShiftStates.Unknown && newShiftState == ShiftStates.Unknown)
			{
				buffer = Helper.AddByte(buffer, BAU_LTRS);
				newShiftState = ShiftStates.Ltr;
			}

			if (shiftState == ShiftStates.Unknown && newShiftState == ShiftStates.Ltr ||
				shiftState == ShiftStates.Figs && newShiftState == ShiftStates.Ltr)
			{
				buffer = Helper.AddByte(buffer, BAU_LTRS);
				buffer = Helper.AddByte(buffer, baudCode);
				shiftState = ShiftStates.Ltr;
				return buffer;
			}

			if (shiftState == ShiftStates.Unknown && newShiftState == ShiftStates.Figs ||
				shiftState == ShiftStates.Ltr && newShiftState == ShiftStates.Figs)
			{
				buffer = Helper.AddByte(buffer, BAU_FIGS);
				buffer = Helper.AddByte(buffer, baudCode);
				shiftState = ShiftStates.Figs;
				return buffer;
			}

			if (shiftState == newShiftState || newShiftState == ShiftStates.Both)
			{
				buffer = Helper.AddByte(buffer, baudCode);
				return buffer;
			}

			// should not happen
			return new byte[0];
		}

		private static byte? FindBaudot(char asciiChar, ShiftStates shiftState, CodeSets codeSet)
		{
			for (int c = 0; c < 32; c++)
			{
				char chr = _codeTab[c].GetCode(shiftState, codeSet);
				if (chr==asciiChar)
				{
					return (byte)c;
				}
			}
			return null;
		}

		/// <summary>
		/// Code page 437 (0x00-0xFF) to plain ASCII conversion (0x00-0x7F).
		/// This conversion is valid for all code pages
		/// </summary>
		/// <param name="asciiChar"></param>
		/// <returns></returns>
		private static string CodePage437ToPlainAscii(char asciiChar)
		{
			switch (asciiChar)
			{
				case 'ä':
				case 'Ä':
					return "ae";
				case 'á':
				case 'à':
				case 'â':
				case 'Á':
				case 'À':
				case 'Â':
					return "a";
				case 'ö':
				case 'Ö':
					return "oe";
				case 'ó':
				case 'ò':
				case 'ô':
				case 'Ó':
				case 'Ò':
				case 'Ô':
					return "o";
				case 'ü':
				case 'Ü':
					return "ue";
				case 'ú':
				case 'ù':
				case 'Ú':
				case 'Ù':
				case 'û':
				case 'Û':
					return "u";
				case 'ß':
					return "ss";
				case '°':
					return "o";
				default:
					if (asciiChar < 128)
					{
						return asciiChar.ToString();
					}
					else
					{
						return "";
					}
			}
		}

#region ASCII -> ASCII Telex character set

		private static AsciiConvItem[] _asciiIta2Tab = new AsciiConvItem[]
		{
			new AsciiConvItem(0x00, ASC_NUL),
			new AsciiConvItem(0x05, ASC_WRU),
			new AsciiConvItem(0x07, ASC_BEL),
			new AsciiConvItem(0x0A, ASC_LF),
			new AsciiConvItem(0x0D, ASC_CR),
			new AsciiConvItem(0x10, ASC_SHIFTF),
			new AsciiConvItem(0x11, ASC_SHIFTG),
			new AsciiConvItem(0x12, ASC_SHIFTH),
			new AsciiConvItem(0x1E, ASC_LTRS),
			new AsciiConvItem(0x1F, ASC_FIGS),
			new AsciiConvItem(0x20, ' '),
			new AsciiConvItem(0x22, "''"), // "
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
			new AsciiConvItem(0x7C, '/'),
			new AsciiConvItem(0x7D, ",)"), // }
			new AsciiConvItem(0x7E, '-'), // ~
		};

		/*
		private static AsciiConvItem[] _asciiIta2ExtTab = new AsciiConvItem[]
		{
			new AsciiConvItem(0x00, ASC_NUL),
			new AsciiConvItem(0x05, ASC_WRU),
			new AsciiConvItem(0x07, ASC_BEL),
			new AsciiConvItem(0x0A, ASC_LF),
			new AsciiConvItem(0x0D, ASC_CR),
			new AsciiConvItem(0x1E, ASC_LTRS),
			new AsciiConvItem(0x1F, ASC_FIGS),
			new AsciiConvItem(0x20, ' '),
			new AsciiConvItem(0x21, '!'),
			new AsciiConvItem(0x22, "''"), // "
			new AsciiConvItem(0x23, '#'),
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
			new AsciiConvItem(0x5B, "(:"), // [
			//new AsciiConvItem(0x5C, ""), // \
			new AsciiConvItem(0x5D, ":)"), // ]
			//new AsciiConvItem(0x5E, ""), // ^
			new AsciiConvItem(0x5F, ' '), // _
			new AsciiConvItem(0x60, '\''), // `
			new AsciiConvItem(0x61, 0x7A, 'a'), // a..z
			new AsciiConvItem(0x7B, "(,"), // {
			new AsciiConvItem(0x7C, '/'),
			new AsciiConvItem(0x7D, ",)"), // }
			new AsciiConvItem(0x7E, '-'), // ~
		};
		*/

		private static AsciiConvItem[] _asciiUsTtyTab = new AsciiConvItem[]
		{
			new AsciiConvItem(0x00, ASC_NUL),
			new AsciiConvItem(0x05, ASC_WRU),
			new AsciiConvItem(0x07, ASC_BEL),
			new AsciiConvItem(0x0A, ASC_LF),
			new AsciiConvItem(0x0D, ASC_CR),
			new AsciiConvItem(0x1E, ASC_LTRS),
			new AsciiConvItem(0x1F, ASC_FIGS),
			new AsciiConvItem(0x20, ' '),
			new AsciiConvItem(0x21, '!'),
			new AsciiConvItem(0x22, '"'), // "
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
			new AsciiConvItem(0x40, '@'),
			new AsciiConvItem(0x41, 0x5A, 'a'), // A..Z
			new AsciiConvItem(0x5B, "(:"), // [
			//new AsciiConvItem(0x5C, ""), // \
			new AsciiConvItem(0x5D, ":)"), // ]
			//new AsciiConvItem(0x5E, ""), // ^
			new AsciiConvItem(0x5F, ' '), // _
			new AsciiConvItem(0x60, '\''), // `
			new AsciiConvItem(0x61, 0x7A, 'a'), // a..z
			new AsciiConvItem(0x7B, "(,"), // {
			new AsciiConvItem(0x7C, '/'),
			new AsciiConvItem(0x7D, ",)"), // }
			new AsciiConvItem(0x7E, '-'), // ~
		};



#endregion

		private static CodeItem[] _codeTab = new CodeItem[32]
		{
			new CodeItem(
				0x00,
				ASC_NUL,
				new string[] { "", "" }
				),
			new CodeItem(
				0x01,
				new char?[] { 't' },
				new char?[] { '5' },
				new string[] { "t" },
				new string[] { "5" }
				),
			new CodeItem(
				0x02,
				'\r',
				new string[] { "WR", "CR" }
				),
			new CodeItem(
				0x03,
				new char?[] { 'o' },
				new char?[] { '9' },
				new string[] { "o" },
				new string[] { "9" }
				),
			new CodeItem(
				0x04,
				' ',
				new string[] { "ZWR", "SP" }
				),
			new CodeItem(
				0x05,
				new char?[] { 'h' },
				new char?[] { ASC_SHIFTH, '#' },
				new string[] { "h" },
				new string[] { ASC_SHIFTH.ToString(), "#" },
				'#',
				"#"
				),
			new CodeItem(
				0x06,
				new char?[] { 'n' },
				new char?[] { ',' },
				new string[] { "n" },
				new string[] { "," }
				),
			new CodeItem(
				0x07,
				new char?[] { 'm' },
				new char?[] { '.' },
				new string[] { "m" },
				new string[] { "." }
				),
			new CodeItem(
				0x08,
				'\n',
				new string[] { "ZL", "NL" }
				),
			new CodeItem(
				0x09,
				new char?[] { 'l' },
				new char?[] { ')' },
				new string[] { "l" },
				new string[] { ")" }
				),
			new CodeItem(
				0x0A,
				new char?[] { 'r' },
				new char?[] { '4' },
				new string[] { "r" },
				new string[] { "4" }
				),
			new CodeItem(
				0x0B,
				new char?[] { 'g' },
				new char?[] { ASC_SHIFTG, '&' }, // or @
				new string[] { "g" },
				new string[] { ASC_SHIFTG.ToString(), "&" }, // or @
				'&',
				"&"
				),
			new CodeItem(
				0x0C,
				new char?[] { 'i' },
				new char?[] { '8' },
				new string[] { "i" },
				new string[] { "8" }
				),
			new CodeItem(
				0x0D,
				new char?[] { 'p' },
				new char?[] { '0' },
				new string[] { "p" },
				new string[] { "0" }
				),
			new CodeItem(
				0x0E,
				new char?[] { 'c' },
				new char?[] { ':' },
				new string[] { "c" },
				new string[] { ":" }
				),
			new CodeItem(
				0x0F,
				new char?[] { 'v' },
				new char?[] { '=' },
				new string[] { "v" },
				new string[] { "=" }
				),
			new CodeItem(
				0x10,
				new char?[] { 'e' },
				new char?[] { '3' },
				new string[] { "e" },
				new string[] { "3" }
				),
			new CodeItem(
				0x11,
				new char?[] { 'z' },
				new char?[] { '+', '"' },
				new string[] { "z" },
				new string[] { "+", "\"" }
				),
			new CodeItem(
				0x12,
				new char?[] { 'd' },
				new char?[] { ASC_WRU, '$' }, // or Pound
				new string[] { "d" },
				new string[] { "WRU", "$" } // or Pound
				),
			new CodeItem(
				0x13,
				new char?[] { 'b' },
				new char?[] { '?' },
				new string[] { "b" },
				new string[] { "?" }
				),
			new CodeItem(
				0x14,
				new char?[] { 's' },
				new char?[] { '\'', ASC_BEL },
				new string[] { "s" },
				new string[] { "'", "BEL" }
				),
			new CodeItem(
				0x15,
				new char?[] { 'y' },
				new char?[] { '6' },
				new string[] { "y" },
				new string[] { "6" }
				),
			new CodeItem(
				0x16,
				new char?[] { 'f' },
				new char?[] { ASC_SHIFTF, '!' }, // or %
				new string[] { "f" },
				new string[] { ASC_SHIFTF.ToString(), "!" }, // or %
				'!',
				"!"
				),
			new CodeItem(
				0x17,
				new char?[] { 'x' },
				new char?[] { '/' },
				new string[] { "x" },
				new string[] { "/" }
				),
			new CodeItem(
				0x18,
				new char?[] { 'a' },
				new char?[] { '-' },
				new string[] { "a" },
				new string[] { "-" }
				),
			new CodeItem(
				0x19,
				new char?[] { 'w' },
				new char?[] { '2' },
				new string[] { "w" },
				new string[] { "2" }
				),
			new CodeItem(
				0x1A,
				new char?[] { 'j' },
				new char?[] { ASC_BEL, '\'' },
				new string[] { "j" },
				new string[] { "BEL", "'" }
				),
			new CodeItem(
				0x1B,
				ASC_FIGS,
				new string[] { "Zi", "Fig" }
				),
			new CodeItem(
				0x1C,
				new char?[] { 'u' },
				new char?[] { '7' },
				new string[] { "u" },
				new string[] { "7" }
				),
			new CodeItem(
				0x1D,
				new char?[] { 'q' },
				new char?[] { '1' },
				new string[] { "q" },
				new string[] { "1" }
				),
			new CodeItem(
				0x1E,
				new char?[] { 'k' },
				new char?[] { '(' },
				new string[] { "k" },
				new string[] { "(" }
				),
			new CodeItem(
				0x1F,
				ASC_LTRS,
				new string[] { "Bu", "Ltr" }
				)
		};

#region Keyboard handling

		/// <summary>
		/// all characters that are to be recognized as input in the terminal windows must be explicitly
		/// defined here.
		/// </summary>
		/// <param name="keyChar"></param>
		/// <returns></returns>
		public static char? KeyboardCharacters(char keyChar)
		{
			int code = (int)keyChar;
			char? newChar = null;

			// all characters that are to be recognized as input in the terminal windows must be explicitly defined here.
			switch (char.ToLower(keyChar))
			{
				default:
					// letters and numbers
					if (code >= 0x30 && code <= 0x39 || code >= 0x41 && code <= 0x5A || code >= 0x61 && code <= 0x7A)
					{
						newChar = keyChar;
					}
					break;
				case ' ':
				case '+':
				case '-':
				case ',':
				case '.':
				case ':':
				case '\'':
				case '/':
				case '(':
				case ')':
				case '=':
				case '?':
				// tty-us characters
				case '@':
				case '!':
				case '$':
				case '%':
				case '&':
				case '#':
				case ';':
				// characters that will be replaced
				case 'ä':
				case 'ö':
				case 'ü':
				case 'ß':
				case '[':
				case ']':
				case '{':
				case '}':
				case '<':
				case '>':
				case '´':
				case '`':
				// control characters
				case ASC_WRU: // ctrl-e ENQ
				case ASC_BEL: // ctrl-g BEL
				case ASC_HEREIS: // ctrl-i here is
				case '\x17': // ctrl-w WRU
					newChar = keyChar;
					break;
			}
			return newChar;
		}

#endregion
	}
}
