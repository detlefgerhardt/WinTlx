using System;
using System.Linq;
using System.Windows.Forms;
using WinTlx.Config;
using WinTlx.Languages;

namespace WinTlx.TextEditor
{
	public partial class TextEditorForm : Form
	{
		private const string TAG = nameof(TextEditorForm);

		private TextEditorManager _tem;

		public delegate void CloseEventHandler();
		public event CloseEventHandler CloseEditor;

		public TextEditorForm()
		{
			InitializeComponent();
			this.KeyPreview = true;

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageManager.Instance.ChangeLanguage(ConfigManager.Instance.Config.Language);

			_tem = TextEditorManager.Instance;
			EditorRtb.Text = _tem.Text;
			SetCharWidth();
			ShowLineAndColumn();
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			this.Text = LngText(LngKeys.Editor_Header);
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

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
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
			}
			return false;
		}

		private void TextEditorForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			EditorRtb.Dispose();
			CloseEditor?.Invoke();
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
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			_tem.SaveFile();
		}

		private void SendBtn_Click(object sender, EventArgs e)
		{
			_tem.SendText(EditorRtb.Lines);
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
			string keyChars = char.ToLower(e.KeyChar).ToString();

			// convert chars
			switch (keyChars)
			{
				case "à":
				case "á":
				case "â":
					keyChars = "e";
					break;
				case "è":
				case "é":
				case "ê":
					keyChars = "e";
					break;
				case "ì":
				case "í":
				case "î":
					keyChars = "i";
					break;
				case "ä":
					keyChars = "ae";
					break;
				case "ö":
					keyChars = "oe";
					break;
				case "ü":
					keyChars = "ue";
					break;
				case "ß":
					keyChars = "ss";
					break;
				case "´":
				case "`":
					keyChars = "'";
					break;
				case "\"":
					keyChars = "''";
					break;
				case "*":
					keyChars = "x";
					break;
			}

			// skip invalid chars
			string keyChars2 = "";
			for (int i=0; i<keyChars.Length; i++)
			{
				if (TextEditorManager.ALLOWED_CHARS.Contains(keyChars[i]))
				{
					keyChars2 += keyChars[i];
				}
			}

			if (keyChars2.Length==1)
			{
				e.KeyChar = keyChars[0];
			}
			else
			{
				SendKeys.Send(keyChars2);
				e.Handled = true;
			}
		}

		private void EditorRtb_TextChanged(object sender, EventArgs e)
		{
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

		private void EditorRtb_Leave(object sender, EventArgs e)
		{
			_tem.Text = EditorRtb.Text;
		}

		private void CharWidthTb_Leave(object sender, EventArgs e)
		{
			if (int.TryParse(CharWidthTb.Text, out int width))
			{
				_tem.CharWidth = width;
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
			Helper.PaintRuler(e.Graphics, _tem.CharWidth, 8.98F);
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
			EditorRtb.RightMargin = (int)(3 + 8.98F * _tem.CharWidth);
		}

	}
}
