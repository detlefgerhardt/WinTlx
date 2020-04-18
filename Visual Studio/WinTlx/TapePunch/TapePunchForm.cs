using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTlx.Languages;

namespace WinTlx.TapePunch
{
	public partial class TapePunchForm : Form
	{
		private const string TAG = nameof(TapePunchForm);

		public delegate void ClosedEventHandler();
		public event ClosedEventHandler Closed;

		private Rectangle _parentWindowsPosition;

		private ItelexProtocol _itelex;

		private TapePunchManager _tapePunch;

		public TapePunchForm(Rectangle position)
		{
			InitializeComponent();

			_parentWindowsPosition = position;

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageChanged();

			_itelex = ItelexProtocol.Instance;

			_tapePunch = TapePunchManager.Instance;
			_tapePunch.Punched += TapePunch_Punched;
			_tapePunch.Changed += TapePunch_Changed;
			_tapePunch.SetPuncherLinesHorizontal(PunchedTapePb.Width);
			RecvCbSet(_tapePunch.PuncherOn);

			EditCb.Enabled = true;
			_tapePunch.EditOn = false;
			EditCbSet(false);

			EditStartBtn.Enabled = false;
			EditEndBtn.Enabled = false;
			EditCropBtn.Enabled = false;

			TapePositionSb.SmallChange = 1;
			TapePositionSb.LargeChange = 1;
			TapePositionSb.DelayedScroll += TapePositionSb_DelayedScroll;
		}

		private void TapePositionSb_DelayedScroll()
		{
			_tapePunch.DisplayPos = TapePositionSb.Value;
			Helper.ControlInvokeRequired(TapePositionSb, () => PunchedTapePb.Refresh());
			TapePunch_Changed();
		}

		private void TapePunch_Punched()
		{
			UpdateScrollbar();
		}

		private void TapePunch_Changed()
		{
			Helper.ControlInvokeRequired(PunchedTapePb,
				() => PunchedTapePb.Refresh()
			);
			Helper.ControlInvokeRequired(BufferStatusLbl,
				() => BufferStatusLbl.Text = $"{_tapePunch.DisplayPos + 1} / {_tapePunch.BufferSize}"
			);
			UpdateScrollbar();
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			this.Text = $"{Constants.PROGRAM_NAME} {LngText(LngKeys.TapePunch_TapePunch)}";
			RecvCb.Text = LngText(LngKeys.TapePunch_RecvButton);
			SendBtn.Text = LngText(LngKeys.TapePunch_SendButton);
			ClearBtn.Text = LngText(LngKeys.TapePunch_ClearButton);
			LoadBtn.Text = LngText(LngKeys.TapePunch_LoadButton);
			SaveBtn.Text = LngText(LngKeys.TapePunch_SaveButton);
			CloseBtn.Text = LngText(LngKeys.TapePunch_CloseButton);
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		private void PunchTapeForm_Load(object sender, EventArgs e)
		{
			SetPosition(_parentWindowsPosition);
		}

		private void PunchTapeForm_ResizeEnd(object sender, EventArgs e)
		{
			_tapePunch.SetPuncherLinesHorizontal(PunchedTapePb.Width);
			PunchedTapePb.Refresh();
			UpdateScrollbar();
		}

		private void UpdateScrollbar()
		{
			Helper.ControlInvokeRequired(TapePositionSb, () =>
			{
				//TapePositionSb.Minimum = _tapePunch.BufferSize == 0 ? 0 : 1;
				TapePositionSb.Minimum = 0;
				TapePositionSb.Maximum = _tapePunch.BufferSize;
				TapePositionSb.Value = _tapePunch.DisplayPos;
			});
		}

		private void CloseBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		public void SetPosition(Rectangle position)
		{
			int x = position.X + position.Width;
			int y = position.Y;
			SetBounds(x, y, Bounds.Width, Bounds.Height);
		}

		/// <summary>
		/// Update puncher graphic
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PunchedTapePb_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			_tapePunch.DrawTapeHorizontal(g, PunchedTapePb.Width, PunchedTapePb.Height);
		}

