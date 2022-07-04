using System.Collections.Generic;
using System.Linq;
using WinTlx.Config;
using WinTlx.Languages;

namespace WinTlx.Codes
{
	//public enum CodeSets { ITA2 = 0, ITA2EXT = 1, USTTY = 2 };
	public enum CodeSets { ITA2 = 0, USTTY = 1, CYRILL = 2 };

	public enum ShiftStates
	{
		Unknown,
		Ltr,
		Figs,
		Third,
		Both
	}

	//public enum ThirdLevelStates { Inactive = 0, Active = 1}

	public static class CodeManager
	{
		// F=Quadrat ohne Inhalt., G=Quadrat mit Querstrich, H=Quadrat mit Schrägstrich

		// special ASCII codes
		public const char ASC_INVALID = '~'; // replace invalid baudot character
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
		public const char ASC_SPC = '\x20';

		// special ITA2 codes
		public const byte BAU_NUL = 0x00;
		public const byte BAU_CR = 0x02;
		public const byte BAU_SPC = 0x04;
		public const byte BAU_LF = 0x08;
		public const byte BAU_WRU = 0x12;
		public const byte BAU_BEL = 0x1A;
		public const byte BAU_FIGS = 0x1B;
		public const byte BAU_LTRS = 0x1F;

		public const byte BAU_SHIFTF = 0x16;
		public const byte BAU_SHIFTG = 0x0B;
		public const byte BAU_SHIFTH = 0x05;

		public enum SendRecv { Send, Recv };

		public static string BaudotStringToAscii(byte[] baudotData, KeyStates keyStates, SendRecv sendRecv, bool debug)
		{
			string asciiStr = "";
			for (int i = 0; i < baudotData.Length; i++)
			{
				byte baudotChr = baudotData[i];
				if (baudotChr == BAU_LTRS)
				{
					keyStates.ShiftState = ShiftStates.Ltr;
					if (debug) asciiStr += ASC_LTRS;
				}
				else if (baudotChr == BAU_FIGS)
				{
					keyStates.ShiftState = ShiftStates.Figs;
					if (debug) asciiStr += ASC_FIGS;
				}
				else if (baudotChr == BAU_NUL)
				{
					bool hasThirdLevel = GetCodeTab(keyStates.CodeSet).HasThirdLevel;
					if (hasThirdLevel)
					{
						keyStates.ShiftState = ShiftStates.Third;
					}
					if (debug) asciiStr += ASC_NUL;
				}
				else
				{
					char asciiChr = BaudotCharToAscii(baudotData[i], keyStates, sendRecv);
					asciiStr += asciiChr;
				}
			}
			return asciiStr;
		}

		public static char BaudotCharToAscii(byte baudotCode, KeyStates keyStates, SendRecv sendRecv)
		{
			if (baudotCode > 0x1F)
			{
				return ASC_INVALID;
			}

			CodeItem codeItem = GetCodeItem(keyStates.CodeSet, baudotCode);
			return codeItem.GetChar(keyStates.ShiftState);
		}

		public static string BaudotCodeToPuncherText(byte baudotCode, KeyStates keyStates)
		{
			if (baudotCode > 0x1F)
			{
				return ASC_INVALID.ToString();
			}

			CodeItem codeItem = GetCodeItem(keyStates.CodeSet, baudotCode);
			char codeChar = codeItem.GetChar(keyStates.ShiftState);
			string codeName = GetCodeName(codeChar, LanguageManager.Instance.CurrentLanguage.Key);
			if (!string.IsNullOrEmpty(codeName)) return codeName;
			return codeItem.GetName(keyStates.ShiftState);
		}

		public static byte[] AsciiStringToBaudot(string asciiStr, KeyStates keyStates)
		{
			byte[] baudotData = new byte[0];
			for (int i = 0; i < asciiStr.Length; i++)
			{
				char chr = asciiStr[i];
				byte[] data;
				if (chr >= 128 && chr <= 255)
				{
					data = new byte[] { (byte)(chr - 128) };
				}
				else
				{
					string telexData = AsciiCharToTelex(asciiStr[i], keyStates.CodeSet);
					data = TelexStringToBaudot(telexData, keyStates);
				}
				baudotData = baudotData.Concat(data).ToArray();
			}
			return baudotData;
		}

		/// <summary>
		/// convert ASCII string to printable characters and replacement characters
		/// </summary>
		/// <param name="asciiStr"></param>
		/// <param name="nobrackets">true: do not convert various forms of brackets</param>
		/// <returns></returns>
		public static string AsciiStringToTelex(string asciiStr, CodeSets codeSet, bool noBracketConversion = false)
		{
			string telexStr = "";
			for (int i = 0; i < asciiStr.Length; i++)
			{
				telexStr += AsciiCharToTelex(asciiStr[i], codeSet, noBracketConversion);
			}
			return telexStr;
		}

