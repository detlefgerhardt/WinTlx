﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinTlx.Languages;

namespace WinTlx.Scheduler
{
	public partial class SchedulerForm : Form
	{
		private const string TAG = nameof(SchedulerForm);

		public delegate void ClosedEventHandler();
		public event ClosedEventHandler ClosedEvent;

		private const int SCHEDULE_COL_ACTIVE = 0;
		private const int SCHEDULE_COL_SUCCESS = 1;
		private const int SCHEDULE_COL_ERROR = 2;
		private const int SCHEDULE_COL_DATE = 3;
		private const int SCHEDULE_COL_TIME = 4;
		private const int SCHEDULE_COL_DEST = 5;
		private const int SCHEDULE_COL_FILE = 6;
		private const int SCHEDULE_COL_COUNT = 7;

		private readonly SchedulerManager _manager;

		private DataGridViewCellEventArgs _contextMenuLocation;

		private Rectangle _parentWindowsPosition;

		public SchedulerForm(Rectangle position)
		{
			_parentWindowsPosition = position;

			InitializeComponent();

			//LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageChanged();

			SchedulerView.BackgroundColor = Color.White;
			SchedulerView.RowHeadersVisible = false;
			SchedulerView.ScrollBars = ScrollBars.Both;
			//SchedulerView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			SchedulerView.AllowUserToAddRows = false;
			SchedulerView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			SetSchedulerColumns();
			SetContextMenu();

			_manager = SchedulerManager.Instance;
			_manager.Changed += SchedulerManager_Changed;
			//_manager.LoadScheduler();

			ShowSchedules();
		}

		private void SchedulerForm_Load(object sender, EventArgs e)
		{
			Point pos = Helper.CenterForm(this, _parentWindowsPosition);
			SetBounds(pos.X, pos.Y, Bounds.Width, Bounds.Height);
		}

		private void SetSchedulerColumns()
		{
			var chkCol = new DataGridViewCheckBoxColumn();
			chkCol.Name = "Active";
			chkCol.HeaderText = $"{LngText(LngKeys.Scheduler_ActiveRow)}";
			chkCol.Width = 55;
			SchedulerView.Columns.Add(chkCol);

			chkCol = new DataGridViewCheckBoxColumn();
			chkCol.Name = "Success";
			chkCol.HeaderText = $"{LngText(LngKeys.Scheduler_SuccessRow)}";
			chkCol.Width = 55;
			SchedulerView.Columns.Add(chkCol);

			chkCol = new DataGridViewCheckBoxColumn();
			chkCol.Name = "Error";
			chkCol.HeaderText = $"{LngText(LngKeys.Scheduler_ErrorRow)}";
			chkCol.Width = 55;
			SchedulerView.Columns.Add(chkCol);

			var calCol = new CalendarColumn();
			calCol.Name = "Date";
			calCol.HeaderText = $"{LngText(LngKeys.Scheduler_DateRow)}";
			calCol.Width = 80;
			SchedulerView.Columns.Add(calCol);

			var timeCol = new TimeColumn();
			timeCol.Name = "Time";
			timeCol.HeaderText = $"{LngText(LngKeys.Scheduler_TimeRow)}";
			timeCol.Width = 70;
			SchedulerView.Columns.Add(timeCol);

			var destCol = new DataGridViewTextBoxColumn();
			destCol.Name = "Destination";
			destCol.HeaderText = $"{LngText(LngKeys.Scheduler_DestRow)}";
			destCol.Width = 200;
			SchedulerView.Columns.Add(destCol);

			var fileCol = new DataGridViewTextBoxColumn();
			fileCol.Name = "File";
			fileCol.HeaderText = $"{LngText(LngKeys.Scheduler_FileRow)}";
			fileCol.Width = 200;
			SchedulerView.Columns.Add(fileCol);
		}

		private void SetContextMenu()
		{
			ContextMenuStrip strip = new ContextMenuStrip();

			ToolStripMenuItem toolStripItem = new ToolStripMenuItem();
			toolStripItem.Text = LngText(LngKeys.Scheduler_DeleteEntry);
			toolStripItem.Enabled = true;
			toolStripItem.Click += ContextMenu_Delete_Handler;
			strip.Items.Add(toolStripItem);

			toolStripItem = new ToolStripMenuItem();
			toolStripItem.Text = LngText(LngKeys.Scheduler_CopyEntry);
			toolStripItem.Enabled = true;
			toolStripItem.Click += ContextMenu_Copy_Handler;
			strip.Items.Add(toolStripItem);

			for (int i = 1; i < SCHEDULE_COL_COUNT; i++)
			{
				SchedulerView.Columns[i].ContextMenuStrip = strip;
				//EpgView.Columns[8].ContextMenuStrip.Items.Add(toolStripItem1);
			}
		}

