using System.Windows.Forms;

namespace WinTelex
{
	class KeyDownConverter
	{
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
						case 13:
							str = "\r\n";
							break;
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
						case 106:
							str = "@";
							break;
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
						case 226:
							str = ">";
							break;
						default:
							if (keyValue > 0x40 && keyValue <= 0x5A)
								str = ((char)keyValue).ToString();
							break;
					}
					break;
			}

			//Debug.WriteLine($"{keyValue} {modifier} {str}");

			return str;
		}
	}
}