		public static string AsciiWithBaudotEscCodeToAscii(string asciiStrWithEscCodes, KeyStates keyStates)
		{
			string asciiStr = "";
			foreach(char chr in asciiStrWithEscCodes)
			{
				if (chr >= 128 && chr <= 255)
				{
					byte code = (byte)(chr - 128);
					asciiStr += BaudotStringToAscii(new byte[] { code }, keyStates, SendRecv.Send, false);
				}
				else
				{
					asciiStr += chr;
				}
			}
			return asciiStr;
		}

		/*
		/// <summary>
		/// check if there is a baudot escape sequence ("\xx") at position pos an return value
		/// </summary>
		/// <param name="asciiStr"></param>
		/// <param name="pos"></param>
		/// <returns></returns>
		public static byte? GetBaudotEscCode(string asciiStr, int pos)
		{
			if (asciiStr[pos] != '\\') return null;
			if (pos + 2 >= asciiStr.Length) return null;
			if (int.TryParse(asciiStr.Substring(pos + 1, 2), out int code))
			{
				return code < 32 ? (byte?)code : null;
			}
			return null;
		}
		*/

		private const string BRACKETS = "[]{}<>";

		/// <summary>
		/// convert any ASCII character to a baudot printable ASCII character or replacement character
		/// </summary>
		/// <param name="asciiChr"></param>
		/// <param name="nobracketConversion">true: do not convert various forms of brackets</param>
		/// <returns></returns>
		public static string AsciiCharToTelex(char asciiChr, CodeSets codeSet, bool noBracketConversion = false)
		{
			if (noBracketConversion && BRACKETS.Contains(asciiChr)) return "";

			ICodeTab codeTab = GetCodeTab(codeSet);
			string asciiData = CodePageToPlainAscii(asciiChr, codeSet);
			string telexData = "";
			for (int i = 0; i < asciiData.Length; i++)
			{
				foreach (AsciiConvItem convItem in codeTab.AsciiTab)
				{
					string ascii = convItem.GetCodeInRange(asciiData[i]);
					if (!string.IsNullOrEmpty(ascii)) telexData += ascii;
				}
			}
			return telexData;
		}

		public static byte[] TelexStringToBaudot(string telexStr, KeyStates keyStates)
		{
			byte[] buffer = new byte[0];
			for (int i = 0; i < telexStr.Length; i++)
			{
				byte[] baudotData = TelexCharToBaudotWithShift(telexStr[i], keyStates);
				buffer = buffer.Concat(baudotData).ToArray();
			}
			return buffer;
		}

		public static byte[] TelexCharToBaudotWithShift(char telexChr, KeyStates keyStates)
		{
			byte? ltrCode = FindBaudot(telexChr, ShiftStates.Ltr, keyStates.CodeSet);
			byte? figCode = FindBaudot(telexChr, ShiftStates.Figs, keyStates.CodeSet);
			byte? thirdLevel = FindBaudot(telexChr, ShiftStates.Third, keyStates.CodeSet);
			byte baudCode;
			ShiftStates newShiftState;
			if (ltrCode != null && figCode != null)
			{
				baudCode = ltrCode.Value;
				newShiftState = ShiftStates.Both;
			}
			//else if (keyStates.ShiftState == ShiftStates.Third && ltrCode != null)
			//{
			//	baudCode = ltrCode.Value;
			//	newShiftState = ShiftStates.Third;
			//}
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
			else if (thirdLevel != null)
			{
				baudCode = thirdLevel.Value;
				newShiftState = ShiftStates.Third;
			}
			else
			{
				return new byte[0];
			}

			return BaudotCodeToBaudotWithShift(baudCode, newShiftState, keyStates);
		}

