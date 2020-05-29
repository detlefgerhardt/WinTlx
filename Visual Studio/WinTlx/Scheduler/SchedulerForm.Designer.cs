namespace WinTlx.Scheduler
{
	partial class SchedulerForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchedulerForm));
			this.SchedularView = new System.Windows.Forms.DataGridView();
			this.AddEntryBtn = new System.Windows.Forms.Button();
			this.CloseBtn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.SchedularView)).BeginInit();
			this.SuspendLayout();
			// 
			// SchedularView
			// 
			this.SchedularView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.SchedularView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.SchedularView.Location = new System.Drawing.Point(12, 12);
			this.SchedularView.Name = "SchedularView";
			this.SchedularView.Size = new System.Drawing.Size(719, 257);
			this.SchedularView.TabIndex = 0;
			this.SchedularView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SchedularView_CellClick);
			this.SchedularView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.SchedularView_CellEndEdit);
			this.SchedularView.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.SchedularView_CellMouseEnter);
			this.SchedularView.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.SchedularView_CellMouseUp);
			this.SchedularView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.SchedularView_CellValueChanged);
			// 
			// AddEntryBtn
			// 
			this.AddEntryBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AddEntryBtn.Location = new System.Drawing.Point(12, 275);
			this.AddEntryBtn.Name = "AddEntryBtn";
			this.AddEntryBtn.Size = new System.Drawing.Size(80, 23);
			this.AddEntryBtn.TabIndex = 1;
			this.AddEntryBtn.Text = "Add schedule";
			this.AddEntryBtn.UseVisualStyleBackColor = true;
			this.AddEntryBtn.Click += new System.EventHandler(this.AddScheduleBtn_Click);
			// 
			// CloseBtn
			// 
			this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseBtn.Location = new System.Drawing.Point(651, 276);
			this.CloseBtn.Name = "CloseBtn";
			this.CloseBtn.Size = new System.Drawing.Size(80, 23);
			this.CloseBtn.TabIndex = 3;
			this.CloseBtn.Text = "Close";
			this.CloseBtn.UseVisualStyleBackColor = true;
			this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
			// 
			// SchedulerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(743, 311);
			this.Controls.Add(this.CloseBtn);
			this.Controls.Add(this.AddEntryBtn);
			this.Controls.Add(this.SchedularView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(300, 200);
			this.Name = "SchedulerForm";
			this.Text = "Scheduler";
			((System.ComponentModel.ISupportInitialize)(this.SchedularView)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView SchedularView;
		private System.Windows.Forms.Button AddEntryBtn;
		private System.Windows.Forms.Button CloseBtn;
	}
}