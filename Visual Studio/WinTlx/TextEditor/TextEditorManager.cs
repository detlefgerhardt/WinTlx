using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Languages;

namespace WinTlx.TextEditor
{
	class TextEditorManager
	{
		public const int DEFAULT_LINE_LENGTH = 68;

		// delimiters in RichRextBox
		private const string DELIMITER = " -?()";

		public delegate void SendEventHandler(string asciiText);
		public event SendEventHandler Send;

		public const string ALLOWED_CHARS = "abcdefghijklmnopqrstuvwxyzäöüß01234567890/()=?'.,:+- ";

		public bool Saved { get; set; }

		public int CharWidth { get; set; }

		public List<string> UndoStack;
		private int _undoPtr;

		private string _text;
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				if (UndoStack.Count == 0 || value != UndoStack.Last())
				{
					UndoStack.Add(value);
					_undoPtr = UndoStack.Count - 1;
				}
				if (value != _text)
				{ 
					_text = value;
					Saved = false;
				}
			}
		}

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static TextEditorManager instance;

		public static TextEditorManager Instance => instance ?? (instance = new TextEditorManager());

		private TextEditorManager()
		{
			ResetUndo();
			CharWidth = DEFAULT_LINE_LENGTH;
			Text = "";
			Saved = true;

			/*
			Text = 
			"Ich habe alle meine Scans bisher mit dem Original Adobe Acrobat Programm erledigt. Deutsches Wörterbuch ist immer eingeschaltet, ebenso die Scan-Verbessserung, die vor dem OCR Prozess eine evtl. leicht schief eingezogene Seite gerade ausrichtet.\n" +
			"Man könnte es natürlich auch einmal mit dem Finereader machen.\n" +
			"Ich schaue mal nach, ob ich noch die Original-Scandaten, also ohne OCR habe, dann könnte man diese Daten einmal mit einer anderen OCR Software laufen lassen. Kann Euch aber aber erst am Montag berichten, morgen hat meine Frau Geburtstag und es gibt immer noch Präferenzen.\n";
			*/
		}

		public void Undo(string text)
		{
			Debug.WriteLine($"Undo: count={UndoStack.Count} _undoPtr={_undoPtr}");
			if (UndoStack.Count>0 && _undoPtr>0)
			{
				if (UndoStack.Last() != text)
				{
					UndoStack.Add(text);
				}
				_text = UndoStack[--_undoPtr];
				UndoStack.Add(_text);
			}
			Debug.WriteLine($"    count={UndoStack.Count} _undoPtr={_undoPtr}");
		}

		public void Redo()
		{
			Debug.WriteLine($"Redo: count={UndoStack.Count} _undoPtr={_undoPtr}");
			if (UndoStack.Count > 0 && _undoPtr < UndoStack.Count - 1)
			{
				_text = UndoStack[++_undoPtr];
			}
			Debug.WriteLine($"    count={UndoStack.Count} _undoPtr={_undoPtr}");
		}

		private void ResetUndo()
		{
			UndoStack = new List<string>();
			_undoPtr = -1;
		}

		public void LoadFile()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				InitialDirectory = "..",
				Filter = "txt files (*.txt)|*.txt|punch files (*.ls)|*.ls|All files (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true
			};

			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			try
			{
				string ext = Path.GetExtension(openFileDialog.FileName).ToLower();
				if (ext == ".ls")
				{
					ReadPunchFile(openFileDialog.FileName);
				}
				else
				{
					string fullName = openFileDialog.FileName;
					ResetUndo();
					Text = File.ReadAllText(fullName);
				}
				Saved = true;
			}
			catch (Exception)
			{
				ShowError(LanguageManager.Instance.GetText(LngKeys.Editor_LoadError));
			}
		}

		private void ReadPunchFile(string fullName)
		{
			byte[] punchData;
			try
			{
				punchData = File.ReadAllBytes(fullName);
			}
			catch (Exception)
			{
				throw;
			}

			ResetUndo();
			Text = PunchDataToText(punchData);
		}

		private string PunchDataToText(byte[] punchData)
		{
			string text = "";
			ShiftStates shiftState = ShiftStates.Ltr;

			foreach (byte code in punchData)
			{
				if (code == CodeManager.BAU_LTRS)
				{
					shiftState = ShiftStates.Ltr;
				}
				else if (code == CodeManager.BAU_FIGS)
				{
					shiftState = ShiftStates.Figs;
				}
				else
				{
					char ascii = CodeManager.BaudotCharToAscii(code, shiftState, ConfigManager.Instance.Config.CodeSet, CodeManager.SendRecv.Send);
					if (ascii != '\r')
					{
						text += ascii;
					}
				}
			}
			return text;
		}

		public bool SaveFile()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true
			};

			try
			{
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					File.WriteAllText(saveFileDialog.FileName, Text, Encoding.UTF8);
					Saved = true;
				}
				return true;
			}
			catch (Exception)
			{
				ShowError(LanguageManager.Instance.GetText(LngKeys.Editor_SaveError));
				return false;
			}
		}

		public void SendText(string[] lines)
		{
			foreach(string line in lines)
			{
				List<string> newLines = WrapLine(line, CharWidth);
				foreach(string nl in newLines)
				{
					Send?.Invoke(nl + "\r\n");
				}
			}
		}

		public bool Closing()
		{
			if (Saved)
			{
				return true;
			}

			DialogResult result = MessageBox.Show(
				LngText(LngKeys.Editor_NotSavedMessage),
				LngText(LngKeys.Editor_NotSavedHeader),
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button3);

			switch (result)
			{
				case DialogResult.Yes:
					return SaveFile();
				case DialogResult.No:
					return true;
				case DialogResult.Cancel:
				default:
					return false;
			}
		}

		private void ShowError(string text)
		{
			MessageBox.Show(
				text,
				LngText(LngKeys.Editor_Error),
				MessageBoxButtons.OK,
				MessageBoxIcon.Error,
				MessageBoxDefaultButton.Button1);
		}

		public void AlignBlock(string[] lines)
		{
			string newText = "";
			foreach (string line in lines)
			{
				newText += BlockSatz(line, CharWidth);
			}
			Text = newText;
		}

		private string BlockSatz(string textLine, int len)
		{
			if (string.IsNullOrWhiteSpace(textLine))
			{
				return textLine + '\n';
			}

			string newLine = "";
			List<string> textLines = WrapWords(textLine, len);
			for (int l = 0; l < textLines.Count; l++)
			{
				int diff = len - textLines[l].Length;
				if (diff == 0) continue;

				string[] words = textLines[l].Split(' ');

				if (l < textLines.Count - 1)
				{
					while (diff > 0)
					{
						for (int w = 0; w < words.Length - 1; w++)
						{
							words[w] += " ";
							diff--;
							if (diff == 0) break;
						}
					}
				}

				string line = string.Join(" ", words);
				newLine += line + " ";
			}

			return newLine.Trim(' ') + '\n';
		}

		public void AlignLeft(string[] lines)
		{
			string newText = "";
			foreach (string line in lines)
			{
				string[] words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				newText += string.Join(" ", words) + '\n';
			}
			Text = newText.Trim();
		}

		public string[] ShiftLeft(string[] lines)
		{
			List<string> newLines = new List<string>();
			foreach (string line in lines)
			{
				newLines.AddRange(WrapLine(line, CharWidth));
			}

			for (int i = 0; i < newLines.Count; i++)
			{
				if (string.IsNullOrEmpty(newLines[i]))
				{
					continue;
				}
				newLines[i] = newLines[i].Substring(1);
			}
			return newLines.ToArray();
		}

		public string[] ShiftRight(string[] lines)
		{
			List<string> newLines = new List<string>();
			foreach (string line in lines)
			{
				newLines.AddRange(WrapLine(line, CharWidth));
			}

			for (int i = 0; i < newLines.Count; i++)
			{
				if (string.IsNullOrEmpty(newLines[i]))
				{
					continue;
				}
				newLines[i] = " " + newLines[i];
				if (newLines[i].Length > CharWidth)
				{
					newLines[i] = newLines[i].Substring(0, CharWidth);
				}
			}
			return newLines.ToArray();
		}

		/// <summary>
		/// Warp one long line to sevaral shot lines (<=len), additinal spaces are omitted
		/// </summary>
		/// <param name="line"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		private List<string> WrapWords(string line, int len)
		{
			List<AlignItem> words = SplitItems(line);
			List<string> newLines = new List<string>();
			string newLine = "";
			for (int i = 0; i < words.Count; i++)
			{
				if (newLine.Length + words[i].TextWithDelim.Trim().Length <= len)
				{
					newLine += words[i].TextWithDelim;
					continue;
				}

				// line is full
				if (newLine != "")
				{
					newLines.Add(newLine.Trim());
				}
				newLine = words[i].TextWithDelim;
				while (newLine.Length > len)
				{
					newLines.Add(newLine.Substring(0, len).Trim());
					newLine = newLine.Substring(Math.Min(newLine.Length, len));
				}
			}
			if (newLine != "")
			{
				newLines.Add(newLine.Trim());
			}

			return newLines;
		}

		/// <summary>
		/// Warps one long line to sevaral short lines (<=len), additional spaces are preserved
		/// </summary>
		/// <param name="line"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		private List<string> WrapLine(string line, int len)
		{
			if (string.IsNullOrEmpty(line) || line.Length <= len)
			{
				return new List<string>() { line };
			}

			List<string> newLines = new List<string>();
			while (line.Length >= len)
			{
				int pos = -1;
				for (int i = len-1; i > 0; i--)
				{
					Debug.WriteLine(line[i]);
					if (DELIMITER.Contains(line[i]))
					{
						pos = i;
						break;
					}
				}
				if (pos == -1)
				{
					pos = len - 1;
				}
				newLines.Add(line.Substring(0, pos));
				line = line.Substring(pos + 1);
			}
			if (line.Length > 0)
			{
				newLines.Add(line);
			}

			return newLines;
		}

		private List<AlignItem> SplitItems(string line)
		{
			List<AlignItem> items = new List<AlignItem>();
			//bool skipSpace = false;
			string word = "";
			AlignItem item;
			for (int i = 0; i < line.Length; i++)
			{
				char chr = line[i];
				//if (chr == ' ' && skipSpace)
				//{
				//	continue;
				//}
				if (!DELIMITER.Contains(chr))
				{
					word += chr;
					//skipSpace = false;
					continue;
				}

				// delimiter found

				if (word == "" && items.Count > 0 && items.Last().Delimiter == chr)
				{
					if (chr != ' ')
					{
						items.Last().Text += chr;
					}
				}
				else
				{
					item = new AlignItem(word, chr);
					items.Add(item);
				}
				word = "";
				//skipSpace = chr == ' ';
			}
			if (word != "")
			{
				item = new AlignItem(word, ' ');
				items.Add(item);
			}

			return items;
		}

		public void ConvertToBaudot()
		{
			Text = CodeManager.AsciiStringToTelex(Text, ConfigManager.Instance.Config.CodeSet);
		}

		public void ConvertToRtty()
		{
			string convStr = "";
			string str = Text;
			for (int i = 0; i < str.Length; i++)
			{
				char chr = str[i];
				if (chr >= 'a' && chr <= 'z' || chr >= '0' && chr <= '9' || chr == ' ')
				{
					convStr += chr;
					continue;
				}
				if (chr >= 'A' && chr <= 'Z')
				{
					convStr += "8";
					continue;
				}
				if (chr >= 128)
				{
					convStr += ".";
					continue;
				}
				if (chr == '\r' || chr == '\n')
				{
					convStr += chr;
					continue;
				}

				switch (chr)
				{
					case '!':
					case '|':
					case '/':
						chr = '/';
						break;
					case '\\':
						chr = ')';
						break;
					case '"':
					case '\'':
						chr = '\'';
						break;
					case '$':
						chr = '8';
						break;
					case '%':
						chr = '8';
						break;
					case '&':
						chr = 'k';
						break;
					case '(':
					case '[':
					case '{':
					case '<':
						chr = '(';
						break;
					case ')':
					case ']':
					case '}':
					case '>':
						chr = ')';
						break;
					case '@':
						chr = '0';
						break;
					case '#':
						chr = '6';
						break;
					case '+':
					case '*':
						chr = '+';
						break;
					case '-':
					case '~':
					case '_':
						chr = '-';
						break;
					case '=':
						chr = '=';
						break;
					case ',':
					case ';':
						chr = ',';
						break;
					case '.':
						chr = '.';
						break;
					case ':':
						chr = ':';
						break;
					case '?':
						chr = '?';
						break;
					default:
						chr = '.';
						break;
				}
				convStr += chr;

			}
			Text = convStr;
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}
	}
}
