using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Debugging;
using WinTlx.Languages;

namespace WinTlx.TextEditor
{
	public enum Cmds { Wru, HereIs, Delay, Wait, SendFile, Send, Dial, Disconnect }

	public enum CmdParamTypes { None, Str, Num };

	class TextEditorManager
	{
		private readonly List<CmdItem> _cmdList = new List<CmdItem>()
		{
			new CmdItem(Cmds.Wru, "wru"),
			new CmdItem(Cmds.HereIs, "hereis"),
			new CmdItem(Cmds.Delay, "delay", CmdParamTypes.Num),
			new CmdItem(Cmds.Wait, "wait", new CmdParamTypes[] { CmdParamTypes.Str, CmdParamTypes.Num }),
			new CmdItem(Cmds.SendFile, "sendfile", CmdParamTypes.Str),
			new CmdItem(Cmds.Send, "send", CmdParamTypes.Str),
			new CmdItem(Cmds.Dial, "dial", new CmdParamTypes[] { CmdParamTypes.Num, CmdParamTypes.Num }),
			new CmdItem(Cmds.Disconnect, "disconnect"),
		};

		public const int DEFAULT_LINE_LENGTH = 68;

		// delimiters in RichRextBox
		private const string DELIMITER = " -?()";
		private readonly List<DelimiterItem> _delimiters = new List<DelimiterItem>()
		{
			new DelimiterItem(' ', WrapMode.Before),
			new DelimiterItem('-', WrapMode.Both), // WarpMode depends on space before (space before: before, no space before: after)
			new DelimiterItem('+', WrapMode.Before),
			new DelimiterItem('(', WrapMode.Before),
			new DelimiterItem('.', WrapMode.After),
			new DelimiterItem(',', WrapMode.After),
			new DelimiterItem(':', WrapMode.After),
			new DelimiterItem('?', WrapMode.After),
			new DelimiterItem(')', WrapMode.After),
			new DelimiterItem('=', WrapMode.After),
		};

		private readonly ConfigManager _configManager;

		private readonly BufferManager _bufferManager;

		private readonly ItelexProtocol _itelex;

		private bool _stopScript;

		private string _recvString = null;

		public delegate void DialEventHandler(string number);
		public event DialEventHandler Dial;

		public delegate void DisconnectEventHandler();
		public event DisconnectEventHandler Disconnect;

		public delegate void SavedStatusChangedEventHandler();
		public event SavedStatusChangedEventHandler SavedStatusChanged;

		public const string ALLOWED_CHARS = "abcdefghijklmnopqrstuvwxyzäöüß01234567890/()=?'.,:+- ";

		private bool _saved;
		public bool Saved
		{
			get
			{
				return _saved;
			}
			set
			{
				bool changed = value != _saved;
				_saved = value;
				if (changed) SavedStatusChanged?.Invoke();
			}
		}

		private string _filename;
		public string Filename
		{
			get
			{
				return _filename;
			}
			set
			{
				bool changed = value != _filename;
				_filename = value;
				if (changed) SavedStatusChanged?.Invoke();
			}
		}

		public int LineWidth { get; set; }

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
			_configManager = ConfigManager.Instance;

			_bufferManager = BufferManager.Instance;

			_itelex = ItelexProtocol.Instance;
			_itelex.Received += Itelex_Received;
			_itelex.Dropped += Itelex_Dropped;

			ResetUndo();
			LineWidth = DEFAULT_LINE_LENGTH;
			Text = "";
			Saved = true;
			Filename = null;
		}

		public void Undo(string text)
		{
			//Debug.WriteLine($"Undo: count={UndoStack.Count} _undoPtr={_undoPtr}");
			if (UndoStack.Count > 0 && _undoPtr > 0)
			{
				if (UndoStack.Last() != text)
				{
					UndoStack.Add(text);
				}
				_text = UndoStack[--_undoPtr];
				UndoStack.Add(_text);
			}
			//Debug.WriteLine($"    count={UndoStack.Count} _undoPtr={_undoPtr}");
		}

