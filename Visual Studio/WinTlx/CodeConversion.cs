using System.Linq;

namespace WinTlx
{
	public static class CodeConversion
	{
		public enum ShiftStates
		{
			Unknown,
			Ltr,
			Figs,
			Both
		}


		public static string BaudotStringToAscii(byte[] baudotData, ref ShiftStates shiftState)
		{
			string asciiStr = "";
			for (int i = 0; i < baudotData.Length; i++)
			{
				byte baudotChr = baudotData[i];
				if (baudotChr == LTR_SHIFT)
				{
					shiftState = ShiftStates.Ltr;
				}
				else if (baudotChr == FIG_SHIFT)
				{
					shiftState = ShiftStates.Figs;
				}
				else
				{
					char asciiChr = BaudotCharToAscii(baudotData[i], shiftState);
					asciiStr += asciiChr;
				}
			}
			return asciiStr;
		}

		public static char BaudotCharToAscii(byte baudotCode, ShiftStates shiftState)
		{
			if (baudotCode > 0x1F)
			{
				return (char)ASC_INV;
			}

			switch (shiftState)
			{
				case ShiftStates.Unknown:
				default:
					return (char)ASC_INV;
				case ShiftStates.Ltr:
					return _codeTabLtrs[baudotCode];
				case ShiftStates.Figs:
					return _codeTabFigs[baudotCode];
			}
		}

		public static string BaudotCodeToPuncherText(byte baudotCode, ShiftStates shiftState)
		{
			switch (shiftState)
			{
				case ShiftStates.Ltr:
					return _codeTabLtrsPuncher[baudotCode];
				case ShiftStates.Figs:
					return _codeTabFigsPuncher[baudotCode];
				default:
					return "";
			}
		}

		public static byte[] AsciiStringToBaudot(string asciiStr, ref ShiftStates shiftState)
		{
			byte[] baudotData = new byte[0];
			for (int i = 0; i < asciiStr.Length; i++)
			{
				string telexData = AsciiCharToTelex(asciiStr[i]);
				byte[] data = TelexStringToBaudot(telexData, ref shiftState);
				baudotData = baudotData.Concat(data).ToArray();
			}
			return baudotData;
		}

		/// <summary>
		/// convert ASCII string to printable characters and replacement characters
		/// </summary>
		/// <param name="asciiStr"></param>
		/// <returns></returns>
		public static string AsciiStringToTelex(string asciiStr)
		{
			string telexStr = "";
			for (int i=0; i<asciiStr.Length; i++)
			{
				telexStr += AsciiCharToTelex(asciiStr[i]);
			}
			return telexStr;
		}

		/// <summary>
		/// convert ASCII character to baudot printable character and replacement character
		/// </summary>
		/// <param name="asciiChr"></param>
		/// <returns></returns>
		private static string AsciiCharToTelex(char asciiChr)
		{
			string asciiData = _codePage437ToAsciiTab(asciiChr);
			string telexData = "";
			for (int i = 0; i < asciiData.Length; i++)
			{
				telexData += _asciiToTelexTab[(int)asciiData[i]];
			}
			return telexData;
		}

		public static byte[] TelexStringToBaudot(string telexStr, ref ShiftStates shiftState)
		{ 
			byte[] buffer = new byte[0];
			for (int i=0; i< telexStr.Length; i++)
			{
				byte[] baudotData = TelexCharToBaudotWithShift(telexStr[i], ref shiftState);
				buffer = buffer.Concat(baudotData).ToArray();
			}
			return buffer;
		}

