using System.Collections.Generic;
using System.Linq;
using WinTlx.Config;

namespace WinTlx
{
	public static class CodeConversion
	{
		//private static ConfigData _config => ConfigManager.Instance.Config;

		public enum ShiftStates
		{
			Unknown,
			Ltr,
			Figs,
			Both
		}

		public static string BaudotStringToAscii(byte[] baudotData, ref ShiftStates shiftState, CodeStandards codeStd)
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
					char asciiChr = BaudotCharToAscii(baudotData[i], shiftState, codeStd);
					asciiStr += asciiChr;
				}
			}
			return asciiStr;
		}

		public static char BaudotCharToAscii(byte baudotCode, ShiftStates shiftState, CodeStandards codeStd)
		{
			if (baudotCode > 0x1F)
			{
				return (char)ASC_INV;
			}

			char[,] codeTab = codeStd == CodeStandards.Ita2 ? _codeTabEu : _codeTabUs;

			switch (shiftState)
			{
				case ShiftStates.Unknown:
				default:
					return ASC_INV;
				case ShiftStates.Ltr:
					return codeTab[LTRS, baudotCode];
				case ShiftStates.Figs:
					return codeTab[FIGS, baudotCode];
			}
		}

		public static string BaudotCodeToPuncherText(byte baudotCode, ShiftStates shiftState, CodeStandards codeStd)
		{
			string[,] codeTab = codeStd == CodeStandards.Ita2 ? _codeTabPuncherEu : _codeTabPuncherUs;

			switch (shiftState)
			{
				case ShiftStates.Ltr:
					return codeTab[LTRS, baudotCode];
				case ShiftStates.Figs:
					return codeTab[FIGS, baudotCode];
				default:
					return "";
			}
		}

		public static byte[] AsciiStringToBaudot(string asciiStr, ref ShiftStates shiftState, CodeStandards codeStd)
		{
			byte[] baudotData = new byte[0];
			for (int i = 0; i < asciiStr.Length; i++)
			{
				string telexData = AsciiCharToTelex(asciiStr[i], codeStd);
				byte[] data = TelexStringToBaudot(telexData, ref shiftState, codeStd);
				baudotData = baudotData.Concat(data).ToArray();
			}
			return baudotData;
		}

		/// <summary>
		/// convert ASCII string to printable characters and replacement characters
		/// </summary>
		/// <param name="asciiStr"></param>
		/// <returns></returns>
		public static string AsciiStringToTelex(string asciiStr, CodeStandards codeStd)
		{
			string telexStr = "";
			for (int i = 0; i < asciiStr.Length; i++)
			{
				telexStr += AsciiCharToTelex(asciiStr[i], codeStd);
			}
			return telexStr;
		}

		/// <summary>
		/// convert ASCII character to baudot printable character and replacement character
		/// </summary>
		/// <param name="asciiChr"></param>
		/// <returns></returns>
		private static string AsciiCharToTelex(char asciiChr, CodeStandards codeStd)
		{
			Dictionary<byte, string> asciiToTelexTab =
				codeStd == CodeStandards.Ita2 ? _asciiToTelexTabEu : _asciiToTelexTabUs;

			string asciiData = _codePage437ToAsciiTab(asciiChr);
			string telexData = "";
			for (int i = 0; i < asciiData.Length; i++)
			{
				//telexData += _asciiToTelexTab[(int)asciiData[i]];
				byte data = (byte)asciiData[i];
				if (asciiToTelexTab.ContainsKey(data))
				{ 
					telexData += asciiToTelexTab[data];
				}
			}
			return telexData;
		}

		public static byte[] TelexStringToBaudot(string telexStr, ref ShiftStates shiftState, CodeStandards codeStd)
		{
			byte[] buffer = new byte[0];
			for (int i = 0; i < telexStr.Length; i++)
			{
				byte[] baudotData = TelexCharToBaudotWithShift(telexStr[i], ref shiftState, codeStd);
				buffer = buffer.Concat(baudotData).ToArray();
			}
			return buffer;
		}

		public static byte[] TelexCharToBaudotWithShift(char telexChr, ref ShiftStates shiftState, CodeStandards codeStd)
		{
			byte? ltrCode = FindBaudot(LTRS, telexChr, codeStd);
			byte? figCode = FindBaudot(FIGS, telexChr, codeStd);
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

		private static byte? FindBaudot(int shift, char telexChar, CodeStandards codeStd)
		{
			// search ascii chars to find baudot code
			char[,] _codeTab = codeStd == CodeStandards.Ita2 ? _codeTabEu : _codeTabUs;
			for (int c = 0; c < 32; c++)
			{
				if (_codeTab[shift, c] == telexChar)
				{
					return (byte)c;
				}
			}
			return null;
		}

		private const char ASC_INV = '~'; // replace invalid baudot character
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
		/// Code page 437 to Ascii conversion
		/// </summary>
		/// <param name="asciiChar"></param>
		/// <returns></returns>
		private static string _codePage437ToAsciiTab(char asciiChar)
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

		// ASCII -> ASCII conversion Europe
		private static Dictionary<byte, string> _asciiToTelexTabEu = new Dictionary<byte, string>
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

		// ASCII -> ASCII conversion US
		private static Dictionary<byte, string> _asciiToTelexTabUs = new Dictionary<byte, string>
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

		#endregion

		private const int LTRS = 0;
		private const int FIGS = 1;

		#region ITA 2 <-> ASCII

		private static char[,] _codeTabEu =
		{
			{
				ASC_NUL,	// 00 00000 NUL
				't',		// 01 00001 t
				'\r',		// 02 00010 CR
				'o',		// 03 00011 o
				' ',		// 04 00100 SP
				'h',		// 05 00101 h
				'n',		// 06 00110 n
				'm',		// 07 00111 m
				'\n',		// 08 01000 LF
				'l',		// 09 01001 l
				'r',		// 0A 01010 r
				'g',		// 0B 01011 g
				'i',		// 0C 01100 i
				'p',		// 0D 01101 p
				'c',		// 0E 01110 c
				'v',		// 0F 01111 v
				'e',		// 10 10000 e
				'z',		// 11 10001 z
				'd',		// 12 10010 d
				'b',		// 13 10011 b
				's',		// 14 10100 s
				'y',		// 15 10101 y
				'f',		// 16 10110 f
				'x',		// 17 10111 x
				'a',		// 18 11000 a
				'w',		// 19 11001 w
				'j',		// 1A 11010 j
				ASC_FIGS,	// 1B 11011 FIG
				'u',		// 1C 11100 u
				'q',		// 1D 11101 q
				'k',		// 1E 11110 k
				ASC_LTRS    // 1F 11111 LTR
			},

			// figures
			{
				ASC_NUL,	// 00 00000 NUL
				'5',		// 01 00001 5
				'\r',		// 02 00010 CR
				'9',		// 03 00011 9
				' ',		// 04 00100 SP
				ASC_INV,	// 05 00101
				',',		// 06 00110 ,
				'.',		// 07 00111 .
				'\n',		// 08 01000 LF
				')',		// 09 01001 )
				'4',		// 0A 01010 4
				ASC_INV,	// 0B 01011
				'8',		// 0C 01100 8
				'0',		// 0D 01101 0
				':',		// 0E 01110 :
				'=',		// 0F 01111 =
				'3',		// 10 10000 3
				'+',		// 11 10001 +
				ASC_WRU,	// 12 10010 WRU
				'?',		// 13 10011 ?
				'\'',		// 14 10100 '
				'6',		// 15 10101 6
				ASC_INV,	// 16 10110
				'/',		// 17 10111 /
				'-',		// 18 11000 -
				'2',		// 19 11001 2
				ASC_BEL,	// 1A 11010 BEL
				ASC_FIGS,	// 1B 11011 FIG
				'7',		// 1C 11100 7
				'1',		// 1D 11101 1
				'(',		// 1E 11110 (
				ASC_LTRS	// 1F 11111 LTR
			}
		};

		#endregion

		#region US-TTY <-> ASCII

		private static char[,] _codeTabUs =
		{
			{
				ASC_NUL,	// 00 00000 NUL
				't',		// 01 00001 t
				'\r',		// 02 00010 CR
				'o',		// 03 00011 o
				' ',		// 04 00100 SP
				'h',		// 05 00101h
				'n',		// 06 00110 n
				'm',		// 07 00111 m
				'\n',		// 08 01000 LF
				'l',		// 09 01001 l
				'r',		// 0A 01010 r
				'g',		// 0B 01011g
				'i',		// 0C 01100 i
				'p',		// 0D 01101 p
				'c',		// 0E 01110 c
				'v',		// 0F 01111 v
				'e',		// 10 10000 e
				'z',		// 11 10001 z
				'd',		// 12 10010 d
				'b',		// 13 10011 b
				's',		// 14 10100 s
				'y',		// 15 10101 y
				'f',		// 16 10110f
				'x',		// 17 10111 x
				'a',		// 18 11000 a
				'w',		// 19 11001 w
				'j',		// 1A 11010 j
				ASC_FIGS,	// 1B 11011 FIG
				'u',		// 1C 11100 u
				'q',		// 1D 11101 q
				'k',		// 1E 11110 k
				ASC_LTRS    // 1F 11111 LTR
			},

			// figures
			{
				ASC_NUL,	// 00 00000 NUL
				'5',		// 01 00001 5
				'\r',		// 02 00010 CR
				'9',		// 03 00011 9
				' ',		// 04 00100 SP
				'#',		// 05 00101#
				',',		// 06 00110 ,
				'.',		// 07 00111 .
				'\n',		// 08 01000 LF
				')',		// 09 01001 )
				'4',		// 0A 01010 4
				'&',		// 0B 01011& (@)
				'8',		// 0C 01100 8
				'0',		// 0D 01101 0
				':',		// 0E 01110 :
				';',		// 0F 01111 ;
				'3',		// 10 10000 3
				'"',		// 11 10001 "
				'$',		// 12 10010 $ (Pound)
				'?',		// 13 10011 ?
				ASC_BEL,	// 14 10100 BEL
				'6',		// 15 10101 6
				'!',		// 16 10110 ! (%)
				'/',		// 17 10111 /
				'-',		// 18 11000 -
				'2',		// 19 11001 2
				'\'',       // 1A 11010 '
				ASC_FIGS,	// 1B 11011 FIG
				'7',		// 1C 11100 7
				'1',		// 1D 11101 1
				'(',		// 1E 11110 (
				ASC_LTRS	// 1F 11111 LTR
			}
		};

		#endregion

		#region ITA-2 <-> ASCII for tape punch labeling

		// bitorder is: 54.321

		private static string[,] _codeTabPuncherEu =
		{
			// letters
			{
				"",			// 00
				"t",		// 01
				"CR",		// 02 carriage return
				"o",		// 03
				"SP",		// 04 space
				"h",		// 05
				"n",		// 06
				"m",		// 07
				"LF",		// 08 line feed
				"l",		// 09
				"r",		// 0A
				"g",		// 0B
				"i",		// 0C
				"p",		// 0D
				"c",		// 0E
				"v",		// 0F
				"e",		// 10
				"z",		// 11
				"d",		// 12
				"b",		// 13
				"s",		// 14
				"y",		// 15
				"f",		// 16
				"x",		// 17
				"a",		// 18
				"w",		// 19
				"j",		// 1A
				"FIG",		// 1B figures
				"u",		// 1C
				"q",		// 1D
				"k",		// 1E
				"LTR"       // 1F letters
			},

			// figures
			{
				"",			// 00
				"5",		// 01
				"CR",		// 02 carriage return
				"9",		// 03
				"SP",		// 04 space
				"",			// 05 
				",",		// 06
				".",		// 07
				"LF",		// 08 new line
				")",		// 09
				"4",		// 0A
				"",			// 0B
				"8",		// 0C
				"0",		// 0D
				":",		// 0E
				"=",		// 0F
				"3",		// 10
				"+",		// 11
				"WRU",		// 12 who are you
				"?",		// 13
				"",			// 14 
				"6",		// 15
				"",			// 16 
				"/",		// 17
				"-",		// 18
				"2",		// 19
				"BEL",		// 1A
				"FIG",		// 1B figures
				"7",		// 1C
				"1",		// 1D
				"(",		// 1E
				"LTR"       // 1F letters
			}
		};

		#endregion

		#region US-TTY <-> ASCII for tape punch labeling

		private static string[,] _codeTabPuncherUs =
		{
			{
				"",			// 00 NUL
				"t",		// 01 t
				"CR",		// 02 CR
				"o",		// 03 o
				" ",		// 04 SP
				"h",		// 05 h
				"n",		// 06 n
				"m",		// 07 m
				"LF",		// 08 LF
				"l",		// 09 l
				"r",		// 0A r
				"g",		// 0B g
				"i",		// 0C i
				"p",		// 0D p
				"c",		// 0E c
				"v",		// 0F v
				"e",		// 10 e
				"z",		// 11 z
				"d",		// 12 d
				"b",		// 13 b
				"s",		// 14 s
				"y",		// 15 y
				"f",		// 16 f
				"x",		// 17 x
				"a",		// 18 a
				"w",		// 19 w
				"j",		// 1A j
				"FIG",		// 1B FIG
				"u",		// 1C u
				"q",		// 1D q
				"k",		// 1E k
				"LTR"	    // 1F LTR
			},

			// figures
			{
				"",			// 00 NUL
				"5",		// 01 5
				"CR",		// 02 CR
				"9",		// 03 9
				" ",		// 04 SP
				"#",		// 05 #
				",",		// 06 ,
				".",		// 07 .
				"LF",		// 08 LF
				")",		// 09 )
				"4",		// 0A 4
				"&",		// 0B & (@)
				"8",		// 0C 8
				"0",		// 0D 0
				":",		// 0E :
				";",		// 0F ;
				"3",		// 10 3
				"\"",		// 11 "
				"$",		// 12 $ (Pound)
				"?",		// 13 ?   
				"BEL",		// 14 BEL  
				"6",		// 15 6
				"!",		// 16 ! (%)
				"/",		// 17 /
				"-",		// 18 -
				"2",		// 19 2
				"\'",		// 1A '    
				"FIG",		// 1B FIG
				"7",		// 1C 7
				"1",		// 1D 1
				"(",		// 1E (
				"LTR"		// 1F LTR
			}

		};

		#endregion

		#region Keyboard handling

		/// <summary>
		/// all characters that are to be recognized as input in the terminal windows must be explicitly defined here.
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
					// lowercase letters and numbers
					if (code >= 0x30 && code <= 0x39 || code >= 0x61 && code <= 0x7A)
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
				case '´':
				case '`':
				// control characters
				case '\x05': // ctrl-g ENQ (WRU)
				case '\x07': // ctrl-g BEL
				case '\x09': // ctrl-i here is
				case '\x17': // ctrl-w WRU
					newChar = keyChar;
					break;
			}
			return newChar;
		}

		#endregion
	}
}
