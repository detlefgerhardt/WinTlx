using System.Diagnostics;
using System.Windows.Forms;

namespace WinTlx
{
	class KeyDownConverter
	{
#if false
		public static string ToStr(int keyValue, Keys modifier)
		{
			string str = "";

			switch (modifier)
			{
				case Keys.None:
					switch (keyValue)
					{
						//case 8: // Backspace
						//	break;
						//case 13: // is not handled here
						//	str = "\r\n";
						//	break;
						case 27:
							str = "\x1B";
							break;
						case 32:
							str = " ";
							break;
						case 96:
							str = "0";
							break;
						case 97:
							str = "1";
							break;
						case 98:
							str = "2";
							break;
						case 99:
							str = "3";
							break;
						case 100:
							str = "4";
							break;
						case 101:
							str = "5";
							break;
						case 102:
							str = "6";
							break;
						case 103:
							str = "7";
							break;
						case 104:
							str = "8";
							break;
						case 105:
							str = "9";
							break;
						//case 106:
						//	str = "@";
						//	break;
						case 111:
							str = "\x07";
							break;
						case 107:
						case 187:
							str = "+";
							break;
						case 188:
							str = ",";
							break;
						case 109:
						case 189:
							str = "-";
							break;
						case 219:
							str = "ß";
							break;
						case 190:
							str = ".";
							break;
						case 191:
							str = "#";
							break;
						//case 220:
						//	str = "^";
						//	break;
						case 221:
							str = "'";
							break;
						case 222:
							str = "ä";
							break;
						case 192:
							str = "ö";
							break;
						case 186:
							str = "ü";
							break;
						//case 226:
						//	str = "<";
						//	break;
						default:
							if (keyValue >= 0x30 && keyValue <= 0x39 || keyValue > 0x40 && keyValue <= 0x5A)
								str = ((char)keyValue).ToString().ToLower();
							break;
					}
					break;
				case Keys.Shift:
					switch(keyValue)
					{
						case 8: // Backspace
							str = "xxx ";
							break;
						//case 13: // shift enter = new line (is not handled here)
						//	str = "\n";
						//	break;
						case 16: // right shift
							break;
						case 48:
							str = "=";
							break;
						case 49:
							str = "!";
							break;
						case 50:
							str = "\"";
							break;
						case 51:
							str = "§";
							break;
						case 52:
							str = "$";
							break;
						case 53:
							str = "%";
							break;
						case 54:
							str = "&";
							break;
						case 55:
							str = "/";
							break;
						case 56:
							str = "(";
							break;
						case 57:
							str = ")";
							break;
						case 187:
							str = "*";
							break;
						case 188:
							str = ";";
							break;
						case 189:
							str = "_";
							break;
						case 190:
							str = ":";
							break;
						case 191:
							str = "'";
							break;
						case 219:
							str = "?";
							break;
						case 221:
							str = "'";
							break;
						case 226:
							str = ">";
							break;
						default:
							if (keyValue > 0x40 && keyValue <= 0x5A)
								str = ((char)keyValue).ToString();
							break;
					}
					break;
				case Keys.Control:
					Debug.WriteLine(keyValue);
					switch (keyValue)
					{
						//case 13: // ctrl enter = carriage return (is not handled here)
						//	str = "\r";
						//	break;
						case 73: // ctrl i -> here is
							str = CodeConversion.ASC_HEREIS.ToString();
							break;
						case 87: // ctrl w - WRU
							str = CodeConversion.ASC_WRU.ToString();
							break;
					}
					break;
			}

			return str;
		}
#endif
		public static string ToStr2(Keys keyData)
		{
			string str = "";

			int code1 = (int)(keyData & (~Keys.Shift) & (~Keys.Control) & (~Keys.ControlKey) & (~Keys.Alt));
			if (code1 == 0)
			{
				return "";
			}

			/*
			if ((keyData & Keys.ControlKey) == Keys.ControlKey)
				return "";
			if ((keyData & Keys.Alt) == Keys.Alt)
				return "";
				*/

			if (keyData == Keys.Shift || keyData == Keys.Control || keyData == Keys.Alt ||
				keyData == (Keys.Menu | Keys.Control | Keys.Alt) || keyData == (Keys.ControlKey | Keys.Control | Keys.Alt))
			{
				return str;
			}

			int code = (int)(keyData & (~Keys.Shift));
			Debug.WriteLine($"{keyData} {code}");

			switch (keyData)
			{
				default:
					if (code >= 0x30 && code <= 0x39 || code > 0x40 && code <= 0x5A)
						str = ((char)code).ToString().ToLower();
					else if (code >= 96 && code <= 105)
					{
						// keypad
						str = ((char)(code-48)).ToString().ToLower();
					}
					break;
				//case 8: // Backspace
				//	break;
				//case 13: // is not handled here
				//	str = "\r\n";
				//	break;
				case Keys.G | Keys.Control: // ctrl-g -> BEL
					str = CodeConversion.ASC_BEL.ToString();
					break;
				case Keys.I | Keys.Control: // ctrl-i -> here is
					str = CodeConversion.ASC_HEREIS.ToString();
					break;
				case Keys.W | Keys.Control: // ctrl-w - WRU
					str = CodeConversion.ASC_WRU.ToString();
					break;
				case Keys.Escape:
					str = "";
					break;
				case Keys.Space:
					str = " ";
					break;
				case Keys.Q | Keys.Control | Keys.Alt:
					str = "@";
					break;
				case Keys.Oemplus:
				case Keys.Add:
					str = "+";
					break;
				case Keys.OemMinus:
				case Keys.Subtract:
					str = "-";
					break;
				case Keys.Multiply:
					str = "x";
					break;
				case Keys.Oemcomma:
					str = ",";
					break;
				case Keys.Oemcomma | Keys.Shift:
					str = ";";
					break;
				case Keys.OemPeriod:
					str = ".";
					break;
				case Keys.OemPeriod | Keys.Shift:
					str = ":";
					break;
				case Keys.D1 | Keys.Shift:
					str = "!";
					break;
				case Keys.D2 | Keys.Shift:
					str = "\"";
					break;
				case Keys.D3 | Keys.Shift:
					str = "";
					break;
				case Keys.D4 | Keys.Shift:
					str = "$";
					break;
				case Keys.D5 | Keys.Shift:
					str = "%";
					break;
				case Keys.D6 | Keys.Shift:
					str = "&";
					break;
				case Keys.D7 | Keys.Shift:
				case Keys.Divide:
					str = "/";
					break;
				case Keys.D8 | Keys.Shift:
					str = "(";
					break;
				case Keys.D9 | Keys.Shift:
					str = ")";
					break;
				case Keys.D0 | Keys.Shift:
					str = "=";
					break;
				case Keys.Oem2:
					str = "#";
					break;
				case Keys.Oem2 | Keys.Shift:
					str = "'";
					break;
				case Keys.Oem4:
					str = "ß";
					break;
				case Keys.Oem4 | Keys.Shift:
					str = "?";
					break;
				case Keys.Oem6:
					str = "´";
					break;
				case Keys.Oem6 | Keys.Shift:
					str = "`";
					break;
				case Keys.Oem7:
				case Keys.Oem7 | Keys.Shift:
					str = "ä";
					break;
				case Keys.Oem3:
				case Keys.Oem3 | Keys.Shift:
					str = "ö";
					break;
				case Keys.Oem1:
				case Keys.Oem1 | Keys.Shift:
					str = "ü";
					break;
					/*
						case 219:
							str = "ß";
							break;
						case 190:
							str = ".";
							break;
						case 191:
							str = "#";
							break;
						//case 220:
						//	str = "^";
						//	break;
						case 221:
							str = "'";
							break;
						case 222:
							str = "ä";
							break;
						case 192:
							str = "ö";
							break;
						case 186:
							str = "ü";
							break;
						//case 226:
						//	str = "<";
						//	break;
						default:
							if (keyValue >= 0x30 && keyValue <= 0x39 || keyValue > 0x40 && keyValue <= 0x5A)
								str = ((char)keyValue).ToString().ToLower();
							break;
							*/
			}

			/*
				case Keys.Shift:
					switch (keyValue)
					{
						case 8: // Backspace
							str = "xxx ";
							break;
						//case 13: // shift enter = new line (is not handled here)
						//	str = "\n";
						//	break;
						case 16: // right shift
							break;
						case 48:
							str = "=";
							break;
						case 49:
							str = "!";
							break;
						case 50:
							str = "\"";
							break;
						case 51:
							str = "§";
							break;
						case 52:
							str = "$";
							break;
						case 53:
							str = "%";
							break;
						case 54:
							str = "&";
							break;
						case 55:
							str = "/";
							break;
						case 56:
							str = "(";
							break;
						case 57:
							str = ")";
							break;
						case 187:
							str = "*";
							break;
						case 188:
							str = ";";
							break;
						case 189:
							str = "_";
							break;
						case 190:
							str = ":";
							break;
						case 191:
							str = "'";
							break;
						case 219:
							str = "?";
							break;
						case 221:
							str = "'";
							break;
						case 226:
							str = ">";
							break;
						default:
							if (keyValue > 0x40 && keyValue <= 0x5A)
								str = ((char)keyValue).ToString();
							break;
					}
					break;
				case Keys.Control:
					Debug.WriteLine(keyValue);
					switch (keyValue)
					{
						//case 13: // ctrl enter = carriage return (is not handled here)
						//	str = "\r";
						//	break;
						case 73: // ctrl i -> here is
							str = CodeConversion.ASC_HEREIS.ToString();
							break;
						case 87: // ctrl w - WRU
							str = CodeConversion.ASC_WRU.ToString();
							break;
					}
					break;
			}
			*/
			return str;
		}

	}
}