		public void Redo()
		{
			//Debug.WriteLine($"Redo: count={UndoStack.Count} _undoPtr={_undoPtr}");
			if (UndoStack.Count > 0 && _undoPtr < UndoStack.Count - 1)
			{
				_text = UndoStack[++_undoPtr];
			}
			//Debug.WriteLine($"    count={UndoStack.Count} _undoPtr={_undoPtr}");
		}

		private void ResetUndo()
		{
			UndoStack = new List<string>();
			_undoPtr = -1;
		}

		public void LoadFile(string fullName = null)
		{
			if (string.IsNullOrEmpty(fullName))
			{
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					InitialDirectory = "..",
					Filter = "txt files (*.txt)|*.txt|punch files (*.ls)|*.ls|All files (*.*)|*.*",
					FilterIndex = 1,
					RestoreDirectory = true,
				};

				if (openFileDialog.ShowDialog() != DialogResult.OK) return;
				fullName = openFileDialog.FileName;
			}

			try
			{
				string ext = Path.GetExtension(fullName).ToLower();
				if (ext == ".ls")
				{
					ReadPunchFile(fullName);
				}
				else
				{
					ResetUndo();
					Text = ConvertText(File.ReadAllText(fullName), ConfigManager.Instance.Config.CodeSet);
					Filename = fullName;
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
			KeyStates keyStates = new KeyStates(ShiftStates.Ltr, ConfigManager.Instance.Config.CodeSet);

			foreach (byte code in punchData)
			{
				if (code == CodeManager.BAU_LTRS)
				{
					keyStates.ShiftState = ShiftStates.Ltr;
				}
				else if (code == CodeManager.BAU_FIGS)
				{
					keyStates.ShiftState = ShiftStates.Figs;
				}
				else
				{
					char ascii = CodeManager.BaudotCharToAscii(code, keyStates, CodeManager.SendRecv.Send);
					if (ascii != '\r')
					{
						text += ascii;
					}
				}
			}
			return text;
		}

		public bool SaveFile(string fullname=null)
		{
			if (string.IsNullOrEmpty(fullname))
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
					FilterIndex = 1,
					RestoreDirectory = true,
					FileName = Path.GetFileName(Filename),
				};

				if (saveFileDialog.ShowDialog() != DialogResult.OK) return false;
				fullname = saveFileDialog.FileName;
			}