		public static byte[] BaudotCodeToBaudotWithShift(byte baudCode, ShiftStates newShiftState, KeyStates keyStates)
		{
			byte[] buffer = new byte[0];

			if (baudCode == BAU_LTRS)
			{
				buffer = Helper.AddByte(buffer, BAU_LTRS);
				keyStates.ShiftState = ShiftStates.Ltr;
				return buffer;
			}
			if (baudCode == BAU_FIGS)
			{
				buffer = Helper.AddByte(buffer, BAU_FIGS);
				keyStates.ShiftState = ShiftStates.Figs;
				return buffer;
			}
			if (baudCode == BAU_NUL)
			{
				bool hasThirdLevel = GetCodeTab(keyStates.CodeSet).HasThirdLevel;
				if (hasThirdLevel)
				{
					keyStates.ShiftState = ShiftStates.Third;
				}
				buffer = Helper.AddByte(buffer, BAU_NUL);
				return buffer;
			}

			if (keyStates.ShiftState == ShiftStates.Unknown && newShiftState == ShiftStates.Unknown)
			{
				buffer = Helper.AddByte(buffer, BAU_LTRS);
				newShiftState = ShiftStates.Ltr;
			}

			if (newShiftState == ShiftStates.Ltr && keyStates.ShiftState != ShiftStates.Ltr)
			{
				buffer = Helper.AddByte(buffer, BAU_LTRS);
				buffer = Helper.AddByte(buffer, baudCode);
				keyStates.ShiftState = ShiftStates.Ltr;
				return buffer;
			}

			if (newShiftState == ShiftStates.Figs && keyStates.ShiftState != ShiftStates.Figs)
			{
				buffer = Helper.AddByte(buffer, BAU_FIGS);
				buffer = Helper.AddByte(buffer, baudCode);
				keyStates.ShiftState = ShiftStates.Figs;
				return buffer;
			}

			if (newShiftState == ShiftStates.Third && keyStates.ShiftState != ShiftStates.Third)
			{
				buffer = Helper.AddByte(buffer, BAU_NUL);
				buffer = Helper.AddByte(buffer, baudCode);
				keyStates.ShiftState = ShiftStates.Third;
				return buffer;
			}

			if (keyStates.ShiftState == newShiftState || newShiftState == ShiftStates.Both)
			{
				buffer = Helper.AddByte(buffer, baudCode);
				return buffer;
			}

			// should not happen
			return new byte[0];
		}

		public static string AsciiToDebugStr(string ascStr)
		{
			string newStr = "";
			for (int i = 0; i < ascStr.Length; i++)
			{
				char ascChr = ascStr[i];
				int ascCode = (int)ascChr;
				string newChr;
				if (ascCode < 32)
				{
					switch (ascCode)
					{
						case ASC_NUL:
							newChr = "[NU]";
							break;
						case ASC_WRU:
							newChr = "[WRU]";
							break;
						case ASC_BEL:
							newChr = "[KL]";
							break;
						case ASC_LF:
							newChr = "[LF]";
							break;
						case ASC_CR:
							newChr = "[CR]";
							break;
						case ASC_LTRS:
							newChr = "[BU]";
							break;
						case ASC_FIGS:
							newChr = "[ZI]";
							break;
						default:
							newChr = $"{ascCode:X02}";
							break;
					}
				}
				else
				{
					newChr = ascChr.ToString();
				}
				newStr += newChr;
			}
			return newStr;
		}

		private static byte? FindBaudot(char asciiChar, ShiftStates shiftState, CodeSets codeSet)
		{
			if (codeSet == CodeSets.CYRILL && ConfigManager.Instance.Config.CodeSet != CodeSets.CYRILL) return null;

			for (byte c = 0; c < 32; c++)
			{
				CodeItem codeItem = GetCodeItem(codeSet, c);
				char chr = codeItem.GetChar(shiftState);
				if (chr == asciiChar)
				{
					return c;
				}
			}
			return null;
		}

		/*
		public static CodeItem FindBaudot(char asciiChar, KeyStates keyStates)
		{
			for (byte c = 0; c < 32; c++)
			{
				CodeItem codeItem = GetCodeItem(keyStates.CodeSet, c);

				if (codeItem.GetChar(ShiftStates.Ltr, keyStates.ThirdLevelState) == asciiChar)
				{
					keyStates.ShiftState = ShiftStates.Ltr;
					return codeItem;
				}
				if (codeItem.GetChar(ShiftStates.Figs, keyStates.ThirdLevelState) == asciiChar)
				{
					keyStates.ShiftState = ShiftStates.Figs;
					return codeItem;
				}
			}
			keyStates.ShiftState = ShiftStates.Unknown;
			return null;
		}
		*/

		private static string CodePageToPlainAscii(char asciiChar, CodeSets codeSet)
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
					else if (asciiChar >= 0x0410 && asciiChar <= 0x044F)
					{
						return asciiChar.ToString();
					}
					else
					{
						return "";
					}
			}

			/*
			switch (codeSet)
			{
				case CodeSets.ITA2:
				case CodeSets.USTTY:
				default:
					return CodePage437ToPlainAscii(asciiChar);
				case CodeSets.CYRILL:
					return CodeCyrillToPlanAscii(asciiChar);
			}
			*/
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

		public static byte[] MirrorByteArray(byte[] buffer)
		{
			byte[] newBuffer = new byte[buffer.Length];
			for (int i = 0; i < buffer.Length; i++)
			{
				newBuffer[i] = MirrorCode(buffer[i]);
			}
			return newBuffer;
		}

