using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using WinTlx.Languages;

namespace WinTlx.Favorites
{
	public partial class FavoritesForm : Form
	{
		private const string TAG = nameof(FavoritesForm);

		public delegate void CloseEventHandler();
		public event CloseEventHandler CloseEditor;

		private readonly FavoritesManager _favoritesManager;

		private readonly Timer _favListUpdateTimer;
		private bool _favListUpdate = false;

		private FavoritesItemSorter.SortIndex? _favListSortColumnIndex;
		private bool _favListSortColumnAscend;

		private Rectangle _parentWindowsPosition;

		public FavoritesForm(Rectangle position)
		{
			_parentWindowsPosition = position;

			InitializeComponent();

			FavoritesTab.TabPages[0].BackColor = SystemColors.Window;
			FavoritesTab.TabPages[1].BackColor = SystemColors.Window;

			_favoritesManager = FavoritesManager.Instance;
			FavListSetColumns();
			_favListSortColumnIndex = null;
			_favListSortColumnAscend = true;
			_favoritesManager.FavListLoad();

			_favListUpdate = false;
			_favListUpdateTimer = new Timer();
			_favListUpdateTimer.Interval = 100;
			_favListUpdateTimer.Tick += FavListUpdateTimer_Tick;
			_favListUpdateTimer.Start();

			CallHistorySetColumns();
			_favoritesManager.UpdateCallHistory += FavoritesManager_UpdateCallHistory;
			_favoritesManager.CallHistoryLoad();

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageChanged();

			FavListShow();
			CallHistoryShow();
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			this.Text = $"{Constants.PROGRAM_NAME} {LngText(LngKeys.Favorites_Header)}";
			FavAddBtn.Text = LngText(LngKeys.Favorites_FavAddButton);
			FavDeleteBtn.Text = LngText(LngKeys.Favorites_FavDeleteButton);
			FavDialBtn.Text = LngText(LngKeys.Favorites_FavDialButton);
			HistClearBtn.Text = LngText(LngKeys.Favorites_HistClearButton);
			HistDialBtn.Text = LngText(LngKeys.Favorites_HistDialButton);
			FavoritesTab.TabPages[0].Text = LngText(LngKeys.Favorites_FavoritesTab);
			FavoritesTab.TabPages[1].Text = LngText(LngKeys.Favorites_CallHistoryTab);
			FavListSetColumns();
			FavListShow();
			CallHistorySetColumns();
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		private void FavoritesForm_Load(object sender, System.EventArgs e)
		{
			Point pos = Helper.CenterForm(this, _parentWindowsPosition);
			SetBounds(pos.X, pos.Y, Bounds.Width, Bounds.Height);
		}

		private void FavoritesForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			_favoritesManager.UpdateCallHistory -= FavoritesManager_UpdateCallHistory;
			LanguageManager.Instance.LanguageChanged -= LanguageChanged;
			CloseEditor?.Invoke();
		}

		#region FavList

		public const int FAVLIST_INDEX = 0;
		public const int FAVLIST_NUMBER = 1;
		public const int FAVLIST_NAME = 2;
		public const int FAVLIST_ADDRESS = 3;
		public const int FAVLIST_PORT = 4;
		public const int FAVLIST_DIRECTDIAL = 5;

		private void FavListUpdateTimer_Tick(object sender, System.EventArgs e)
		{
			if (_favListUpdate)
			{
				FavListShow();
				_favListUpdate = false;
			}
		}

