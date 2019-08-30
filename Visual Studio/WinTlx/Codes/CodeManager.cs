using System.Collections.Generic;
using System.Linq;

namespace WinTlx.Codes
{
	public static class CodeManager
	{
		//private static ConfigData _config => ConfigManager.Instance.Config;

		public static string BaudotStringToAscii(byte[] baudotData, ref ShiftStates shiftState, CodeSets codeSet)
		{
			string asciiStr = "";
			for (int i = 0; i < baudotData.Length; i++)
			{
				byte baudotChr = baudotData[i];
				if (baudotChr == BAU_LTR)
				{
					shiftState = ShiftStates.Ltr;
				}
				else if (baudotChr == BAU_FIG)
				{
					shiftState = ShiftStates.Figs;
				}
				else
				{
					char asciiChr = BaudotCharToAscii(baudotData[i], shiftState, codeSet);
					asciiStr += asciiChr;
				}
			}
			return asciiStr;
		}

		public static char BaudotCharToAscii(byte baudotCode, ShiftStates shiftState, CodeSets codeSet)
		{
			if (baudotCode > 0x1F)
			{
				return (char)ASC_INV;
			}

			return _codeTab[baudotCode].GetCode(shiftState, codeSet);

			/*
			char[,] codeTab = codeSet == CodeSets.ITA2 ? _codeTabEu : _codeTabUs;

			switch (shiftState)
			{
				case ShiftStates.Unknown:
				default:
					return ASC_INV;
				case ShiftStates.Ltr:
					//return codeTab[LTRS, baudotCode];
					return _codeTab[baudotCode].GetCode(shiftState, codeSet, false);
				case ShiftStates.Figs:
					return codeTab[FIGS, baudotCode];
			}
			*/
		}

		public static string BaudotCodeToPuncherText(byte baudotCode, ShiftStates shiftState, CodeSets codeSet)
		{
			if (baudotCode > 0x1F)
			{
				return ASC_INV.ToString();
			}
			return _codeTab[baudotCode].GetName(shiftState, codeSet);

			/*
			string[,] codeTab = codeSet == CodeSets.ITA2 ? _codeTabPuncherEu : _codeTabPuncherUs;

			switch (shiftState)
			{
				case ShiftStates.Ltr:
					return codeTab[LTRS, baudotCode];
				case ShiftStates.Figs:
					return codeTab[FIGS, baudotCode];
				default:
					return "";
			}
			*/
		}

		public static byte[] AsciiStringToBaudot(string asciiStr, ref ShiftStates shiftState, CodeSets codeSet)
		{
			byte[] baudotData = new byte[0];
			for (int i = 0; i < asciiStr.Length; i++)
			{
				string telexData = AsciiCharToTelex2(asciiStr[i], codeSet);
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
				telexStr += AsciiCharToTelex2(asciiStr[i], codeSet);
			}
			return telexStr;
		}

#if false
		/// <summary>
		/// convert ASCII character to baudot printable character and replacement character
		/// </summary>
		/// <param name="asciiChr"></param>
		/// <returns></returns>
		private static string AsciiCharToTelex(char asciiChr, CodeSets codeSet)
		{
			Dictionary<byte, string> asciiToTelexTab = null;
			switch(codeSet)
			{
				default:
				case CodeSets.ITA2:
					asciiToTelexTab = _asciiToIta2Tab;
					break;
				case CodeSets.ITA2EXT:
					asciiToTelexTab = _asciiToIta2ExtTab;
					break;
				case CodeSets.USTTY:
					asciiToTelexTab = _asciiToUsTtyTab;
					break;
			}

			string asciiData = CodePage437ToPlainAscii(asciiChr);
			string telexData = "";
			for (int i = 0; i < asciiData.Length; i++)
			{
				byte data = (byte)asciiData[i];
				if (asciiToTelexTab.ContainsKey(data))
				{ 
					telexData += asciiToTelexTab[data];
				}
			}
			return telexData;
		}
#endif