		/*
		private static byte[] TelexCharToBaudot(char telexChar, ref ShiftStates shiftState)
		{
			int ltrCode = FindBaudot(_codeTabLtrs, telexChar);
			int ltrCode = FindBaudot(_codeTabLtrs, telexChar);

			char[] tab = null;
			switch (shiftState)
			{
				case ShiftStates.Ltr:
					tab = _codeTabLtrs;
					break;
				case ShiftStates.Figs:
					tab = _codeTabFigs;
					break;
			}

			byte[] baudotData = new byte[] { (byte)ASC_INV };
			if (tab == null)
				return baudotData;

			int baudotCode = -1;
			for (int c = 0; c < tab.Length; c++)
			{
				if (tab[c] == telexChar)
				{
					baudotCode = c;
					break;
				}
			}
			if (baudotCode == -1)
				return baudotData; // invalid

			baudotData = BaudotCodeToBufferWithShift((byte)baudotCode, ref shiftState);
			return baudotData;
		}
		*/

#if false
		byte[] buffer = new byte[0];
			for (int i = 0; i < codeData.Length; i++)
			{
				byte[] codes = BaudotCodeToBufferWithShift(codeData[i], ref shiftState);
				if (codes.Length == 0)
				{	// should not happen
					return codes;
				}
				buffer = buffer.Concat(codes).ToArray();

				//int code = codeData[i];
				//if ((code & 0x40) == 0x40)
				//{   // is letters or figures (code in both groups)
				//	buffer = Helper.AddByte(buffer, (byte)(code & 0x1F));
				//}
				//else if ((code & 0x20) == 0x00)
				//{   // is letter
				//	if (shiftState == ShiftStates.Unknown || shiftState == ShiftStates.Figs)
				//	{
				//		buffer = Helper.AddByte(buffer, LTR_SHIFT);
				//		shiftState = ShiftStates.Ltr;
				//	}
				//	buffer = Helper.AddByte(buffer, (byte)(code & 0x1F));
				//}
				//else if ((code & 0x20) == 0x20)
				//{   // is figure 
				//	if (shiftState == ShiftStates.Unknown || shiftState == ShiftStates.Ltr)
				//	{
				//		buffer = Helper.AddByte(buffer, FIG_SHIFT);
				//		shiftState = ShiftStates.Figs;
				//	}
				//	buffer = Helper.AddByte(buffer, (byte)(code & 0x1F));
				//}
				//else
				//{   // is invalid
				//	return new byte[0];
				//}

			}

			return buffer;
		}
#endif

		public static byte[] TelexCharToBaudotWithShift(char telexChr, ref ShiftStates shiftState)
		{
			byte? ltrCode = FindBaudot(_codeTabLtrs, telexChr);
			byte? figCode = FindBaudot(_codeTabFigs, telexChr);
			byte baudCode;
			ShiftStates newShiftState;
			if (ltrCode!=null && figCode!=null)
			{
				baudCode = ltrCode.Value;
				newShiftState = ShiftStates.Both;
			}
			else if (ltrCode!=null)
			{
				baudCode = ltrCode.Value;
				newShiftState = ShiftStates.Ltr;
			}
			else
			{
				baudCode = figCode.Value;
				newShiftState = ShiftStates.Figs;
			}

			return BaudotCodeToBaudotWithShift(baudCode, newShiftState, ref shiftState);
		}

		/*
		public static byte[] BaudotCodeToBaudotWithShift_old(byte? ltrCode, byte? figCode, ref ShiftStates shiftState)
		{ 
			byte[] buffer = new byte[0];

			if (ltrCode == LTR_SHIFT || figCode == LTR_SHIFT)
			{
				shiftState = ShiftStates.Ltr;
				buffer = Helper.AddByte(buffer, LTR_SHIFT);
			}
			else if (ltrCode == FIG_SHIFT || figCode == FIG_SHIFT)
			{
				shiftState = ShiftStates.Ltr;
				buffer = Helper.AddByte(buffer, FIG_SHIFT);
			}

			if (ltrCode != null & figCode != null)
			{   // is letters or figures (code in both groups)
				buffer = Helper.AddByte(buffer, ltrCode.Value);
			}
			else if (ltrCode != null)
			{   // is letter
				if (shiftState == ShiftStates.Unknown || shiftState == ShiftStates.Figs)
				{
					buffer = Helper.AddByte(buffer, LTR_SHIFT);
					shiftState = ShiftStates.Ltr;
				}
				buffer = Helper.AddByte(buffer, ltrCode.Value);
			}
			else if (figCode != null)
			{   // is figure 
				if (shiftState == ShiftStates.Unknown || shiftState == ShiftStates.Ltr)
				{
					buffer = Helper.AddByte(buffer, FIG_SHIFT);
					shiftState = ShiftStates.Figs;
				}
				buffer = Helper.AddByte(buffer, figCode.Value);
			}
			else
			{   // is invalid
				return new byte[0];
			}
			return buffer;
		}
		*/

