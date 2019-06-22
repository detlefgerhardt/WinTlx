using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace WinTlx
{
	class KeyDownConverter
	{
		public static char? AsciiKeyToChar(char keyChar)
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
	}
}