		/// <summary>
		/// convert ASCII character to baudot printable character and replacement character
		/// </summary>
		/// <param name="asciiChr"></param>
		/// <returns></returns>
		private static string AsciiCharToTelex2(char asciiChr, CodeSets codeSet)
		{
			AsciiConvItem[] asciiToTelexTab = null;
			switch (codeSet)
			{
				default:
				case CodeSets.ITA2:
					asciiToTelexTab = _asciiIta2Tab2;
					break;
				case CodeSets.ITA2EXT:
					asciiToTelexTab = _asciiIta2ExtTab2;
					break;
				case CodeSets.USTTY:
					asciiToTelexTab = _asciiUsTtyTab2;
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

			if (baudCode == BAU_LTR)
			{
				buffer = Helper.AddByte(buffer, BAU_LTR);
				shiftState = ShiftStates.Ltr;
				return buffer;
			}

			if (baudCode == BAU_FIG)
			{
				buffer = Helper.AddByte(buffer, BAU_FIG);
				shiftState = ShiftStates.Figs;
				return buffer;
			}

			if (shiftState == ShiftStates.Unknown && newShiftState == ShiftStates.Unknown)
			{
				buffer = Helper.AddByte(buffer, BAU_LTR);
				newShiftState = ShiftStates.Ltr;
			}

			if (shiftState == ShiftStates.Unknown && newShiftState == ShiftStates.Ltr ||
				shiftState == ShiftStates.Figs && newShiftState == ShiftStates.Ltr)
			{
				buffer = Helper.AddByte(buffer, BAU_LTR);
				buffer = Helper.AddByte(buffer, baudCode);
				shiftState = ShiftStates.Ltr;
				return buffer;
			}

			if (shiftState == ShiftStates.Unknown && newShiftState == ShiftStates.Figs ||
				shiftState == ShiftStates.Ltr && newShiftState == ShiftStates.Figs)
			{
				buffer = Helper.AddByte(buffer, BAU_FIG);
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

		private static byte? FindBaudot(char asciiChar, ShiftStates shiftState, CodeSets codeSet
			)
		{
			// search ascii chars to find baudot code
			/*
			char[,] _codeTab = codeSet == CodeSets.ITA2 ? _codeTabEu : _codeTabUs;
			for (int c = 0; c < 32; c++)
			{
				if (_codeTab[shift, c] == telexChar)
				{
					return (byte)c;
				}
			}
			return null;
			*/

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

		public const char ASC_INV = '~'; // replace invalid baudot character
		public const char ASC_NUL = '\x00';
		public const char ASC_WRU = '\x05'; // enquire (WRU)
		public const char ASC_BEL = '\x07'; // bel
		public const char ASC_HEREIS = '\x09'; // own answerback
		public const char ASC_LF = '\x0A';
		public const char ASC_CR = '\x0D';
		public const char ASC_LTRS = '\x1E';
		public const char ASC_FIGS = '\x1F';

		public const byte BAU_NUL = 0x00;
		public const byte BAU_LTR = 0x1F;
		public const byte BAU_FIG = 0x1B;
		public const byte BAU_WRU = 0x12;
		public const byte BAU_BEL = 0x1A;
		public const byte BAU_CR = 0x02;
		public const byte BAU_LF = 0x08;

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

#region ASCII -> Telex character set

		private static AsciiConvItem[] _asciiIta2Tab2 = new AsciiConvItem[]
		{
			new AsciiConvItem(0x00, ASC_NUL),
			new AsciiConvItem(0x05, ASC_WRU),
			new AsciiConvItem(0x07, ASC_BEL),
			new AsciiConvItem(0x0A, ASC_LF),
			new AsciiConvItem(0x0D, ASC_CR),
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

		private static AsciiConvItem[] _asciiIta2ExtTab2 = new AsciiConvItem[]
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

		private static AsciiConvItem[] _asciiUsTtyTab2 = new AsciiConvItem[]
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


#if false
		// ASCII -> ASCII ITA2
		private static Dictionary<byte, string> _asciiToIta2Tab = new Dictionary<byte, string>
		{
			{ 0x00, ASC_NUL.ToString() }, // NUL
			{ 0x05, ASC_WRU.ToString() }, // WRU
			{ 0x07, ASC_BEL.ToString() }, // BEL
			{ 0x0A, ASC_LF.ToString() }, // LF
			{ 0x0D, ASC_CR.ToString() }, // CR
			{ 0x1E, ASC_LTRS.ToString() }, // ITA2 letters
			{ 0x1F, ASC_FIGS.ToString() }, // ITA2 figurs
			{ 0x20, " " },  // space
			{ 0x22, "''" }, // " -> ''
			{ 0x27, "'" },	// ' -> '
			{ 0x28, "(" },	// ( -> (
			{ 0x29, ")" },	// ) -> )
			{ 0x2B, "+" },	// + -> +
			{ 0x2C, "," },	// , -> ,
			{ 0x2D, "-" },	// - -> -
			{ 0x2E, "." },	// . -> .
			{ 0x2F, "/" },	// / -> /
			{ 0x30, "0" },	// 0 -> 0
			{ 0x31, "1" },	// 1 -> 1
			{ 0x32, "2" },	// 2 -> 2
			{ 0x33, "3" },	// 3 -> 3
			{ 0x34, "4" },	// 4 -> 4
			{ 0x35, "5" },	// 5 -> 5
			{ 0x36, "6" },	// 6 -> 6
			{ 0x37, "7" },	// 7 -> 7
			{ 0x38, "8" },	// 8 -> 8
			{ 0x39, "9" },	// 9 -> 9
			{ 0x3A, ":" },	// : -> :
			{ 0x3C, "(." },	// < -> (.
			{ 0x3D, "=" },	// = -> =
			{ 0x3E, ".)" },	// > -> .)
			{ 0x3F, "?" },	// ? -> ?
			{ 0x41, "a" },	// A -> a
			{ 0x42, "b" },	// B -> b
			{ 0x43, "c" },	// C -> c
			{ 0x44, "d" },	// D -> d
			{ 0x45, "e" },	// E -> e
			{ 0x46, "f" },	// F -> f
			{ 0x47, "g" },	// G -> g
			{ 0x48, "h" },	// H -> h
			{ 0x49, "i" },	// I -> i
			{ 0x4A, "j" },	// J -> j
			{ 0x4B, "k" },	// K -> k
			{ 0x4C, "l" },	// L -> l
			{ 0x4D, "m" },	// M -> m
			{ 0x4E, "n" },	// N -> n
			{ 0x4F, "o" },	// O -> o
			{ 0x50, "p" },	// P -> p
			{ 0x51, "q" },	// Q -> q
			{ 0x52, "r" },	// R -> r
			{ 0x53, "s" },	// S -> s
			{ 0x54, "t" },	// T -> t
			{ 0x55, "u" },	// U -> u
			{ 0x56, "v" },	// V -> v
			{ 0x57, "w" },	// W -> w
			{ 0x58, "x" },	// X -> x
			{ 0x59, "y" },	// Y -> y
			{ 0x5A, "z" },	// Z -> z
			{ 0x5B, "(:" },	// [ -> (:
			{ 0x5C, "" },	// \ ->
			{ 0x5D, ":)" },	// ] -> :)
			{ 0x5E, "" },	// ^ ->
			{ 0x5F, " " },	// _ -> SP
			{ 0x60, "'" },	// ` -> '
			{ 0x61, "a" },	// a -> a
			{ 0x62, "b" },	// b -> b
			{ 0x63, "c" },	// c -> c
			{ 0x64, "d" },	// d -> d
			{ 0x65, "e" },	// e -> e
			{ 0x66, "f" },	// f -> f
			{ 0x67, "g" },	// g -> g
			{ 0x68, "h" },	// h -> h
			{ 0x69, "i" },	// i -> i
			{ 0x6A, "j" },	// j -> j
			{ 0x6B, "k" },	// k -> k
			{ 0x6C, "l" },	// l -> l
			{ 0x6D, "m" },	// m -> m
			{ 0x6E, "n" },	// n -> n
			{ 0x6F, "o" },	// o -> o
			{ 0x70, "p" },	// p -> p
			{ 0x71, "q" },	// q -> q
			{ 0x72, "r" },	// r -> r
			{ 0x73, "s" },	// S -> s
			{ 0x74, "t" },	// t -> t
			{ 0x75, "u" },	// u -> u
			{ 0x76, "v" },	// v -> v
			{ 0x77, "w" },	// w -> w
			{ 0x78, "x" },	// x -> x
			{ 0x79, "y" },	// y -> y
			{ 0x7A, "z" },	// z -> z
			{ 0x7B, "(," },	// { -> (,
			{ 0x7C, "/" },	// / -> /
			{ 0x7D, ")," },	// } -> ,)
			{ 0x7E, "-" },	// ~ -> -
		};

		// ASCII -> ASCII ITA2EXT (standard ITA2 with addidional characters)
		private static Dictionary<byte, string> _asciiToIta2ExtTab = new Dictionary<byte, string>
		{
			{ 0x00, ASC_NUL.ToString() }, // NUL
			{ 0x05, ASC_WRU.ToString() }, // WRU
			{ 0x07, ASC_BEL.ToString() }, // BEL
			{ 0x0A, ASC_LF.ToString() }, // LF
			{ 0x0D, ASC_CR.ToString() }, // CR
			{ 0x1E, ASC_LTRS.ToString() }, // ITA2 letters
			{ 0x1F, ASC_FIGS.ToString() }, // ITA2 figurs
			{ 0x20, " " },  // space
			{ 0x21, "!" },  // space
			{ 0x22, "\"" }, // " -> "
			{ 0x23, "#" },	// # -> #
			{ 0x26, "&" },  // & -> &
			{ 0x27, "'" },	// ' -> '
			{ 0x28, "(" },	// ( -> (
			{ 0x29, ")" },	// ) -> )
			{ 0x2B, "+" },	// + -> +
			{ 0x2C, "," },	// , -> ,
			{ 0x2D, "-" },	// - -> -
			{ 0x2E, "." },	// . -> .
			{ 0x2F, "/" },	// / -> /
			{ 0x30, "0" },	// 0 -> 0
			{ 0x31, "1" },	// 1 -> 1
			{ 0x32, "2" },	// 2 -> 2
			{ 0x33, "3" },	// 3 -> 3
			{ 0x34, "4" },	// 4 -> 4
			{ 0x35, "5" },	// 5 -> 5
			{ 0x36, "6" },	// 6 -> 6
			{ 0x37, "7" },	// 7 -> 7
			{ 0x38, "8" },	// 8 -> 8
			{ 0x39, "9" },	// 9 -> 9
			{ 0x3A, ":" },	// : -> :
			{ 0x3C, "(." },	// < -> (.
			{ 0x3D, "=" },	// = -> =
			{ 0x3E, ".)" },	// > -> .)
			{ 0x3F, "?" },	// ? -> ?
			{ 0x41, "a" },	// A -> a
			{ 0x42, "b" },	// B -> b
			{ 0x43, "c" },	// C -> c
			{ 0x44, "d" },	// D -> d
			{ 0x45, "e" },	// E -> e
			{ 0x46, "f" },	// F -> f
			{ 0x47, "g" },	// G -> g
			{ 0x48, "h" },	// H -> h
			{ 0x49, "i" },	// I -> i
			{ 0x4A, "j" },	// J -> j
			{ 0x4B, "k" },	// K -> k
			{ 0x4C, "l" },	// L -> l
			{ 0x4D, "m" },	// M -> m
			{ 0x4E, "n" },	// N -> n
			{ 0x4F, "o" },	// O -> o
			{ 0x50, "p" },	// P -> p
			{ 0x51, "q" },	// Q -> q
			{ 0x52, "r" },	// R -> r
			{ 0x53, "s" },	// S -> s
			{ 0x54, "t" },	// T -> t
			{ 0x55, "u" },	// U -> u
			{ 0x56, "v" },	// V -> v
			{ 0x57, "w" },	// W -> w
			{ 0x58, "x" },	// X -> x
			{ 0x59, "y" },	// Y -> y
			{ 0x5A, "z" },	// Z -> z
			{ 0x5B, "(:" },	// [ -> (:
			{ 0x5C, "" },	// \ ->
			{ 0x5D, ":)" },	// ] -> :)
			//{ 0x5E, "" },	// ^ ->
			{ 0x5F, " " },	// _ -> SP
			{ 0x60, "'" },	// ` -> '
			{ 0x61, "a" },	// a -> a
			{ 0x62, "b" },	// b -> b
			{ 0x63, "c" },	// c -> c
			{ 0x64, "d" },	// d -> d
			{ 0x65, "e" },	// e -> e
			{ 0x66, "f" },	// f -> f
			{ 0x67, "g" },	// g -> g
			{ 0x68, "h" },	// h -> h
			{ 0x69, "i" },	// i -> i
			{ 0x6A, "j" },	// j -> j
			{ 0x6B, "k" },	// k -> k
			{ 0x6C, "l" },	// l -> l
			{ 0x6D, "m" },	// m -> m
			{ 0x6E, "n" },	// n -> n
			{ 0x6F, "o" },	// o -> o
			{ 0x70, "p" },	// p -> p
			{ 0x71, "q" },	// q -> q
			{ 0x72, "r" },	// r -> r
			{ 0x73, "s" },	// S -> s
			{ 0x74, "t" },	// t -> t
			{ 0x75, "u" },	// u -> u
			{ 0x76, "v" },	// v -> v
			{ 0x77, "w" },	// w -> w
			{ 0x78, "x" },	// x -> x
			{ 0x79, "y" },	// y -> y
			{ 0x7A, "z" },	// z -> z
			{ 0x7B, "(," },	// { -> (,
			{ 0x7C, "/" },	// / -> /
			{ 0x7D, ")," },	// } -> ,)
			{ 0x7E, "-" },	// ~ -> -
		};

		// ASCII -> ASCII USTTY conversion
		private static Dictionary<byte, string> _asciiToUsTtyTab = new Dictionary<byte, string>
		{
			{ 0x00, ASC_NUL.ToString() }, // NUL
			{ 0x05, ASC_WRU.ToString() }, // WRU
			{ 0x07, ASC_BEL.ToString() }, // BEL
			{ 0x0A, ASC_LF.ToString() }, // LF
			{ 0x0D, ASC_CR.ToString() }, // CR
			{ 0x1E, ASC_LTRS.ToString() }, // ITA2 letters
			{ 0x1F, ASC_FIGS.ToString() }, // ITA2 figurs
			{ 0x20, " " },  // space
			{ 0x21, "!" },  // ! -> ! (%)
			{ 0x22, "\"" }, // " -> "
			{ 0x23, "#" },	// # -> # ($)
			{ 0x24, "$" },  // $ -> $
			{ 0x26, "&" },  // & -> & (@)
			{ 0x27, "'" },	// ' -> '
			{ 0x28, "(" },	// ( -> (
			{ 0x29, ")" },	// ) -> )
			{ 0x2C, "," },	// , -> ,
			{ 0x2D, "-" },	// - -> -
			{ 0x2E, "." },	// . -> .
			{ 0x2F, "/" },	// / -> /
			{ 0x30, "0" },	// 0 -> 0
			{ 0x31, "1" },	// 1 -> 1
			{ 0x32, "2" },	// 2 -> 2
			{ 0x33, "3" },	// 3 -> 3
			{ 0x34, "4" },	// 4 -> 4
			{ 0x35, "5" },	// 5 -> 5
			{ 0x36, "6" },	// 6 -> 6
			{ 0x37, "7" },	// 7 -> 7
			{ 0x38, "8" },	// 8 -> 8
			{ 0x39, "9" },	// 9 -> 9
			{ 0x3A, ":" },	// : -> :
			{ 0x3B, ";" },	// ; -> ;
			{ 0x3C, "(." },	// < -> (.
			{ 0x3E, ".)" },	// > -> .)
			{ 0x3F, "?" },	// ? -> ?
			{ 0x40, "@" },  // @ -> @
			{ 0x41, "a" },	// A -> a
			{ 0x42, "b" },	// B -> b
			{ 0x43, "c" },	// C -> c
			{ 0x44, "d" },	// D -> d
			{ 0x45, "e" },	// E -> e
			{ 0x46, "f" },	// F -> f
			{ 0x47, "g" },	// G -> g
			{ 0x48, "h" },	// H -> h
			{ 0x49, "i" },	// I -> i
			{ 0x4A, "j" },	// J -> j
			{ 0x4B, "k" },	// K -> k
			{ 0x4C, "l" },	// L -> l
			{ 0x4D, "m" },	// M -> m
			{ 0x4E, "n" },	// N -> n
			{ 0x4F, "o" },	// O -> o
			{ 0x50, "p" },	// P -> p
			{ 0x51, "q" },	// Q -> q
			{ 0x52, "r" },	// R -> r
			{ 0x53, "s" },	// S -> s
			{ 0x54, "t" },	// T -> t
			{ 0x55, "u" },	// U -> u
			{ 0x56, "v" },	// V -> v
			{ 0x57, "w" },	// W -> w
			{ 0x58, "x" },	// X -> x
			{ 0x59, "y" },	// Y -> y
			{ 0x5A, "z" },	// Z -> z
			{ 0x5B, "(:" },	// [ -> (:
			{ 0x5C, "" },	// \
			{ 0x5D, ":)" },	// ] -> :)
			{ 0x5E, "^" },	// ^
			{ 0x5F, " " },	// _ -> SP
			{ 0x60, "'" },	// ` -> '
			{ 0x61, "a" },	// a -> a
			{ 0x62, "b" },	// b -> b
			{ 0x63, "c" },	// c -> c
			{ 0x64, "d" },	// d -> d
			{ 0x65, "e" },	// e -> e
			{ 0x66, "f" },	// f -> f
			{ 0x67, "g" },	// g -> g
			{ 0x68, "h" },	// h -> h
			{ 0x69, "i" },	// i -> i
			{ 0x6A, "j" },	// j -> j
			{ 0x6B, "k" },	// k -> k
			{ 0x6C, "l" },	// l -> l
			{ 0x6D, "m" },	// m -> m
			{ 0x6E, "n" },	// n -> n
			{ 0x6F, "o" },	// o -> o
			{ 0x70, "p" },	// p -> p
			{ 0x71, "q" },	// q -> q
			{ 0x72, "r" },	// r -> r
			{ 0x73, "s" },	// S -> s
			{ 0x74, "t" },	// t -> t
			{ 0x75, "u" },	// u -> u
			{ 0x76, "v" },	// v -> v
			{ 0x77, "w" },	// w -> w
			{ 0x78, "x" },	// x -> x
			{ 0x79, "y" },	// y -> y
			{ 0x7A, "z" },	// z -> z
			{ 0x7B, "(," },	// { -> (,
			{ 0x7C, "/" },	// / -> /
			{ 0x7D, ")," },	// } -> ,)
			{ 0x7E, "-" },	// ~ -> -
		};
#endif

#endregion

		//private const int LTRS = 0;
		//private const int FIGS = 1;

		private static CodeItem[] _codeTab = new CodeItem[32]
		{
			new CodeItem(
				0x00,
				ASC_NUL,
				new string[] { "", "", "" }
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
				new char?[] { null, '#', '#' },
				new string[] { "h" },
				new string[] { null, "#", "#" },
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
				new char?[] { null, '&', '&' }, // @
				new string[] { "g" },
				new string[] { null, "&", "&" }, // @
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
				new char?[] { '+', '+', '"' },
				new string[] { "z" },
				new string[] { "+", "+", "\"" }
				),
			new CodeItem(
				0x12,
				new char?[] { 'd' },
				new char?[] { ASC_WRU, ASC_WRU, '$' }, // or Pound
				new string[] { "d" },
				new string[] { "WRU", "WRU", "$" } // or Pound
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
				new char?[] { '\'', '\'', ASC_BEL },
				new string[] { "s" },
				new string[] { "'", "'", "BEL" }
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
				new char?[] { null, '!', '!' }, // or %
				new string[] { "f" },
				new string[] { null, "!", "!" }, // or %
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
				new char?[] { ASC_BEL, ASC_BEL, '\'' },
				new string[] { "j" },
				new string[] { "BEL", "BEL", "'" }
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
