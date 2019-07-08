using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTlx.Languages;

namespace WinTlx
{
	public partial class SendFileForm : Form
	{
		private const string TAG = "SendFileForm";

		private enum Cropping { Right, Center, Left };

		private string[] _textLines;

		private Cropping _croppingMode;

		public string AsciiText { get; set; }

		public SendFileForm()
		{
			InitializeComponent();

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageChanged();

			string line = "";
			for (int i = 0; i < 68; i++)
			{
				line += (char)(48 + (i+1) % 10);
			}
			TextTb.Text = line;

			LineLengthTb.Text = MainForm.SCREEN_WIDTH.ToString();

			CropRightRb.CheckedChanged += Crop_Changed;
			CropRightRb.Tag = Cropping.Right;
			CropLeftRb.CheckedChanged += Crop_Changed;
			CropLeftRb.Tag = Cropping.Left;
			CropCenterRb.CheckedChanged += Crop_Changed;
			CropCenterRb.Tag = Cropping.Center;
			CropRightRb.Checked = true;

			CropRightRb.Checked = true;
			_textLines = File.ReadAllLines("test.txt");
			ShowCroppedText();
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			this.Text = $"{Constants.PROGRAM_NAME} {LngText(LngKeys.SendFile_SendFile)}";
			LoadBtn.Text = LngText(LngKeys.SendFile_LoadFile);
			LineLengthLbl.Text = LngText(LngKeys.SendFile_LineLength);
			CroppingGb.Text = LngText(LngKeys.SendFile_Cropping);
			CropRightRb.Text = LngText(LngKeys.SendFile_CroppingRight);
			CropCenterRb.Text = LngText(LngKeys.SendFile_CroppingCenter);
			CropLeftRb.Text = LngText(LngKeys.SendFile_CroppingLeft);
			ConvertCb.Text = LngText(LngKeys.SendFile_Convert);
			SendBtn.Text = LngText(LngKeys.SendFile_SendButton);
			CancelBtn.Text = LngText(LngKeys.SendFile_CancelButton);
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		private void Crop_Changed(object sender, EventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			_croppingMode = (Cropping)rb.Tag;
			ShowCroppedText();
		}

		private void ConvertCb_CheckedChanged(object sender, EventArgs e)
		{
			ShowCroppedText();
		}

		private void LineLengthTb_Leave(object sender, EventArgs e)
		{
			ShowCroppedText();
		}

		private void LoadBtn_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = "..";
			openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;

			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			try
			{
				string fullName = openFileDialog.FileName;
				_textLines = File.ReadAllLines(fullName);
				ShowCroppedText();
				/*
				foreach (string line in lines)
				{
					// convert to replacements for real length
					string sendLine = CodeConversion.AsciiStringToTelex(line);
					if (sendLine.Length > 68)
						sendLine = sendLine.Substring(0, 68);
					//SendAsciiText(sendLine + "\r\n");
				}
				*/
			}
			catch (Exception)
			{
			}
		}

		private void SendBtn_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(TextTb.Text))
			{
				AsciiText = ConvertAscii(TextTb.Text);
			}
			else
			{
				AsciiText = null;
			}
			Close();
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			AsciiText = null;
			Close();
		}

		private void ShowCroppedText()
		{
			if (_textLines==null || _textLines.Length==0)
			{
				TextTb.Text = "";
				return;
			}

			int maxLineLength = 0;
			int lineCount = 0;
			for (int i=0; i<_textLines.Length; i++)
			{
				if (!string.IsNullOrWhiteSpace(_textLines[i]))
				{
					lineCount = i + 1;
					if (_textLines[i].TrimEnd().Length>maxLineLength)
					{
						maxLineLength = _textLines[i].Trim().Length;
					}
				}
			}

			string text = "";
			foreach (string line in _textLines)
			{
				text += line + "\r\n";
			}

			uint lineLength;
			if (!uint.TryParse(LineLengthTb.Text, out lineLength))
			{
				TextTb.Text = text;
				return;
			}

			int left = 0;
			int right = 0;
			switch (_croppingMode)
			{
				case Cropping.Right:
					left = 0;
					if (maxLineLength <= lineLength)
					{
						right = maxLineLength - 1;
					}
					else
					{
						right = (int)lineLength - 1;
					}
					break;
				case Cropping.Left:
					right = maxLineLength - 1;
					if (maxLineLength <= lineLength)
					{
						left = 0;
					}
					else
					{
						left = maxLineLength - (int)lineLength;
					}
					break;
				case Cropping.Center:
					if (maxLineLength <= lineLength)
					{
						left = 0;
						right = maxLineLength - 1;
					}
					else
					{
						left = (maxLineLength - (int)lineLength) / 2;
						right = (int)lineLength;
					}
					break;
			}


			// crop lines
			text = "";
			for (int i=0; i< lineCount; i++)
			{
				string line = _textLines[i].TrimEnd();
				line = CropLine(line, left, right);
				text += line + "\r\n";
			}

			if (ConvertCb.Checked)
			{
				text = ConvertAscii(text);
			}

			TextTb.Text = text;
		}

		private string CropLine(string str, int left, int right)
		{
			if (str.Length<left)
			{
				return "";
			}

			int len = right - left + 1;
			if (left+len>str.Length)
			{
				len = str.Length - left;
			}
			return str.Substring(left, len);
		}

		private string ConvertAscii(string str)
		{
			string convStr = "";
			for (int i = 0; i < str.Length; i++)
			{
				char chr = str[i];
				if (chr>='a' && chr<='z' || chr>='0' && chr<='9' || chr==' ')
				{
					convStr += chr;
					continue;
				}
				if (chr>='A' && chr<='Z')
				{
					convStr += "8";
					continue;
				}
				if (chr>=128)
				{
					convStr += ".";
					continue;
				}
				if (chr=='\r' || chr=='\n')
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
			return convStr;
		}

		private void RulerPnl_Paint(object sender, PaintEventArgs e)
		{
			Helper.PaintRuler(e.Graphics, MainForm.SCREEN_WIDTH, 7F);
		}

	}
}