		private void SchedulerView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
		{
			_contextMenuLocation = e;
		}

		private void ContextMenu_Delete_Handler(object sender, EventArgs e)
		{
			SchedulerItem item = (SchedulerItem)SchedulerView.Rows[_contextMenuLocation.RowIndex].Tag;
			_manager.ScheduleData.SchedulerList.Remove(item);

			ShowSchedules();
		}

		private void ContextMenu_Copy_Handler(object sender, EventArgs e)
		{
			SchedulerItem item = (SchedulerItem)SchedulerView.Rows[_contextMenuLocation.RowIndex].Tag;

			SchedulerItem newItem = new SchedulerItem()
			{
				Active = false,
				Success = false,
				Error = false,
				Timestamp = item.Timestamp,
				Destination = item.Destination,
				Filename = item.Filename
			};
			_manager.ScheduleData.SchedulerList.Add(newItem);

			ShowSchedules();
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			this.Text = $"{LngText(LngKeys.Scheduler_Scheduler)}";
			AddEntryBtn.Text = $"{LngText(LngKeys.Scheduler_AddEntry)}";
			CloseBtn.Text = $"{LngText(LngKeys.Scheduler_CloseButton)}";
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		private void SchedulerManager_Changed()
		{
			ShowSchedules();
		}

		private void InitView()
		{
			//List<DataGridViewRow> rows = new List<DataGridViewRow>();

			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(SchedulerView);
			//rows.Add(row);

			SchedulerView.Rows.Clear();
			//SchedulerView.Rows.AddRange(rows.ToArray());
		}

		private void ShowSchedules()
		{
			_manager.SortScheduler();

			List<SchedulerItem> schedule = _manager.ScheduleData?.SchedulerList;
			if (schedule == null)
			{
				SchedulerView.Rows.Clear();
				return;
			}

			List<DataGridViewRow> rows = new List<DataGridViewRow>();

			foreach (SchedulerItem item in schedule)
			{
				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(SchedulerView);

				row.Cells[SCHEDULE_COL_ACTIVE].Value = item.Active;
				row.Cells[SCHEDULE_COL_SUCCESS].Value = item.Success;
				row.Cells[SCHEDULE_COL_ERROR].Value = item.Error;
				row.Cells[SCHEDULE_COL_DATE].Value = new DateTime(item.Timestamp.Year, item.Timestamp.Month, item.Timestamp.Day);
				row.Cells[SCHEDULE_COL_TIME].Value = item.Timestamp;
				row.Cells[SCHEDULE_COL_DEST].Value = item.Destination;
				row.Cells[SCHEDULE_COL_FILE].Value = Path.GetFileName(item.Filename);
				row.Cells[SCHEDULE_COL_FILE].Tag = item.Filename;


				for (int i = 0; i < SCHEDULE_COL_COUNT; i++)
				{
					Color color = Color.Black;
					if (i==_manager.SchedulerActive)
					{
						color = Color.Green;
					}
					else if (!item.Active || item.Success)
					{
						color = Color.Gray;
					}
					else if (item.Error)
					{
						color = Color.Red;
					}

					DataGridViewCell cell = row.Cells[i];
					DataGridViewCellStyle style = cell.Style;
					//style.Font = new Font(EpgView.Font.FontFamily, 8.25F, FontStyle.Regular);
					style.ForeColor = color;
					style.BackColor = Color.White;
					cell.Style.ApplyStyle(style);
				}

				row.Tag = item;

				rows.Add(row);
			}

			Helper.ControlInvokeRequired(SchedulerView, () =>
				{
					SchedulerView.Rows.Clear();
					SchedulerView.Rows.AddRange(rows.ToArray());
				});
		}

		private void AddScheduleBtn_Click(object sender, EventArgs e)
		{
			SchedulerItem item = new SchedulerItem()
			{
				Active = false,
				Success = false,
				Error = false,
				Timestamp = DateTime.Now
			};
			_manager.ScheduleData.SchedulerList.Add(item);
			ShowSchedules();
		}

		private void SchedulerView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == -1 || e.RowIndex == -1)
			{
				return;
			}