		private void FavListSetColumns()
		{
			FavList.BackgroundColor = SystemColors.Window;
			FavList.RowHeadersVisible = true;
			FavList.ScrollBars = ScrollBars.Both;
			FavList.RowHeadersVisible = false;
			FavList.MultiSelect = false;
			FavList.AllowUserToAddRows = false;
			FavList.ShowEditingIcon = false;
			FavList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

			FavList.Columns.Clear();

			var textCol = new DataGridViewTextBoxColumn
			{
				Name = "Index",
				HeaderText = "#",
				Width = 30,
				ReadOnly = true,
				Resizable = DataGridViewTriState.False,
				SortMode = DataGridViewColumnSortMode.NotSortable,
			};
			FavList.Columns.Add(textCol);

			textCol = new DataGridViewTextBoxColumn
			{
				Name = "Number",
				HeaderText = LngText(LngKeys.Favorites_EntryNumber),
				Width = 80,
				Resizable = DataGridViewTriState.False,
				SortMode = DataGridViewColumnSortMode.NotSortable,
			};
			FavList.Columns.Add(textCol);

			textCol = new DataGridViewTextBoxColumn
			{
				Name = "Name",
				HeaderText = LngText(LngKeys.Favorites_EntryName),
				//Width = 200,
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
				MinimumWidth = 100,
				SortMode = DataGridViewColumnSortMode.NotSortable,
			};
			FavList.Columns.Add(textCol);

			textCol = new DataGridViewTextBoxColumn
			{
				Name = "Address",
				HeaderText = LngText(LngKeys.Favorites_EntryAddress),
				Width = 100,
				Resizable = DataGridViewTriState.False,
				SortMode = DataGridViewColumnSortMode.NotSortable,
			};
			FavList.Columns.Add(textCol);

			textCol = new DataGridViewTextBoxColumn
			{
				Name = "Port",
				HeaderText = LngText(LngKeys.Favorites_EntryPort),
				Width = 38,
				Resizable = DataGridViewTriState.False,
				SortMode = DataGridViewColumnSortMode.NotSortable,
			};
			FavList.Columns.Add(textCol);

			textCol = new DataGridViewTextBoxColumn
			{
				Name = "DirectDial",
				HeaderText = LngText(LngKeys.Favorites_EntryDirectDial),
				//Width = 70,
				Resizable = DataGridViewTriState.False,
				AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
				SortMode = DataGridViewColumnSortMode.NotSortable,
			};
			FavList.Columns.Add(textCol);
		}

		private void FavAddBtn_Click(object sender, System.EventArgs e)
		{
			FavListAddEmptyRow();
		}

		private void FavDeleteBtn_Click(object sender, System.EventArgs e)
		{
			FavoriteItem favItem = GetSelectedFavItem();
			if (favItem == null) return;

			_favoritesManager.DelFavorite(favItem.Number);
			_favListUpdate = true;
			_favoritesManager.FavListSave();
		}

		private void FavDialBtn_Click(object sender, System.EventArgs e)
		{
			FavDial();
		}

		private void FavList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			//Debug.WriteLine("FavList_CellEndEdit");
			List<FavoriteItem> favList = FavListGet(false);
			//Debug.WriteLine($"changed={_favoritesManager.FavListCheckChanged(favList)}");
			if (!_favoritesManager.FavListCheckChanged(favList)) return;

			_favoritesManager.Favorites = favList;
			_favoritesManager.FavListSave();

			//int rowIndex = e.RowIndex;
			_favListUpdate = true;
		}

		private void FavList_RowLeave(object sender, DataGridViewCellEventArgs e)
		{
		}