			try
			{
				File.WriteAllText(fullname, Text, Encoding.UTF8);
				Saved = true;
				Filename = Path.GetFileName(fullname);
				return true;
			}
			catch (Exception)
			{
				ShowError(LanguageManager.Instance.GetText(LngKeys.Editor_SaveError));
				return false;
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
				newText += BlockSatz(line, LineWidth, "\n", false);
			}
			Text = newText;
		}

		/// <summary>
		/// block align one line. if line is longer than len then the last part is not aligned.
		/// with force=true also the last part is aligned
		/// </summary>
		/// <param name="textLine"></param>
		/// <param name="len"></param>
		/// <param name="nl"></param>
		/// <param name="force"></param>
		/// <returns></returns>
		public string BlockSatz(string textLine, int len, string nl, bool force)
		{
			if (string.IsNullOrWhiteSpace(textLine) || textLine.Length == len)
			{
				return textLine + nl;
			}

			string newLine = "";
			List<string> textLines = WrapWords(textLine, len);
			for (int l = 0; l < textLines.Count; l++)
			{
				int diff = len - textLines[l].Length;
				if (diff == 0) continue;

				string[] words = textLines[l].Split(' ');

				if (force || l < textLines.Count - 1)
				{
					while (diff > 0 && words.Length > 1)
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

			return newLine.Trim(' ') + nl;
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
				newLines.AddRange(WrapLine(line, LineWidth));
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
				newLines.AddRange(WrapLine(line, LineWidth));
			}

			for (int i = 0; i < newLines.Count; i++)
			{
				if (string.IsNullOrEmpty(newLines[i])) continue;

				newLines[i] = " " + newLines[i];
				if (newLines[i].Length > LineWidth)
				{
					newLines[i] = newLines[i].Substring(0, LineWidth);
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
		public List<string> WrapWords(string line, int len)
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
				if (newLine != "") newLines.Add(newLine.Trim());
				newLine = words[i].TextWithDelim;
				while (newLine.Length > len)
				{
					newLines.Add(newLine.Substring(0, len).Trim());
					newLine = newLine.Substring(Math.Min(newLine.Length, len));
				}
			}
			if (newLine != "") newLines.Add(newLine.Trim());

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
			// char lastChar = '\0';
			while (line.Length >= len)
			{
				int pos = -1;
				for (int i = len - 1; i > 0; i--)
				{
					DelimiterItem delim = _delimiters.Find(d => d.Char == line[i]);
					if (delim != null)
					{
						WrapMode wrapMode = delim.WrapMode;
						if (wrapMode == WrapMode.Both)
						{
							char charBefore = line[i - 1];
							wrapMode = charBefore == ' ' ? WrapMode.Before : WrapMode.After;
						}
						pos = wrapMode == WrapMode.Before ? i : i + 1;
						break;
					}
				}
				if (pos == -1) pos = len - 1;

				if (newLines.Count==0)
				{   // keep the indentation in the first line
					line = line.TrimEnd();
				}
				else
				{
					// no indentation in wrapped lines
					line = line.Trim();
				}
				newLines.Add(line.Substring(0, pos));
				line = line.Substring(pos).Trim();
			}
			if (line.Length > 0) newLines.Add(line);

			return newLines;
		}

		private List<AlignItem> SplitItems(string line)
		{
			List<AlignItem> items = new List<AlignItem>();
			string word = "";
			AlignItem item;
			for (int i = 0; i < line.Length; i++)
			{
				char chr = line[i];
				if (!DELIMITER.Contains(chr))
				{
					word += chr;
					continue;
				}

				// delimiter found

				if (word == "" && items.Count > 0 && items.Last().Delimiter == chr)
				{
					if (chr != ' ') items.Last().Text += chr;
				}
				else
				{
					item = new AlignItem(word, chr);
					items.Add(item);
				}
				word = "";
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

		public string ConvertText(string text, CodeSets codeSet)
		{
			string newText = "";
			foreach (char chr in text)
			{
				newText += ConvertTextChar(chr, codeSet);
			}
			return newText;
		}

		public string ConvertTextChar(char chr, CodeSets codeSet)
		{
			string scripChar = @"{}\";

			if (scripChar.Contains(chr)) return chr.ToString();

			return CodeManager.AsciiCharToTelex(chr, codeSet, true);
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

		public async Task SendTextAsync(string[] lines)
		{
			_stopScript = false;
			bool script = false;

			foreach (string line in lines)
			{
				if (line.StartsWith("{script}"))
				{
					script = true;
					continue;
				}
				if (line.StartsWith("{endscript}"))
				{
					script = false;
					continue;
				}

				if (!script)
				{
					List<string> newLines = WrapLine(line, LineWidth);
					/*
					for (int i=0; i<newLines.Count; i++)
					{
						if (_stopScript) return;
						//await SendTextLine(nl.TrimEnd() + "\r\n");
						if (i==0)
						{
							newLines[i] = newLines[i].Trim();
						}
						else
						{
							newLines[i] = newLines[i].TrimEnd();
						}
						await SendTextLine(newLines[i].Trim() + "\r\n");
					}
					*/
					foreach(string nl in newLines)
					{
						if (_stopScript) return;
						await SendTextLine(nl + "\r\n");
					}
				}
				else
				{
					string scriptLine = line.Trim();
					if (!string.IsNullOrEmpty(scriptLine) && !scriptLine.StartsWith("//"))
					{
						//SendDebugText(scriptLine, DebugForm.Modes.Command);
						await DoScriptCmd(scriptLine);
					}
				}
			}
		}

		private async Task SendTextLine(string line)
		{
			Debug.WriteLine($"SendTextLine {line}");
			while (true)
			{
				if (_stopScript) return;

				int pos = line.IndexOf('{');
				if (pos == -1)
				{
					await SendLocalAndText(line);
					break;
				}

				string line1 = line.Substring(0, pos);
				string line2 = line.Substring(pos + 1);
				int pos2 = line2.IndexOf('}');
				if (pos2 == -1)
				{
					await SendLocalAndText(line);
					break;
				}
				await SendLocalAndText(line1);
				//await _bufferManager.WaitLocalOutpuBufferEmpty();
				string cmd = line2.Substring(0, pos2);
				await DoScriptCmd(cmd);
				await _bufferManager.WaitSendBufferEmpty();
				line = line2.Substring(pos2 + 1);
			}
		}

		private async Task DoScriptCmd(string cmdStr)
		{
			if (string.IsNullOrWhiteSpace(cmdStr))
			{
				Debug.Write("");
				return;
			}

			cmdStr = cmdStr.Trim();
			CmdItem cmdItem = null;
			foreach (CmdItem cmd in _cmdList)
			{
				if (cmdStr.Length >= cmd.Name.Length && cmdStr.Substring(0, cmd.Name.Length) == cmd.Name)
				{
					cmdItem = cmd;
					break;
				}
			}

			CmdParameter[] prms = null;
			bool error;
			if (cmdItem == null)
			{
				error = true;
			}
			else
			{
				prms = ParseCmdParams(cmdStr, cmdItem.ParamList, out error);
			}

			if (!error)
			{
				//await SendLocalMsg($"@{cmdStr}@");
			}
			else
			{
				await SendLocalMsg($"?{cmdStr}?");
				return;
			}

			if (cmdItem == null) return;

			Debug.WriteLine($"Cmd {cmdItem.Name} {cmdStr}");

			bool result = true;
			switch (cmdItem.Type)
			{
				case Cmds.Wru:
					await SendWru();
					break;
				case Cmds.HereIs:
					await SendAnswerBack();
					break;
				case Cmds.Delay:
					await Task.Delay(prms[0].NumValue * 1000);
					break;
				case Cmds.Wait:
					result = await CmdWait(prms);
					break;
				case Cmds.Send:
					await SendLocalAndText(prms[0].StrValue);
					break;
				case Cmds.SendFile:
					result = await CmdSendFile(prms[0].StrValue);
					break;
				case Cmds.Dial:
					result = await CmdDial(prms);
					break;
				case Cmds.Disconnect:
					Disconnect?.Invoke();
					await Task.Delay(3000);
					break;
			}
			//SendDebugText("{1}", TextDebugForm.Modes.Command);
			Debug.WriteLine("wait start");
			//await _bufferManager.WaitSendBufferEmpty();
			Debug.WriteLine("wait stop");
			//SendDebugText("{2}", TextDebugForm.Modes.Command);

			if (!result)
			{
				_stopScript = true;
			}
		}

		private CmdParameter[] ParseCmdParams(string cmdStr, CmdParamTypes[] prmTypes, out bool error)
		{
			int prmCount = prmTypes == null ? 0 : prmTypes.Length;
			error = true;
			int pos = cmdStr.IndexOf('(');
			if (pos == -1)
			{
				// no params in command, error if params required
				error = prmCount != 0;
				return null;
			}

			cmdStr = cmdStr.Substring(pos + 1);
			if (!cmdStr.EndsWith(")")) return null; // error, no closing ')'

			cmdStr = cmdStr.TrimEnd(')'); // remove closing ')'

			List<string> prms = SplitCmdParams(cmdStr);
			if (prms.Count != prmCount) return null;

			// retrieve all parameters
			List<CmdParameter> cmdPrms = new List<CmdParameter>();
			for (int i=0; i<prms.Count; i++)
			{
				prms[i] = prms[i].Trim();
				CmdParameter prm = new CmdParameter(prmTypes[i], prms[i].Trim());
				if (prm.Error)
				{
					// invalid parameter
					error = true;
					return null;
				}
				cmdPrms.Add(prm);
			}

			error = false;
			return cmdPrms.ToArray();
		}

		private List<string> SplitCmdParams(string prmStr)
		{
			List<string> prms = new List<string>();
			bool quote = false;
			bool esc = false;
			string prm = "";
			for (int i=0; i<prmStr.Length; i++)
			{
				char chr = prmStr[i];
				if (esc)
				{
					if (chr == 'x' && i < prmStr.Length - 2)
					{
						byte? code = ParseEscCode(prmStr[i + 1], prmStr[i + 2]);
						if (code.HasValue)
						{
							prm += (char)(code + 128);
							i += 2;
						}
					}
					else
					{
						prm += ParseEscChar(chr);
					}
					esc = false;
					continue;
				}
				if (quote)
				{
					if (chr == '\\')
					{
						esc = true;
						continue;
					}
					prm += chr;
					if (chr=='\'')
					{
						quote = false;
					}
					continue;
				}
				if (chr=='\'')
				{
					prm += chr;
					quote = true;
					continue;
				}
				if (chr==',')
				{
					prms.Add(prm.Trim());
					prm = "";
					continue;
				}
				if (chr==')')
				{
					prms.Add(prm.Trim());
					break;
				}
				prm += chr;
			}
			if (!string.IsNullOrWhiteSpace(prm)) prms.Add(prm.Trim());
			return prms;
		}

		private char ParseEscChar(char chr)
		{
			switch (chr)
			{
				case 'b':
					return CodeManager.ASC_BEL;
				case 'r':
					return CodeManager.ASC_CR;
				case 'n':
					return CodeManager.ASC_LF;
				case 'w':
					return CodeManager.ASC_WRU;
				case 'a':
					return CodeManager.ASC_LTRS;
				case '1':
					return CodeManager.ASC_FIGS;
				case '0':
					return CodeManager.ASC_NUL;
				default:
					return chr;
			}

		}

		private byte? ParseEscCode(char chr1, char chr2)
		{
			string str = new string(new char[] { chr1, chr2 });
			if (int.TryParse(str, out int code))
			{
				return code < 32 ? (byte?)code : null;
			}
			return null;
		}

		private async Task SendAnswerBack()
		{
			string answerBack = _configManager.Config.AnswerbackWinTlx;
			answerBack = answerBack.Replace(@"\r", "\r");
			answerBack = answerBack.Replace(@"\n", "\n");
			await SendLocalAndText(answerBack);
		}

		/// <summary>
		/// wait(text,timeout)
		/// </summary>
		/// <param name="prms"></param>
		/// <returns></returns>
		private async Task<bool> CmdWait(CmdParameter[] prms)
		{
			_recvString = "";
			string waitStr = prms[0].StrValue;
			int timeout = prms[1].NumValue;
			//Debug.WriteLine($"start wait {waitStr},{timeout}");
			bool result = await Task.Run<bool>(async () =>
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();
				while (true)
				{
					await Task.Delay(100);
					if (_recvString.Contains(waitStr))
					{
						//Debug.WriteLine($"found wait 1 {waitStr}");
						await _bufferManager.WaitLocalOutpuBufferEmpty();
						//Debug.WriteLine($"found wait 2 {waitStr}");
						return true;
					}
					if (sw.ElapsedMilliseconds > timeout * 1000) return false;
				}
			});
			_recvString = null;
			return result;
		}

		private async Task<bool> CmdSendFile(string filename)
		{
			CodeSets codeSet = ConfigManager.Instance.Config.CodeSet;

			string[] lines;
			try
			{
				//string text = ConvertText(File.ReadAllText(filename));
				lines = File.ReadAllLines(filename);
			}
			catch(Exception)
			{
				return false;
			}

			foreach (string line in lines)
			{
				List<string> wrappedLines = WrapLine(line, LineWidth);
				foreach (string wrappedLine in wrappedLines)
				{
					if (_stopScript) return false;
					string convLine = ConvertText(wrappedLine, codeSet).Trim();
					await SendTextLine(convLine + "\r\n");
					Debug.WriteLine($"SendBufferCount={_bufferManager.SendBufferCount}");
				}
			}
			return true;
		}

		private async Task<bool> CmdDial(CmdParameter[] prms)
		{
			bool result = await Task.Run<bool>(async () =>
			{
				int itelexNumber = prms[0].NumValue;
				int waitSec = prms[1].NumValue;
				if (waitSec == -1) waitSec = 4;
				Dial?.Invoke(itelexNumber.ToString());
				Stopwatch sw = new Stopwatch();
				sw.Start();
				while (true)
				{
					await Task.Delay(100);
					if (sw.ElapsedMilliseconds > 10 * 1000) return false;
					if (_itelex.ConnectionState == ItelexProtocol.ConnectionStates.Connected)
					{
						await Task.Delay(waitSec * 1000);
						return true;
					}
				}
			});
			return result;
		}

		private async Task SendWru()
		{
			//SendDebugText("{WRU}", DebugForm.Modes.Output);

			await _bufferManager.WaitSendBufferEmpty();
			await SendLocalAndText(CodeManager.ASC_FIGS.ToString());
			await SendLocalAndText(CodeManager.ASC_WRU.ToString());
			await Task.Delay(10000);
		}

		private async Task SendLocalAndText(string asciiText)
		{
			//SendDebugText(asciiText, DebugForm.Modes.Output);

			await Task.Run(() =>
			{
				_bufferManager.SendBufferEnqueueString(asciiText);
			});
		}

		private async Task SendLocalMsg(string asciiText)
		{
			await Task.Run(async () =>
			{
				foreach (char chr in asciiText)
				{
					await _bufferManager.WaitLocalOutpuBufferEmpty();
					_bufferManager.LocalOutputMsg(chr.ToString());
				}
			});
		}

		private void Itelex_Received(string asciiText)
		{
			//SendDebugText(asciiText, DebugForm.Modes.Input);

			if (_recvString != null) _recvString += asciiText;
		}

		private void Itelex_Dropped()
		{
			_stopScript = true;
		}
	}

	class CmdItem
	{
		public Cmds Type { get; set; }

		public string Name { get; set; }

		public CmdParamTypes[] ParamList { get; set; }

		public CmdItem(Cmds type, string name, CmdParamTypes paramType)
		{
			Type = type;
			Name = name;
			ParamList = new CmdParamTypes[] { paramType };
		}

		public CmdItem(Cmds type, string name, CmdParamTypes[] paramList = null)
		{
			Type = type;
			Name = name;
			ParamList = paramList;
		}

		public override string ToString()
		{
			return $"{Name} {Type} {ParamList}";
		}
	}

	class CmdParameter
	{
		public object Value { get; private set; }

		public int NumValue
		{
			get
			{
				return Value != null ? (int)Value : 0;
			}
		}

		public string StrValue
		{
			get
			{
				return Value != null ? (string)Value : "";
			}
		}

		public CmdParamTypes Type { get; private set; }

		public bool Error { get; private set; }

		public CmdParameter(CmdParamTypes type, string valStr)
		{
			Type = type;
			Error = false;
			switch(type)
			{
				case CmdParamTypes.Str:
					if (valStr.StartsWith("'") && valStr.EndsWith("'"))
					{
						Value = valStr.Substring(1, valStr.Length - 2);
					}
					else
					{
						Value = valStr;
					}
					break;
				case CmdParamTypes.Num:
					if (int.TryParse(valStr, out int num))
					{
						Value = num;
					}
					else
					{
						Error = true;
					}
					break;
				default:
					Error = true;
					break;
			}
		}

		public override string ToString()
		{
			return $"{Type} {StrValue} {NumValue} {Error}";
		}
	}
}