		public static byte[] BaudotCodeToBaudotWithShift(byte baudCode, ShiftStates newShiftState, ref ShiftStates shiftState)
		{
			byte[] buffer = new byte[0];

			if (baudCode==LTR_SHIFT)
			{
				buffer = Helper.AddByte(buffer, LTR_SHIFT);
				shiftState = ShiftStates.Ltr;
				return buffer;
			}

			if (baudCode == FIG_SHIFT)
			{
				buffer = Helper.AddByte(buffer, FIG_SHIFT);
				shiftState = ShiftStates.Figs;
				return buffer;
			}

			if (shiftState==ShiftStates.Unknown && newShiftState==ShiftStates.Unknown)
			{
				buffer = Helper.AddByte(buffer, LTR_SHIFT);
				newShiftState = ShiftStates.Ltr;
			}

			if (shiftState==ShiftStates.Unknown && newShiftState==ShiftStates.Ltr ||
				shiftState == ShiftStates.Figs && newShiftState == ShiftStates.Ltr)
			{
				buffer = Helper.AddByte(buffer, LTR_SHIFT);
				buffer = Helper.AddByte(buffer, baudCode);
				shiftState = ShiftStates.Ltr;
				return buffer;
			}

			if (shiftState == ShiftStates.Unknown && newShiftState == ShiftStates.Figs ||
				shiftState == ShiftStates.Ltr && newShiftState == ShiftStates.Figs)
			{
				buffer = Helper.AddByte(buffer, FIG_SHIFT);
				buffer = Helper.AddByte(buffer, baudCode);
				shiftState = ShiftStates.Figs;
				return buffer;
			}

			if (shiftState== newShiftState || newShiftState==ShiftStates.Both)
			{
				buffer = Helper.AddByte(buffer, baudCode);
				return buffer;
			}

			// should not happen
			return new byte[0];
		}

		private static byte? FindBaudot(char[] tab, char telexChar)
		{
			for (int c = 0; c < tab.Length; c++)
			{
				if (tab[c] == telexChar)
				{
					return (byte)c;
				}
			}
			return null;
		}

		/// <summary>
		/// Code page 437 to Ascii conversion table
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

		private const char ASC_INV = '#'; // replace invalid baudot character
		public const char ASC_WRU = '\x09'; // Ctrl-I, Kennungsgeber-Abfrage (wer da?)
		public const char ASC_BEL = '\x07'; // Ctrl-G, Bell
		public const char ASC_LTRS = '\x1E';
		public const char ASC_FIGS = '\x1F';

		public const byte BAU_WRU = 0x12; // figure enquiry (wer da?)
		public const byte BAU_BEL = 0x1A; // figure bell
		public const byte BAU_CR = 0x02; // letter+figure carriage return
		public const byte BAU_LF = 0x08; // letter+figure linefeed
		public const byte LTR_SHIFT = 0x1F;
		public const byte FIG_SHIFT = 0x1B;

		#region ASCII -> Telex character set