		private void ClearBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.Clear();
		}

		private void RecvCb_Click(object sender, EventArgs e)
		{
			SetRecv(RecvCb.Checked);
		}

		private void SetRecv(bool on)
		{
			_tapePunch.PuncherOn = on;
			RecvCbSet(on);
			if (on)
			{
				_tapePunch.EditOn = false;
				EditCbSet(false);
			}
		}

		private void EditCb_Click(object sender, EventArgs e)
		{
			SetEdit(EditCb.Checked);
		}

		private void SetEdit(bool on)
		{
			_tapePunch.EditOn = on;
			EditCbSet(on);
			if (on)
			{
				_tapePunch.EditInsert = true;
				EditInsertCb.Checked = true;
				//_tapePunch.PuncherOn = false;
				//RecvCbSet(false);
			}
		}

		private void EditInsertCb_Click(object sender, EventArgs e)
		{
			_tapePunch.EditInsert = EditInsertCb.Checked;
		}

		private void EditDeleteBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.DeleteCode();
		}

		private async void SendBtn_Click(object sender, EventArgs e)
		{
			SendBtnActive(true);
			SetRecv(false);
			await Task.Run(() =>
			{
				byte[] buffer = _tapePunch.GetBuffer();
				for (int i = 0; i < buffer.Length; i++)
				{
					_itelex.SendBaudotCode(buffer[i]);
				}
			});
			SendBtnActive(false);
		}

		private void LoadBtn_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			//openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "ls files (*.ls)|*.ls|bin files (*.bin)|*.bin|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				SetRecv(false);
				string filePath = openFileDialog.FileName;
				byte[] buffer = File.ReadAllBytes(filePath);
				_tapePunch.SetBuffer(buffer);
				PunchedTapePb.Refresh();
				UpdateScrollbar();
			}
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();

			saveFileDialog.Filter = "ls files (*.ls)|*.ls|bin files (*.bin)|*.bin|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 1;
			saveFileDialog.RestoreDirectory = true;

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				byte[] buffer = _tapePunch.GetBuffer();
				File.WriteAllBytes(saveFileDialog.FileName, buffer);
			}
		}

		private void TapePunchHorizontalForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Closed?.Invoke();
		}

		private void ScrollLeftBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.ScrollRight(_tapePunch.VisiblePunchLines-1);
		}

		private void ScrollRightBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.ScrollLeft(_tapePunch.VisiblePunchLines - 1);
		}

		private void ScrollFirstBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.DisplayPos = _tapePunch.SetMinPosition(0);
			PunchedTapePb.Refresh();
			UpdateScrollbar();
		}

		private void ScrollLastBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.DisplayPos = _tapePunch.BufferSize;
			PunchedTapePb.Refresh();
			UpdateScrollbar();
		}

		private void RecvCbSet(bool on)
		{
			RecvCb.Checked = on;
			if (on)
			{
				RecvCb.ForeColor = Color.Green;
				RecvCb.Font = new Font("Arial", 8.25F, FontStyle.Bold);
			}
			else
			{
				RecvCb.ForeColor = Color.Black;
				RecvCb.Font = new Font("Arial", 8.25F, FontStyle.Regular);
			}
		}

		private void EditCbSet(bool on)
		{
			EditCb.Checked = on;
			if (on)
			{
				EditCb.ForeColor = Color.Green;
				EditCb.Font = new Font("Arial", 8.25F, FontStyle.Bold);
			}
			else
			{
				EditCb.ForeColor = Color.Black;
				EditCb.Font = new Font("Arial", 8.25F, FontStyle.Regular);
			}
			EditPl.Visible = on;
			PunchedTapePb.Refresh();
		}

		private void SendBtnActive(bool active)
		{
			SendBtn.Enabled = !active;
			if (active)
			{
				SendBtn.ForeColor = Color.Green;
				SendBtn.Font = new Font("Arial", 8.25F, FontStyle.Bold);
			}
			else
			{
				SendBtn.ForeColor = Color.Black;
				SendBtn.Font = new Font("Arial", 8.25F, FontStyle.Regular);
			}
		}

	}
}