		private void FavList_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			FavListCellClick(sender, e);
		}

		private void FavList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0)
			{
				FavDial();
			}
		}

		private void FavListCellClick(object sender, DataGridViewCellEventArgs e)
		{
			int colIndex = e.ColumnIndex;
			int rowIndex = e.RowIndex;
			if (rowIndex < 0)
			{
				FavListSortList(colIndex);
				FavListSetSortIndicator();
				return;
			}
		}

		private void FavListShow()
		{
			int scrollPos = FavList.FirstDisplayedScrollingRowIndex;
			int? selectedIndex = null;
			for (int i=0; i<FavList.Rows.Count; i++)
			{
				if (FavList.Rows[i].Selected)
				{
					selectedIndex = i;
					break;
				}
			}

			FavList.SuspendLayout();

			List<FavoriteItem> favList = _favoritesManager.Favorites;

			if (_favListSortColumnIndex.HasValue)
			{
				favList.Sort(new FavoritesItemSorter(_favListSortColumnIndex.Value, _favListSortColumnAscend));
			}

			List<DataGridViewRow> rows = new List<DataGridViewRow>();
			for (int favIdx=0; favIdx < favList.Count; favIdx++)
			{
				FavoriteItem favItem = favList[favIdx];
				if (favItem.IsEmpty) continue;

				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(FavList);
				row.Cells[FAVLIST_INDEX].Value = (favIdx + 1).ToString();
				row.Cells[FAVLIST_NUMBER].Value = favItem.Number;
				row.Cells[FAVLIST_NAME].Value = favItem.Name;
				row.Cells[FAVLIST_ADDRESS].Value = favItem.Address;
				row.Cells[FAVLIST_PORT].Value = favItem.Port > 0 ? favItem.Port.ToString() : "";
				row.Cells[FAVLIST_DIRECTDIAL].Value = favItem.DirectDial > 0 ? favItem.DirectDial.ToString() : "";

				/*
				var btnCell = (row.Cells[FAVLIST_DIALBTN] as DataGridViewButtonCell);
				//btnCell.ToolTipText = "Add/Delete HD Timer";
				if (favItem.IsValid)
				{
					btnCell.Style.ForeColor = Color.Black;
				}
				else
				{
					btnCell.Style.ForeColor = Color.LightGray;
				}
				*/

				rows.Add(row);
			}

			FavList.Rows.Clear();
			FavList.Rows.AddRange(rows.ToArray());

			if (selectedIndex.HasValue && selectedIndex<FavList.Rows.Count)
			{
				FavList.Rows[selectedIndex.Value].Selected = true;
			}

			if (scrollPos > -1 && scrollPos < FavList.Rows.Count)
			{
				FavList.FirstDisplayedScrollingRowIndex = scrollPos;
			}

			FavList.ResumeLayout();
		}

		private FavoritesItemSorter.SortIndex? FavListSortColumnToSortIndex(int? sortColumn)
		{
			if (!sortColumn.HasValue) return null;
			switch(sortColumn.Value)
			{
				case FAVLIST_NUMBER:
					return FavoritesItemSorter.SortIndex.Number;
				case FAVLIST_NAME:
					return FavoritesItemSorter.SortIndex.Name;
				default:
					return null;
			}
		}

		private void FavListAddEmptyRow()
		{
			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(FavList);
			row.Cells[FAVLIST_INDEX].Value = "";
			row.Cells[FAVLIST_NUMBER].Value = "";
			row.Cells[FAVLIST_NAME].Value = "";
			row.Cells[FAVLIST_ADDRESS].Value = "";
			row.Cells[FAVLIST_PORT].Value = "";
			row.Cells[FAVLIST_DIRECTDIAL].Value = "";
			//(row.Cells[FAVLIST_DIALBTN] as DataGridViewButtonCell).Style.ForeColor = Color.LightGray;
			//(row.Cells[FAVLIST_DELBTN] as DataGridViewButtonCell).Style.ForeColor = Color.LightGray;
			FavList.Rows.Add(row);
		}

		private List<FavoriteItem> FavListGet(bool onlyValid)
		{
			List<FavoriteItem> favList = new List<FavoriteItem>();
			foreach(DataGridViewRow row in FavList.Rows)
			{
				FavoriteItem favItem = new FavoriteItem()
				{
					Number = FavListGetNumber(row.Cells[FAVLIST_NUMBER].Value),
					Name = FavListGetString(row.Cells[FAVLIST_NAME].Value),
					Address = FavListGetString(row.Cells[FAVLIST_ADDRESS].Value),
					Port = FavListGetInt(row.Cells[FAVLIST_PORT].Value),
					DirectDial = FavListGetInt(row.Cells[FAVLIST_DIRECTDIAL].Value),
				};
				//Debug.WriteLine(favItem);
				if ((!onlyValid || favItem.IsValid) && !favItem.IsEmpty)
				{
					favList.Add(favItem);
				}
			}
			return favList;
		}

		private void FavDial()
		{
			FavoriteItem favItem = GetSelectedFavItem();
			if (favItem == null) return;
			_favoritesManager.FavListDial(favItem);
			Close();
		}

		private FavoriteItem GetSelectedFavItem()
		{
			if (FavList.SelectedRows.Count == 0) return null;

			DataGridViewRow row = FavList.SelectedRows[0];
			string number = FavListGetString(row.Cells[FAVLIST_NUMBER].Value);
			return _favoritesManager.FavListGetFavorite(number);
		}

		private string FavListGetString(object value)
		{
			string valueStr = value as string;
			if (valueStr != null) valueStr = valueStr.Trim();
			return string.IsNullOrEmpty(valueStr) ? null : valueStr;
		}

		private string FavListGetNumber(object value)
		{
			string valueStr = FavListGetString(value);
			if (string.IsNullOrEmpty(valueStr)) return valueStr;

			string valueNum = "";
			foreach(char chr in valueStr)
			{
				if (char.IsDigit(chr)) valueNum += chr;
			}
			return valueNum;
		}

		private int FavListGetInt(object value)
		{
			string valueStr = value as string;
			if (valueStr != null) valueStr = valueStr.Trim();
			if (int.TryParse(valueStr, out int valueInt))
			{
				return valueInt;
			}
			else
			{
				return 0;
			}
		}


		private void FavListSortList(int colIndex)
		{
			switch (colIndex)
			{
				case FAVLIST_NUMBER:
					_favListUpdate = true;
					if (_favListSortColumnIndex == FavoritesItemSorter.SortIndex.Number)
					{
						_favListSortColumnAscend = !_favListSortColumnAscend;
						return;
					}
					_favListSortColumnIndex = FavoritesItemSorter.SortIndex.Number;
					_favListSortColumnAscend = true;
					break;
				case FAVLIST_NAME:
					_favListUpdate = true;
					if (_favListSortColumnIndex == FavoritesItemSorter.SortIndex.Name)
					{
						_favListSortColumnAscend = !_favListSortColumnAscend;
						return;
					}
					_favListSortColumnIndex = FavoritesItemSorter.SortIndex.Name;
					_favListSortColumnAscend = true;
					break;
				default:
					//_sortColumnIndex = null;
					break;
			}
		}

		private void FavListSetSortIndicator()
		{
			//string upStr = " \x25B2";
			//string dnStr = " \x25BC";
			//string upStr = " \x25B4";
			//string dnStr = " \x25BE";
			string upStr = " \x2191";
			string dnStr = " \x2193";

			int? sortColumn = null;
			switch (_favListSortColumnIndex)
			{
				case FavoritesItemSorter.SortIndex.Number:
					sortColumn = FAVLIST_NUMBER;
					break;
				case FavoritesItemSorter.SortIndex.Name:
					sortColumn = FAVLIST_NAME;
					break;
			}

			for (int i=0; i<FavList.Columns.Count; i++)
			{
				var col = FavList.Columns[i];
				if (col.HeaderText.EndsWith(upStr) || col.HeaderText.EndsWith(dnStr))
				{
					col.HeaderText = col.HeaderText.Substring(0, col.HeaderText.Length - 2);
				}
				if (sortColumn.HasValue && i==sortColumn)
				{
					if (_favListSortColumnAscend)
					{
						col.HeaderText += upStr;
					}
					else
					{
						col.HeaderText += dnStr;
					}
				}
			}
		}

		#endregion FavList

		#region Call history

		public const int HISTLIST_DATE = 0;
		public const int HISTLIST_NUMBER = 1;
		public const int HISTLIST_NAME = 2;
		public const int HISTLIST_RESULT = 3;

		private void CallHistorySetColumns()
		{
			CallHistoryList.BackgroundColor = SystemColors.Window;
			CallHistoryList.RowHeadersVisible = true;
			CallHistoryList.ScrollBars = ScrollBars.Both;
			CallHistoryList.RowHeadersVisible = false;
			CallHistoryList.MultiSelect = false;
			CallHistoryList.AllowUserToAddRows = false;
			CallHistoryList.ShowEditingIcon = false;
			CallHistoryList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

			CallHistoryList.Columns.Clear();

			var textCol = new DataGridViewTextBoxColumn
			{
				Name = "CallHistDate",
				HeaderText = LngText(LngKeys.Favorites_EntryDate),
				Width = 95,
				Resizable = DataGridViewTriState.False,
				SortMode = DataGridViewColumnSortMode.NotSortable,
				ReadOnly = true,
			};
			CallHistoryList.Columns.Add(textCol);

			textCol = new DataGridViewTextBoxColumn
			{
				Name = "CallHistNumber",
				HeaderText = LngText(LngKeys.Favorites_EntryNumber),
				Width = 80,
				Resizable = DataGridViewTriState.False,
				SortMode = DataGridViewColumnSortMode.NotSortable,
				ReadOnly = true,
			};
			CallHistoryList.Columns.Add(textCol);

			textCol = new DataGridViewTextBoxColumn
			{
				Name = "CallHistName",
				HeaderText = LngText(LngKeys.Favorites_EntryName),
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
				MinimumWidth = 100,
				SortMode = DataGridViewColumnSortMode.NotSortable,
				ReadOnly = true,
			};
			CallHistoryList.Columns.Add(textCol);

			textCol = new DataGridViewTextBoxColumn
			{
				Name = "CallHistResult",
				HeaderText = LngText(LngKeys.Favorites_EntryResult),
				Width = 100,
				Resizable = DataGridViewTriState.False,
				SortMode = DataGridViewColumnSortMode.NotSortable,
				ReadOnly = true,
			};
			CallHistoryList.Columns.Add(textCol);
		}

		private void HistClearBtn_Click(object sender, System.EventArgs e)
		{
			DialogResult result = MessageBox.Show(
				LngText(LngKeys.Favorites_ClearHistMessage),
				LngText(LngKeys.Favorites_ClearHistHeader),
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Warning,
				MessageBoxDefaultButton.Button2);

			if (result == DialogResult.OK) _favoritesManager.CallHistoryClear();
		}

		private void HistDialBtn_Click(object sender, System.EventArgs e)
		{
			HistDial();
		}

		private void CallHistoryList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			HistDial();
		}

		private void HistDial()
		{
			if (CallHistoryList.SelectedRows.Count == 0) return;

			DataGridViewRow row = CallHistoryList.SelectedRows[0];
			FavoriteItem favItem = new FavoriteItem()
			{
				Number = FavListGetString(row.Cells[HISTLIST_NUMBER].Value),
				Name = FavListGetString(row.Cells[HISTLIST_NAME].Value),
			};

			_favoritesManager.FavListDial(favItem);
			Close();
		}

		private void FavoritesManager_UpdateCallHistory()
		{
			CallHistoryShow();
		}

		private void CallHistoryShow()
		{
			List<CallHistoryItem> histList = _favoritesManager.History;
			histList.Sort(new CallHistoryItemSorter());

			List<DataGridViewRow> rows = new List<DataGridViewRow>();
			for (int histIdx = 0; histIdx < histList.Count; histIdx++)
			{
				CallHistoryItem histItem = histList[histIdx];

				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(CallHistoryList);
				row.Cells[HISTLIST_DATE].Value = histItem.TimeStamp.ToString("dd.MM.yyyy HH:mm");
				row.Cells[HISTLIST_NUMBER].Value = histItem.Number;
				row.Cells[HISTLIST_NAME].Value = histItem.Name;
				row.Cells[HISTLIST_RESULT].Value = histItem.Result;
				rows.Add(row);
			}

			CallHistoryList.Rows.Clear();
			CallHistoryList.Rows.AddRange(rows.ToArray());

			//CallHistoryList.FirstDisplayedCell = CallHistoryList.Rows[CallHistoryList.Rows.Count - 1].Cells[0];
		}

		#endregion Call history

	}
}
