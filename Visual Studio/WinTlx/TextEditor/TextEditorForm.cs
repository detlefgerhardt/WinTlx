using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Languages;

namespace WinTlx.TextEditor
{
	public partial class TextEditorForm : Form
	{
		private const string TAG = nameof(TextEditorForm);

		private readonly TextEditorManager _tem;

		public delegate void CloseEventHandler();
		public event CloseEventHandler CloseEditor;

		private Rectangle _parentWindowsPosition;

		public TextEditorForm(Rectangle position)
		{
			_parentWindowsPosition = position;

			InitializeComponent();
			this.KeyPreview = true;

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageManager.Instance.ChangeLanguage(ConfigManager.Instance.Config.Language);

			_tem = TextEditorManager.Instance;
			_tem.SavedStatusChanged += Tem_SavedStatusChanged;

			EditorRtb.PasteEvent += EditorRtb_Paste;
			EditorRtb.Text = _tem.Text;

			SetCharWidth();

			//InitLoad(@"c:\Itelex\script5.txt");

			ShowLineAndColumn();

			SetTitle();
		}

		private void TextEditorForm_Load(object sender, EventArgs e)
		{
			Point pos = Helper.CenterForm(this, _parentWindowsPosition);
			SetBounds(pos.X, pos.Y, Bounds.Width, Bounds.Height);
		}

		private void TextEditorForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			_tem.Text = EditorRtb.Text;
			CloseEditor?.Invoke();
		}

		private void SetTitle()
		{
			if (string.IsNullOrEmpty(_tem.Filename))
			{
				this.Text = LngText(LngKeys.Editor_Header);
			}
			else
			{
				this.Text = $"{LngText(LngKeys.Editor_Header)} {_tem.Filename}";
			}
		}

		private void InitLoad(string filename)
		{
			_tem.LoadFile(filename);
			EditorRtb.Text = _tem.Text;
			SetTitle();
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			ShowTitle();
			ClearBtn.Text = LngText(LngKeys.Editor_Clear);
			LoadBtn.Text = LngText(LngKeys.Editor_Load);
			SaveBtn.Text = LngText(LngKeys.Editor_Save);
			SendBtn.Text = LngText(LngKeys.Editor_Send);
			CloseBtn.Text = LngText(LngKeys.Editor_Close);
			AlignBlockBtn.Text = LngText(LngKeys.Editor_AlignBlock);
			AlignLeftBtn.Text = LngText(LngKeys.Editor_AlignLeft);
			ShiftLeftBtn.Text = LngText(LngKeys.Editor_ShiftLeft);
			ShiftRightBtn.Text = LngText(LngKeys.Editor_ShiftRight);
			CharWidthLbl.Text = LngText(LngKeys.Editor_CharWidth);
			ShowLineAndColumn();
		}