		public static byte MirrorCode(byte code)
		{
			byte inv = 0;
			for (int i = 0; i < 5; i++)
			{
				if ((code & (1 << i)) != 0)
				{
					inv = (byte)(inv | (1 << (4 - i)));
				}
			}
			return inv;
		}

		public static CodeItem GetCodeItem(CodeSets codeSet, byte code)
		{
			return GetCodeTab(codeSet).CodeTab[code];
		}

		private static ICodeTab GetCodeTab(CodeSets codeSet)
		{
			switch (codeSet)
			{
				case CodeSets.ITA2:
				default:
					return new CodeTabIta2();
				case CodeSets.USTTY:
					return new CodeTabUstty();
				case CodeSets.CYRILL:
					return new CodeTabCyrill();
			}
		}

		private static Dictionary<char, string> _codeNamesDe = new Dictionary<char, string>()
		{
			{ ASC_NUL, "NUL" },
			{ ASC_CR, "WR" },
			{ ASC_SPC, "ZWR" },
			{ ASC_LF, "ZL" },
			{ ASC_WRU, "WRU" },
			{ ASC_BEL, "KL" },
			{ ASC_FIGS, "Zi" },
			{ ASC_LTRS, "Bu" },
		};

		private static Dictionary<char, string> _codeNamesEn = new Dictionary<char, string>()
		{
			{ ASC_NUL, "NUL" },
			{ ASC_CR, "CR" },
			{ ASC_SPC, "SP" },
			{ ASC_LF, "LF" },
			{ ASC_WRU, "WRU" },
			{ ASC_BEL, "BEL" },
			{ ASC_FIGS, "FIGS" },
			{ ASC_LTRS, "LTRS" },
		};

		private static Dictionary<char, string> _codeNamesRu = new Dictionary<char, string>()
		{
			{ ASC_NUL, "PYC" },
			{ ASC_CR, "CR" },
			{ ASC_SPC, "SP" },
			{ ASC_LF, "LF" },
			{ ASC_WRU, "WRU" },
			{ ASC_BEL, "BEL" },
			{ ASC_FIGS, "1..." },
			{ ASC_LTRS, "LAT" },
		};

		public static string GetCodeName(char code, string lngKey)
		{
			Dictionary<char, string> codeNames;
			switch (lngKey)
			{
				case "de":
				default:
					codeNames = _codeNamesDe;
					break;
				case "en":
					codeNames = _codeNamesEn;
					break;
				case "ru":
					codeNames = _codeNamesRu;
					break;
			}

			if (codeNames.ContainsKey(code)) return codeNames[code];
			return null;
		}

		private const string DumpBaudotChrs = ".E.A.SIU.DRJNFCKTZLWHYPQOBG.MXV.";
		private const string DumpBaudotFigs = ".3.-.'87..4.,.:(5+)2.6019?.../: ";

		public static string DumpBaudotArrayToString(byte[] buffer)
		{
			string str1 = "";
			string str2 = "";
			for (int i = 0; i < buffer.Length; i++)
			{
				if (str1 != "") str1 += ",";
				str1 += $"{buffer[i]:D2}";
				str2 += DumpBaudotChrs[buffer[i]];
			}
			return str1 + " " + str2;
		}



		/*
		private static readonly CodeItem[] _codeTab = new CodeItem[32]
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
		*/

		#region Keyboard handling

		/// <summary>
		/// all characters that are to be recognized as input in the terminal windows must be explicitly
		/// defined here.
		/// </summary>
		/// <param name="keyChar"></param>
		/// <returns></returns>
		public static char? KeyboardCharacters(char? keyChar)
		{
			if (keyChar == null) return null;

			int code = (int)keyChar.Value;
			char? newChar = null;

			// all characters that are to be recognized as input in the terminal windows must be explicitly defined here.
			switch (char.ToLower(keyChar.Value))
			{
				default:
					// letters and numbers
					if (code >= 0x30 && code <= 0x39 || code >= 0x41 && code <= 0x5A || code >= 0x61 && code <= 0x7A)
					{
						newChar = keyChar;
					}
					else if (code >= 0x0400 && code <= 0x044F)
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
				case '"':
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

	interface ICodeTab
	{
		bool HasThirdLevel { get; }

		AsciiConvItem[] AsciiTab { get; }

		CodeItem[] CodeTab { get; }
	}

	/*
	class CodeTabBase : ICodeTab
	{
		public AsciiConvItem[] GetAsciiTab()
		{
			return AsciiTab;
		}

		public CodeItem[] GetCodeTab()
		{
			return CodeTab;
		}

		public bool GetThirdLevelState()
		{
			return THIRD_LEVEL_STATE;
		}
	}
	*/
}