			SchedulerView.CurrentCell = SchedulerView[e.ColumnIndex, e.RowIndex];

			switch (e.ColumnIndex)
			{
				case SCHEDULE_COL_ACTIVE:
					break;
				case SCHEDULE_COL_FILE:
					// select file
					OpenFileDialog dialog = new OpenFileDialog();
					string fullName = (string)SchedulerView[e.ColumnIndex, e.RowIndex].Tag;
					if (!string.IsNullOrWhiteSpace(fullName))
					{
						dialog.InitialDirectory = Path.GetDirectoryName(fullName);
						dialog.FileName = Path.GetFileName(fullName);
					}

					if (dialog.ShowDialog() == DialogResult.OK)
					{
						SchedulerView[e.ColumnIndex, e.RowIndex].Tag = dialog.FileName;
						SchedulerView[e.ColumnIndex, e.RowIndex].Value = Path.GetFileName(dialog.FileName);
					}
					break;
				case SCHEDULE_COL_DATE:
				case SCHEDULE_COL_TIME:
				case SCHEDULE_COL_DEST:
					SchedulerView.BeginEdit(true);
					//((TextBox)dataGridView1.EditingControl).SelectionStart = dataGridView1.CurrentCell.Value.ToString().Length;
					break;
			}
		}

		private void CloseBtn_Click(object sender, EventArgs e)
		{
			/*
			_manager.ScheduleData.SchedulerList = new List<SchedulerItem>();
			foreach (DataGridViewRow row in SchedulerView.Rows)
			{
				SchedulerItem item = new SchedulerItem();
				item.Active = (bool)row.Cells[SCHEDULE_COL_ACTIVE].Value;
				item.Success = (bool)row.Cells[SCHEDULE_COL_SUCCESS].Value;
				item.Error = (bool)row.Cells[SCHEDULE_COL_ERROR].Value;
				DateTime date = (DateTime)row.Cells[SCHEDULE_COL_DATE].Value;
				DateTime time = (DateTime)row.Cells[SCHEDULE_COL_TIME].Value;
				item.Timestamp = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
				item.Destination = (string)row.Cells[SCHEDULE_COL_DEST].Value;
				item.Filename = (string)row.Cells[SCHEDULE_COL_FILE].Tag;
				_manager.SchedulerList.Add(item);
			}

			_manager.SaveScheduler();
			*/
			Close();
		}

		private void SchedulerView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			int rowIndex = e.RowIndex;
			if (rowIndex==-1)
			{
				return;
			}

			SchedulerItem item = _manager.ScheduleData.SchedulerList[rowIndex];
			DataGridViewRow row = SchedulerView.Rows[rowIndex];

			switch(e.ColumnIndex)
			{
				case SCHEDULE_COL_ACTIVE:
					item.Active = (bool)row.Cells[SCHEDULE_COL_ACTIVE].Value;
					break;
				case SCHEDULE_COL_SUCCESS:
					item.Success = (bool)row.Cells[SCHEDULE_COL_SUCCESS].Value;
					break;
				case SCHEDULE_COL_ERROR:
					item.Error = (bool)row.Cells[SCHEDULE_COL_ERROR].Value;
					break;
				case SCHEDULE_COL_DATE:
				case SCHEDULE_COL_TIME:
					DateTime date = (DateTime)row.Cells[SCHEDULE_COL_DATE].Value;
					DateTime time = (DateTime)row.Cells[SCHEDULE_COL_TIME].Value;
					item.Timestamp = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
					break;
				case SCHEDULE_COL_DEST:
					item.Destination = (string)row.Cells[SCHEDULE_COL_DEST].Value;
					break;
				case SCHEDULE_COL_FILE:
					item.Filename = (string)row.Cells[SCHEDULE_COL_FILE].Tag;
					break;
			}

			ShowSchedules();
		}

		private void SchedulerView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
			// end of edition on each click on column of checkbox
			if (e.RowIndex == -1)
			{
				return;
			}
			switch(e.ColumnIndex)
			{
				case SCHEDULE_COL_ACTIVE:
				case SCHEDULE_COL_SUCCESS:
				case SCHEDULE_COL_ERROR:
					SchedulerView.EndEdit();
					break;
			}
		}

		private void SchedulerView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			_manager.SaveScheduler();
		}

		private void SchedulerForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			ClosedEvent?.Invoke();
		}

	}
}
