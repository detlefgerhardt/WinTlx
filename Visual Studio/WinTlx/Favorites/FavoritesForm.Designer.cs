namespace WinTlx.Favorites
{
	partial class FavoritesForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FavoritesForm));
			this.FavoritesTab = new System.Windows.Forms.TabControl();
			this.FavoritesPage1 = new System.Windows.Forms.TabPage();
			this.FavDialBtn = new System.Windows.Forms.Button();
			this.FavDeleteBtn = new System.Windows.Forms.Button();
			this.FavAddBtn = new System.Windows.Forms.Button();
			this.FavList = new System.Windows.Forms.DataGridView();
			this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FavNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FavName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FavAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FavPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FavDirectdial = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FavoritesPage2 = new System.Windows.Forms.TabPage();
			this.HistDialBtn = new System.Windows.Forms.Button();
			this.HistClearBtn = new System.Windows.Forms.Button();
			this.CallHistoryList = new System.Windows.Forms.DataGridView();
			this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FavoritesTab.SuspendLayout();
			this.FavoritesPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.FavList)).BeginInit();
			this.FavoritesPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.CallHistoryList)).BeginInit();
			this.SuspendLayout();
			// 
			// FavoritesTab
			// 
			this.FavoritesTab.Controls.Add(this.FavoritesPage1);
			this.FavoritesTab.Controls.Add(this.FavoritesPage2);
			this.FavoritesTab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FavoritesTab.Location = new System.Drawing.Point(0, 0);
			this.FavoritesTab.Name = "FavoritesTab";
			this.FavoritesTab.SelectedIndex = 0;
			this.FavoritesTab.Size = new System.Drawing.Size(735, 450);
			this.FavoritesTab.TabIndex = 0;
			// 
			// FavoritesPage1
			// 
			this.FavoritesPage1.Controls.Add(this.FavDialBtn);
			this.FavoritesPage1.Controls.Add(this.FavDeleteBtn);
			this.FavoritesPage1.Controls.Add(this.FavAddBtn);
			this.FavoritesPage1.Controls.Add(this.FavList);
			this.FavoritesPage1.Location = new System.Drawing.Point(4, 22);
			this.FavoritesPage1.Name = "FavoritesPage1";
			this.FavoritesPage1.Padding = new System.Windows.Forms.Padding(3);
			this.FavoritesPage1.Size = new System.Drawing.Size(727, 424);
			this.FavoritesPage1.TabIndex = 0;
			this.FavoritesPage1.Text = "Favorites";
			this.FavoritesPage1.UseVisualStyleBackColor = true;
			// 
			// FavDialBtn
			// 
			this.FavDialBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FavDialBtn.Location = new System.Drawing.Point(643, 7);
			this.FavDialBtn.Name = "FavDialBtn";
			this.FavDialBtn.Size = new System.Drawing.Size(80, 23);
			this.FavDialBtn.TabIndex = 3;
			this.FavDialBtn.Text = "Dial";
			this.FavDialBtn.UseVisualStyleBackColor = true;
			this.FavDialBtn.Click += new System.EventHandler(this.FavDialBtn_Click);
			// 
			// FavDeleteBtn
			// 
			this.FavDeleteBtn.Location = new System.Drawing.Point(90, 7);
			this.FavDeleteBtn.Name = "FavDeleteBtn";
			this.FavDeleteBtn.Size = new System.Drawing.Size(75, 23);
			this.FavDeleteBtn.TabIndex = 2;
			this.FavDeleteBtn.Text = "Delete";
			this.FavDeleteBtn.UseVisualStyleBackColor = true;
			this.FavDeleteBtn.Click += new System.EventHandler(this.FavDeleteBtn_Click);
			// 
			// FavAddBtn
			// 
			this.FavAddBtn.Location = new System.Drawing.Point(9, 7);
			this.FavAddBtn.Name = "FavAddBtn";
			this.FavAddBtn.Size = new System.Drawing.Size(75, 23);
			this.FavAddBtn.TabIndex = 1;
			this.FavAddBtn.Text = "Add";
			this.FavAddBtn.UseVisualStyleBackColor = true;
			this.FavAddBtn.Click += new System.EventHandler(this.FavAddBtn_Click);
			// 
			// FavList
			// 
			this.FavList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FavList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.FavList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.FavList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Index,
            this.FavNumber,
            this.FavName,
            this.FavAddress,
            this.FavPort,
            this.FavDirectdial});
			this.FavList.Location = new System.Drawing.Point(3, 36);
			this.FavList.Name = "FavList";
			this.FavList.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.FavList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.FavList.Size = new System.Drawing.Size(721, 385);
			this.FavList.TabIndex = 0;
			this.FavList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.FavList_CellClick);
			this.FavList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.FavList_CellDoubleClick);
			this.FavList.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.FavList_CellEndEdit);
			this.FavList.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.FavList_RowLeave);
			// 
			// Index
			// 
			this.Index.HeaderText = "#";
			this.Index.Name = "Index";
			this.Index.Width = 30;
			// 
			// FavNumber
			// 
			this.FavNumber.HeaderText = "Number";
			this.FavNumber.Name = "FavNumber";
			// 
			// FavName
			// 
			this.FavName.HeaderText = "Name";
			this.FavName.Name = "FavName";
			this.FavName.Width = 200;
			// 
			// FavAddress
			// 
			this.FavAddress.HeaderText = "Address";
			this.FavAddress.Name = "FavAddress";
			// 
			// FavPort
			// 
			this.FavPort.HeaderText = "Port";
			this.FavPort.Name = "FavPort";
			this.FavPort.Width = 50;
			// 
			// FavDirectdial
			// 
			this.FavDirectdial.HeaderText = "Direct dial";
			this.FavDirectdial.Name = "FavDirectdial";
			this.FavDirectdial.Width = 80;
			// 
			// FavoritesPage2
			// 
			this.FavoritesPage2.Controls.Add(this.HistDialBtn);
			this.FavoritesPage2.Controls.Add(this.HistClearBtn);
			this.FavoritesPage2.Controls.Add(this.CallHistoryList);
			this.FavoritesPage2.Location = new System.Drawing.Point(4, 22);
			this.FavoritesPage2.Name = "FavoritesPage2";
			this.FavoritesPage2.Padding = new System.Windows.Forms.Padding(3);
			this.FavoritesPage2.Size = new System.Drawing.Size(727, 424);
			this.FavoritesPage2.TabIndex = 1;
			this.FavoritesPage2.Text = "Call history";
			this.FavoritesPage2.UseVisualStyleBackColor = true;
			// 
			// HistDialBtn
			// 
			this.HistDialBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.HistDialBtn.Location = new System.Drawing.Point(644, 6);
			this.HistDialBtn.Name = "HistDialBtn";
			this.HistDialBtn.Size = new System.Drawing.Size(75, 23);
			this.HistDialBtn.TabIndex = 3;
			this.HistDialBtn.Text = "Dial";
			this.HistDialBtn.UseVisualStyleBackColor = true;
			this.HistDialBtn.Click += new System.EventHandler(this.HistDialBtn_Click);
			// 
			// HistClearBtn
			// 
			this.HistClearBtn.Location = new System.Drawing.Point(8, 6);
			this.HistClearBtn.Name = "HistClearBtn";
			this.HistClearBtn.Size = new System.Drawing.Size(75, 23);
			this.HistClearBtn.TabIndex = 2;
			this.HistClearBtn.Text = "Clear";
			this.HistClearBtn.UseVisualStyleBackColor = true;
			this.HistClearBtn.Click += new System.EventHandler(this.HistClearBtn_Click);
			// 
			// CallHistoryList
			// 
			this.CallHistoryList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.CallHistoryList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.CallHistoryList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date});
			this.CallHistoryList.Location = new System.Drawing.Point(3, 35);
			this.CallHistoryList.Name = "CallHistoryList";
			this.CallHistoryList.Size = new System.Drawing.Size(721, 386);
			this.CallHistoryList.TabIndex = 0;
			this.CallHistoryList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CallHistoryList_CellDoubleClick);
			// 
			// Date
			// 
			this.Date.HeaderText = "Date";
			this.Date.Name = "Date";
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.HeaderText = "#";
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.Width = 30;
			// 
			// dataGridViewTextBoxColumn2
			// 
			this.dataGridViewTextBoxColumn2.HeaderText = "Number";
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			// 
			// dataGridViewTextBoxColumn3
			// 
			this.dataGridViewTextBoxColumn3.HeaderText = "Name";
			this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
			this.dataGridViewTextBoxColumn3.Width = 200;
			// 
			// dataGridViewTextBoxColumn4
			// 
			this.dataGridViewTextBoxColumn4.HeaderText = "Address";
			this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
			// 
			// dataGridViewTextBoxColumn5
			// 
			this.dataGridViewTextBoxColumn5.HeaderText = "Port";
			this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
			this.dataGridViewTextBoxColumn5.Width = 50;
			// 
			// dataGridViewTextBoxColumn6
			// 
			this.dataGridViewTextBoxColumn6.HeaderText = "Direct dial";
			this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
			this.dataGridViewTextBoxColumn6.Width = 80;
			// 
			// FavoritesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(735, 450);
			this.Controls.Add(this.FavoritesTab);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(300, 200);
			this.Name = "FavoritesForm";
			this.Text = "Anrufliste";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FavoritesForm_FormClosed);
			this.Load += new System.EventHandler(this.FavoritesForm_Load);
			this.FavoritesTab.ResumeLayout(false);
			this.FavoritesPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.FavList)).EndInit();
			this.FavoritesPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.CallHistoryList)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl FavoritesTab;
		private System.Windows.Forms.TabPage FavoritesPage1;
		private System.Windows.Forms.TabPage FavoritesPage2;
		private System.Windows.Forms.DataGridView FavList;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
		private System.Windows.Forms.DataGridViewTextBoxColumn Index;
		private System.Windows.Forms.DataGridViewTextBoxColumn FavNumber;
		private System.Windows.Forms.DataGridViewTextBoxColumn FavName;
		private System.Windows.Forms.DataGridViewTextBoxColumn FavAddress;
		private System.Windows.Forms.DataGridViewTextBoxColumn FavPort;
		private System.Windows.Forms.DataGridViewTextBoxColumn FavDirectdial;
		private System.Windows.Forms.DataGridView CallHistoryList;
		private System.Windows.Forms.Button FavDialBtn;
		private System.Windows.Forms.Button FavDeleteBtn;
		private System.Windows.Forms.Button FavAddBtn;
		private System.Windows.Forms.Button HistClearBtn;
		private System.Windows.Forms.Button HistDialBtn;
		private System.Windows.Forms.DataGridViewTextBoxColumn Date;
	}
}