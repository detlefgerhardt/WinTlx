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
		public event ClosedEventHandler ClosedEvt;

		public delegate void ClickEventHandler();
		public event ClickEventHandler ClickEvt;

		private Rectangle _parentWindowsPosition;

		private readonly ItelexProtocol _itelex;

		private readonly TapePunchManager _tapePunch;

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

			//CropStartBtn.Enabled = false;
			//CropEndBtn.Enabled = false;

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
			Helper.SetToolTip(RecvCb, LngText(LngKeys.TapePunch_RecvButton_ToolTip));
			SendBtn.Text = LngText(LngKeys.TapePunch_SendButton);
			Helper.SetToolTip(SendBtn, LngText(LngKeys.TapePunch_SendButton_ToolTip));
			ClearBtn.Text = LngText(LngKeys.TapePunch_ClearButton);
			Helper.SetToolTip(ClearBtn, LngText(LngKeys.TapePunch_ClearButton_ToolTip));
			LoadBtn.Text = LngText(LngKeys.TapePunch_LoadButton);
			Helper.SetToolTip(LoadBtn, LngText(LngKeys.TapePunch_LoadButton_ToolTip));
			SaveBtn.Text = LngText(LngKeys.TapePunch_SaveButton);
			Helper.SetToolTip(SaveBtn, LngText(LngKeys.TapePunch_SaveButton_ToolTip));
			CloseBtn.Text = LngText(LngKeys.TapePunch_CloseButton);
			EditCb.Text = LngText(LngKeys.TapePunch_EditButton);
			Helper.SetToolTip(EditCb, LngText(LngKeys.TapePunch_EditButton_ToolTip));
			EditInsertCb.Text = LngText(LngKeys.TapePunch_InsertButton);
			Helper.SetToolTip(EditInsertCb, LngText(LngKeys.TapePunch_InsertButton_ToolTip));
			EditDeleteBtn.Text = LngText(LngKeys.TapePunch_DeleteButton);
			Helper.SetToolTip(EditDeleteBtn, LngText(LngKeys.TapePunch_DeleteButton_ToolTip));

			Helper.SetToolTip(CropStartBtn, LngText(LngKeys.TapePunch_CropStartButton_ToolTip));
			Helper.SetToolTip(CropEndBtn, LngText(LngKeys.TapePunch_CropEndButton_ToolTip));
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
			bool recvStat = _tapePunch.PuncherOn;
			SetRecv(false);
			await Task.Run(() =>
			{
				byte[] buffer = _tapePunch.GetBufferFromCurrentPos();
				for (int i = 0; i < buffer.Length; i++)
				{
					while(_itelex.GetSendBufferCount()>2)
					{
						Task.Delay(100);
					}
					_itelex.SendBaudotCode(buffer[i]);
					_tapePunch.ScrollLeft(1);
					//PunchedTapePb.Refresh();
					//UpdateScrollbar();
				}
			});
			SendBtnActive(false);
			SetRecv(recvStat);
		}

		private void LoadBtn_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				//openFileDialog.InitialDirectory = "c:\\";
				Filter = "ls files (*.ls)|*.ls|bin files (*.bin)|*.bin|All files (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true
			};

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
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "ls files (*.ls)|*.ls|bin files (*.bin)|*.bin|All files (*.*)|*.*",
				FilterIndex = 1,
				RestoreDirectory = true
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				byte[] buffer = _tapePunch.GetBuffer();
				File.WriteAllBytes(saveFileDialog.FileName, buffer);
			}
		}

		private void CropStartBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.CropStart();
		}

		private void CropEndBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.CropEnd();
		}

		private void EditUndoBtn_Click(object sender, EventArgs e)
		{
			_tapePunch.PullUndo();
		}

		private void PunchedTapePb_Click(object sender, EventArgs e)
		{
			ClickEvt?.Invoke();
		}

		private void TapePunchHorizontalForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			ClosedEvt?.Invoke();
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