		private void ShowTitle()
		{
			if (_tem == null) return;

			string title;
			if (string.IsNullOrEmpty(_tem?.Filename))
			{
				title = $"{LngText(LngKeys.Editor_Header)} unbenannt";
			}
			else
			{
				title = $"{LngText(LngKeys.Editor_Header)} {_tem.Filename}";
			}
			if (!_tem.Saved) title += " *";
			this.Text = title;
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		private void TextEditorForm_KeyPress(object sender, KeyPressEventArgs e)
		{
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch(keyData)
			{
				case Keys.Z | Keys.Control:
					_tem.Undo(EditorRtb.Text);
					EditorRtb.Text = _tem.Text;
					return true;
				case Keys.Y | Keys.Control:
					_tem.Redo();
					EditorRtb.Text = _tem.Text;
					return true;
				case Keys.S | Keys.Control:
					_tem.SaveFile(_tem.Filename);
					return true;
			}
			return false;
		}

		private void Tem_SavedStatusChanged()
		{
			ShowTitle();
		}

		private void ClearBtn_Click(object sender, EventArgs e)
		{
			_tem.Text = "";
			EditorRtb.Text = _tem.Text;
		}

		private void LoadBtn_Click(object sender, EventArgs e)
		{
			_tem.LoadFile();
			EditorRtb.Text = _tem.Text;
			SetTitle();
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			_tem.SaveFile();
			SetTitle();
		}

		private async void SendBtn_Click(object sender, EventArgs e)
		{
			await _tem.SendTextAsync(EditorRtb.Lines);
		}

		private void CloseBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void UndoBtn_Click(object sender, EventArgs e)
		{
			_tem.Undo(EditorRtb.Text);
			EditorRtb.Text = _tem.Text;
		}

		private void RedoBtn_Click(object sender, EventArgs e)
		{
			_tem.Redo();
			EditorRtb.Text = _tem.Text;
		}

		private void EditorRtb_KeyPress(object sender, KeyPressEventArgs e)
		{
			string chrStr = _tem.ConvertTextChar(e.KeyChar, ConfigManager.Instance.Config.CodeSet);

			if (chrStr.Length == 1)
			{
				e.KeyChar = chrStr[0];
			}
			else
			{
				SendKeys.Send(chrStr);
				e.Handled = true;
			}
		}

		private void EditorRtb_Paste(object sender, EventArgs e)
		{
			//Debug.WriteLine(EditorRtb.Text);
			EditorRtb.Text = _tem.ConvertText(EditorRtb.Text, ConfigManager.Instance.Config.CodeSet);
			//Debug.WriteLine(EditorRtb.Text);
		}

		private void EditorRtb_TextChanged(object sender, EventArgs e)
		{
			_tem.Text = EditorRtb.Text;
			ShowLineAndColumn();
		}

		private void EditorRtb_CursorPositionChanged(object sender, EventArgs e)
		{
			ShowLineAndColumn();
		}

		private void EditorRtb_SelectionChanged(object sender, EventArgs e)
		{
			ShowLineAndColumn();
		}

		private void CharWidthTb_Leave(object sender, EventArgs e)
		{
			if (int.TryParse(CharWidthTb.Text, out int width))
			{
				_tem.LineWidth = width;
				SetCharWidth();
			}
			else
			{
				CharWidthTb.Text = "";
			}
		}

		private void ConvertToBaudotBtn_Click(object sender, EventArgs e)
		{
			_tem.ConvertToBaudot();
			EditorRtb.Text = _tem.Text;
		}

		private void ConvertToRttyArtBtn_Click(object sender, EventArgs e)
		{
			_tem.ConvertToRtty();
			EditorRtb.Text = _tem.Text;
		}

		private void ShowLineAndColumn()
		{
			int line = EditorRtb.CurrentLine;
			if (line == 0) line++;
			LineNrLbl.Text = $"{LngText(LngKeys.Editor_LineNr)}:{line}";
			ColumnNrLbl.Text = $"{LngText(LngKeys.Editor_ColumnNr)}:{EditorRtb.CurrentColumn}";
		}

		private void LinealPnl_Paint(object sender, PaintEventArgs e)
		{
			Helper.PaintRuler(e.Graphics, _tem.LineWidth, 8.98F);
		}

		private void AlignBlockBtn_Click(object sender, EventArgs e)
		{
			_tem.AlignBlock(EditorRtb.Lines);
			EditorRtb.Text = _tem.Text;
		}

		private void AlignLeftBtn_Click(object sender, EventArgs e)
		{
			_tem.AlignLeft(EditorRtb.Lines);
			EditorRtb.Text = _tem.Text;
		}

		private void ShiftLeftBtn_Click(object sender, EventArgs e)
		{
			EditorRtb.Lines = _tem.ShiftLeft(EditorRtb.Lines);
			_tem.Text = EditorRtb.Text;
		}

		private void ShiftRightBtn_Click(object sender, EventArgs e)
		{
			EditorRtb.Lines = _tem.ShiftRight(EditorRtb.Lines);
		}

		private void SetCharWidth()
		{
			LinealPnl.Refresh();
			EditorRtb.RightMargin = (int)(3 + 8.98F * _tem.LineWidth);
		}

		private void ShowFilename()
		{
		}

	}
}