		private static string[] _asciiToTelexTab =
		{
			"",		// 00 invalid skip
			"",		// 01 invalid skip
			"",		// 02 invalid skip
			"",		// 03 invalid skip
			"",		// 04 invalid skip
			"",		// 05 invalid skip
			"",		// 06 invalid skip
			"\x07",	// 07 ITA2 bel
			"",		// 08 invalid skip
			"\x09",	// 09 ITA2 wru
			"\n",	// 0A line feed
			"",		// 0B invalid skip
			"",		// 0C invalid skip
			"\r",	// 0D carrige return
			"",		// 0E invalid skip
			"",		// 0F invalid skip
			"",		// 10 invalid skip
			"",		// 11 invalid skip
			"",		// 12 invalid skip
			"",		// 13 invalid skip
			"",		// 14 invalid skip
			"",		// 15 invalid skip
			"",		// 16 invalid skip
			"",		// 17 invalid skip
			"",		// 18 invalid skip
			"",		// 19 invalid skip
			"",		// 1A invalid skip
			"",		// 1B invalid skip
			"",		// 1C invalid skip
			"",		// 1D invalid skip
			"\x1E",	// 1E ITA2 letters
			"\x1F", // 1F ITA2 figures
			" ",	// 20 blank
			".",	// 21 ! -> .
			"''",	// 22 " -> ''
			"//",	// 23 # -> //
			"s/",	// 24 $ -> s/
			"o/o",	// 25 % -> o/o
			"+",	// 26 & -> +
			"'",	// 27 '
			"(",	// 28 (
			")",	// 29 )
			"x",	// 2A * -> x
			"+",	// 2B +
			",",	// 2C ,
			"-",	// 2D - 
			".",	// 2E .
			"/",	// 2F /
			"0",	// 30 0
			"1",	// 31 1
			"2",	// 32 2
			"3",	// 33 3
			"4",	// 34 4
			"5",	// 35 5
			"6",	// 36 6
			"7",	// 37 7
			"8",	// 38 8
			"9",	// 39 9
			":",	// 3A :
			",.",	// 3B ; -> ,
			"(.",	// 3C < -> (.
			"=",	// 3D =
			".)",	// 3E > -> .)
			"?",	// 3F ?
			"(at)",	// 40 @ -> (at)
			"a",	// 41 A
			"b",	// 42 B
			"c",	// 43 C
			"d",	// 44 D
			"e",	// 45 E
			"f",	// 46 F
			"g",	// 47 G
			"h",	// 48 H
			"i",	// 49 I
			"j",	// 4A J
			"k",	// 4B K
			"l",	// 4C L
			"m",	// 4D M
			"n",	// 4E N
			"o",	// 4F O
			"p",	// 50 P
			"q",	// 51 Q
			"r",	// 52 R
			"s",	// 53 S
			"t",	// 54 T
			"u",	// 55 U
			"v",	// 56 V
			"w",	// 57 W
			"x",	// 58 X
			"y",	// 59 Y
			"z",	// 5A Z
			"(:",	// 5B [ -> (:
			"/",	// 5C \ -> /
			":)",	// 5D ] -> :)
			"?",	// 5E ^ -> ?
			"-",	// 5F _ -> -
			"'",	// 60 ` -> '
			"a",	// 61 a
			"b",	// 62 b
			"c",	// 63 c
			"d",	// 64 d
			"e",	// 65 e
			"f",	// 66 f
			"g",	// 67 g
			"h",	// 68 h
			"i",	// 69 i
			"j",	// 6A j
			"k",	// 6B k
			"l",	// 6C l
			"m",	// 6D m
			"n",	// 6E n
			"o",	// 6F o
			"p",	// 70 p
			"q",	// 71 q
			"r",	// 72 r
			"s",	// 73 s
			"t",	// 74 t
			"u",	// 75 u
			"v",	// 76 v
			"w",	// 77 w
			"x",	// 78 x
			"y",	// 79 y
			"z",	// 7A z
			"(,",	// 7B { -> (,
			"/",	// 7C | -> /
			",)",	// 7D } -> ,)
			"-",	// 7E ~ -> -
			"",		// 7F invalid skip
		};

		#endregion

		#region Baudot / ITA 2 -> ASCII

		private static char[] _codeTabLtrs =
		{
			ASC_INV,	// 00
			't',		// 01
			'\r',		// 02
			'o',		// 03
			' ',		// 04 blank
			'h',		// 05
			'n',		// 06
			'm',		// 07
			'\n',		// 08
			'l',		// 09
			'r',		// 0A
			'g',		// 0B
			'i',		// 0C
			'p',		// 0D
			'c',		// 0E
			'v',		// 0F
			'e',		// 10
			'z',		// 11
			'd',		// 12
			'b',		// 13
			's',		// 14
			'y',		// 15
			'f',		// 16
			'x',		// 17
			'a',		// 18
			'w',		// 19
			'j',		// 1A
			ASC_FIGS,	// 1B figures
			'u',		// 1C
			'q',		// 1D
			'k',		// 1E
			ASC_LTRS	// 1F letters
		};

		private static char[] _codeTabFigs =
		{
			ASC_INV,	// 00
			'5',		// 01
			'\r',		// 02 carriage return
			'9',		// 03
			' ',		// 04 blank
			ASC_INV,	// 05 $ / pound
			',',		// 06
			'.',		// 07
			'\n',		// 08 new line
			')',		// 09
			'4',		// 0A
			ASC_INV,	// 0B @
			'8',		// 0C
			'0',		// 0D
			':',		// 0E
			'=',		// 0F
			'3',		// 10
			'+',		// 11
			ASC_WRU,	// 12 who are you / enquiry / Kreuz
			'?',		// 13
			'\'',		// 14 or $
			'6',		// 15
			ASC_INV,	// 16 ! / %
			'/',		// 17
			'-',		// 18
			'2',		// 19
			ASC_BEL,	// 1A
			ASC_FIGS,	// 1B figures
			'7',		// 1C
			'1',		// 1D
			'(',		// 1E
			ASC_LTRS	// 1F letters
		};

		#endregion

		#region Baudot / ITA 2 -> Puncher test

		private static string[] _codeTabLtrsPuncher =
		{
			"",			// 00
			"t",		// 01
			"CR",		// 02
			"o",		// 03
			"BL",		// 04 blank
			"h",		// 05
			"n",		// 06
			"m",		// 07
			"LF",		// 08
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
			"LTR"		// 1F letters
		};

		private static string[] _codeTabFigsPuncher =
		{
			"",			// 00
			"5",		// 01
			"CR",		// 02 carriage return
			"9",		// 03
			"BL",		// 04 blank
			"",			// 05 $ / pound
			",",		// 06
			".",		// 07
			"LF",		// 08 new line
			")",		// 09
			"4",		// 0A
			"",			// 0B @
			"8",		// 0C
			"0",		// 0D
			":",		// 0E
			"=",		// 0F
			"3",		// 10
			"+",		// 11
			"WRU",		// 12 who are you
			"?",		// 13
			"",			// 14 or $
			"6",		// 15
			"",			// 16 ! / %
			"/",		// 17
			"-",		// 18
			"2",		// 19
			"BEL",		// 1A
			"FIG",		// 1B figures
			"7",		// 1C
			"1",		// 1D
			"(",		// 1E
			"LTR"		// 1F letters
		};

		#endregion

		public static string DebugBaudotStringToAscii(byte[] baudotData, ShiftStates shiftState)
		{
			string asciiStr = "";
			for (int i = 0; i < baudotData.Length; i++)
			{
				byte baudotChr = baudotData[i];
				if (baudotChr == LTR_SHIFT)
				{
					shiftState = ShiftStates.Ltr;
				}
				else if (baudotChr == FIG_SHIFT)
				{
					shiftState = ShiftStates.Figs;
				}
				else
				{
					string asciiChr = DebugBaudotCharToAscii(baudotData[i], shiftState);
					asciiStr += asciiChr;
				}
			}
			return asciiStr;
		}

		public static string DebugBaudotCharToAscii(byte baudotChar, ShiftStates shiftState)
		{
			if (baudotChar > 0x1F)
			{
				return $"<{baudotChar:X2}>";
			}

			switch (shiftState)
			{
				case ShiftStates.Unknown:
				default:
				case ShiftStates.Ltr:
					return "<is>";
					return DebugTabLtrs[baudotChar];
				case ShiftStates.Figs:
					return DebugTabFigs[baudotChar];
			}
		}

		private static string[] DebugTabLtrs =
		//     0    1    2    3    4    5    6    7    8    9   10   11   12   13   14   15   
			{ "<0>", "t", "<cr>", "o", " ", "h", "n", "m", "<lf>", "l", "r", "g", "i", "p", "c", "v", 

		//    16   17   18   19   20   21   22   23   24   25   26   27   28   29   30   31
			  "e", "z", "d", "b", "s", "y", "f", "x", "a", "w", "j", "<27>", "u", "q", "k", "<31>" };

		private static string[] DebugTabFigs =
		//      0     1    2      3    4     5     6    7    8      9   10    11   12   13   14   15   
			{ "<0>", "5","<cr>", "9", " ", "<5>", ",", ".","<lf>", ")", "4", "<11>", "8", "0", ":", "=", 

		//    16   17    18      19  20   21     22   23   24   25     26       27    28   29   30   31
		      "3", "+", "<WRU>", "?","'", "6", "<22>","/", "-", "2", "<BEL>", "<27>", "7", "1", "(", "<31>" };

	}
}
