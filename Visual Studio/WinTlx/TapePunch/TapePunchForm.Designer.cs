namespace WinTlx.TapePunch
{
	partial class TapePunchForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TapePunchForm));
			this.PunchedTapePb = new System.Windows.Forms.PictureBox();
			this.ClearBtn = new System.Windows.Forms.Button();
			this.RecvCb = new System.Windows.Forms.CheckBox();
			this.CloseBtn = new System.Windows.Forms.Button();
			this.SaveBtn = new System.Windows.Forms.Button();
			this.LoadBtn = new System.Windows.Forms.Button();
			this.SendBtn = new System.Windows.Forms.Button();
			this.BufferStatusLbl = new System.Windows.Forms.Label();
			this.ScrollLeftBtn = new System.Windows.Forms.Button();
			this.ScrollRightBtn = new System.Windows.Forms.Button();
			this.TapePositionSb = new WinTlx.Controls.DelayedHScrollBar();
			this.ScrollFirstBtn = new System.Windows.Forms.Button();
			this.ScrollLastBtn = new System.Windows.Forms.Button();
			this.EditCb = new System.Windows.Forms.CheckBox();
			this.EditDeleteBtn = new System.Windows.Forms.Button();
			this.EditInsertCb = new System.Windows.Forms.CheckBox();
			this.EditStartBtn = new System.Windows.Forms.Button();
			this.EditEndBtn = new System.Windows.Forms.Button();
			this.EditPl = new System.Windows.Forms.Panel();
			this.EditCropBtn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.PunchedTapePb)).BeginInit();
			this.EditPl.SuspendLayout();
			this.SuspendLayout();
			// 
			// PunchedTapePb
			// 
			this.PunchedTapePb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PunchedTapePb.Location = new System.Drawing.Point(12, 71);
			this.PunchedTapePb.Name = "PunchedTapePb";
			this.PunchedTapePb.Size = new System.Drawing.Size(480, 129);
			this.PunchedTapePb.TabIndex = 0;
			this.PunchedTapePb.TabStop = false;
			this.PunchedTapePb.Paint += new System.Windows.Forms.PaintEventHandler(this.PunchedTapePb_Paint);
			// 
			// ClearBtn
			// 
			this.ClearBtn.Location = new System.Drawing.Point(154, 13);
			this.ClearBtn.Name = "ClearBtn";
			this.ClearBtn.Size = new System.Drawing.Size(65, 23);
			this.ClearBtn.TabIndex = 3;
			this.ClearBtn.Text = "Clear";
			this.ClearBtn.UseVisualStyleBackColor = true;
			this.ClearBtn.Click += new System.EventHandler(this.ClearBtn_Click);
			// 
			// RecvCb
			// 
			this.RecvCb.Appearance = System.Windows.Forms.Appearance.Button;
			this.RecvCb.Location = new System.Drawing.Point(12, 12);
			this.RecvCb.Name = "RecvCb";
			this.RecvCb.Size = new System.Drawing.Size(65, 24);
			this.RecvCb.TabIndex = 4;
			this.RecvCb.Text = "Recv";
			this.RecvCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RecvCb.UseVisualStyleBackColor = true;
			this.RecvCb.Click += new System.EventHandler(this.RecvCb_Click);
			// 
			// CloseBtn
			// 
			this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseBtn.Location = new System.Drawing.Point(412, 13);
			this.CloseBtn.Name = "CloseBtn";
			this.CloseBtn.Size = new System.Drawing.Size(80, 23);
			this.CloseBtn.TabIndex = 6;
			this.CloseBtn.Text = "Close";
			this.CloseBtn.UseVisualStyleBackColor = true;
			this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
			// 
			// SaveBtn
			// 
			this.SaveBtn.Location = new System.Drawing.Point(83, 42);
			this.SaveBtn.Name = "SaveBtn";
			this.SaveBtn.Size = new System.Drawing.Size(65, 23);
			this.SaveBtn.TabIndex = 7;
			this.SaveBtn.Text = "Speichern";
			this.SaveBtn.UseVisualStyleBackColor = true;
			this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
			// 
			// LoadBtn
			// 
			this.LoadBtn.Location = new System.Drawing.Point(12, 42);
			this.LoadBtn.Name = "LoadBtn";
			this.LoadBtn.Size = new System.Drawing.Size(65, 23);
			this.LoadBtn.TabIndex = 8;
			this.LoadBtn.Text = "Load";
			this.LoadBtn.UseVisualStyleBackColor = true;
			this.LoadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
			// 
			// SendBtn
			// 
			this.SendBtn.Location = new System.Drawing.Point(82, 13);
			this.SendBtn.Name = "SendBtn";
			this.SendBtn.Size = new System.Drawing.Size(65, 23);
			this.SendBtn.TabIndex = 9;
			this.SendBtn.Text = "Send";
			this.SendBtn.UseVisualStyleBackColor = true;
			this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
			// 
			// BufferStatusLbl
			// 
			this.BufferStatusLbl.AutoSize = true;
			this.BufferStatusLbl.Location = new System.Drawing.Point(256, 18);
			this.BufferStatusLbl.Name = "BufferStatusLbl";
			this.BufferStatusLbl.Size = new System.Drawing.Size(30, 13);
			this.BufferStatusLbl.TabIndex = 10;
			this.BufferStatusLbl.Text = "0 / 0";
			// 
			// ScrollLeftBtn
			// 
			this.ScrollLeftBtn.Location = new System.Drawing.Point(58, 226);
			this.ScrollLeftBtn.Name = "ScrollLeftBtn";
			this.ScrollLeftBtn.Size = new System.Drawing.Size(40, 23);
			this.ScrollLeftBtn.TabIndex = 11;
			this.ScrollLeftBtn.Text = "<<";
			this.ScrollLeftBtn.UseVisualStyleBackColor = true;
			this.ScrollLeftBtn.Click += new System.EventHandler(this.ScrollLeftBtn_Click);
			// 
			// ScrollRightBtn
			// 
			this.ScrollRightBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ScrollRightBtn.Location = new System.Drawing.Point(406, 226);
			this.ScrollRightBtn.Name = "ScrollRightBtn";
			this.ScrollRightBtn.Size = new System.Drawing.Size(40, 23);
			this.ScrollRightBtn.TabIndex = 12;
			this.ScrollRightBtn.Text = ">>";
			this.ScrollRightBtn.UseVisualStyleBackColor = true;
			this.ScrollRightBtn.Click += new System.EventHandler(this.ScrollRightBtn_Click);
			// 
			// TapePositionSb
			// 
			this.TapePositionSb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TapePositionSb.Location = new System.Drawing.Point(12, 203);
			this.TapePositionSb.Name = "TapePositionSb";
			this.TapePositionSb.Size = new System.Drawing.Size(480, 18);
			this.TapePositionSb.TabIndex = 13;
			// 
			// ScrollFirstBtn
			// 
			this.ScrollFirstBtn.Location = new System.Drawing.Point(12, 226);
			this.ScrollFirstBtn.Name = "ScrollFirstBtn";
			this.ScrollFirstBtn.Size = new System.Drawing.Size(40, 23);
			this.ScrollFirstBtn.TabIndex = 14;
			this.ScrollFirstBtn.Text = "|<";
			this.ScrollFirstBtn.UseVisualStyleBackColor = true;
			this.ScrollFirstBtn.Click += new System.EventHandler(this.ScrollFirstBtn_Click);
			// 
			// ScrollLastBtn
			// 
			this.ScrollLastBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ScrollLastBtn.Location = new System.Drawing.Point(452, 226);
			this.ScrollLastBtn.Name = "ScrollLastBtn";
			this.ScrollLastBtn.Size = new System.Drawing.Size(40, 23);
			this.ScrollLastBtn.TabIndex = 15;
			this.ScrollLastBtn.Text = ">|";
			this.ScrollLastBtn.UseVisualStyleBackColor = true;
			this.ScrollLastBtn.Click += new System.EventHandler(this.ScrollLastBtn_Click);
			// 
			// EditCb
			// 
			this.EditCb.Appearance = System.Windows.Forms.Appearance.Button;
			this.EditCb.Location = new System.Drawing.Point(154, 41);
			this.EditCb.Name = "EditCb";
			this.EditCb.Size = new System.Drawing.Size(65, 24);
			this.EditCb.TabIndex = 17;
			this.EditCb.Text = "Edit";
			this.EditCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.EditCb.UseVisualStyleBackColor = true;
			this.EditCb.Click += new System.EventHandler(this.EditCb_Click);
			// 
			// EditDeleteBtn
			// 
			this.EditDeleteBtn.Location = new System.Drawing.Point(81, 0);
			this.EditDeleteBtn.Name = "EditDeleteBtn";
			this.EditDeleteBtn.Size = new System.Drawing.Size(40, 22);
			this.EditDeleteBtn.TabIndex = 18;
			this.EditDeleteBtn.Text = "Del";
			this.EditDeleteBtn.UseVisualStyleBackColor = true;
			this.EditDeleteBtn.Click += new System.EventHandler(this.EditDeleteBtn_Click);
			// 
			// EditInsertCb
			// 
			this.EditInsertCb.Appearance = System.Windows.Forms.Appearance.Button;
			this.EditInsertCb.Location = new System.Drawing.Point(35, 0);
			this.EditInsertCb.Name = "EditInsertCb";
			this.EditInsertCb.Size = new System.Drawing.Size(40, 22);
			this.EditInsertCb.TabIndex = 19;
			this.EditInsertCb.Text = "Ins";
			this.EditInsertCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.EditInsertCb.UseVisualStyleBackColor = true;
			this.EditInsertCb.Click += new System.EventHandler(this.EditInsertCb_Click);
			// 
			// EditStartBtn
			// 
			this.EditStartBtn.Location = new System.Drawing.Point(127, 0);
			this.EditStartBtn.Name = "EditStartBtn";
			this.EditStartBtn.Size = new System.Drawing.Size(40, 22);
			this.EditStartBtn.TabIndex = 20;
			this.EditStartBtn.Text = "Start";
			this.EditStartBtn.UseVisualStyleBackColor = true;
			// 
			// EditEndBtn
			// 
			this.EditEndBtn.Location = new System.Drawing.Point(173, 0);
			this.EditEndBtn.Name = "EditEndBtn";
			this.EditEndBtn.Size = new System.Drawing.Size(40, 22);
			this.EditEndBtn.TabIndex = 21;
			this.EditEndBtn.Text = "End";
			this.EditEndBtn.UseVisualStyleBackColor = true;
			// 
			// EditPl
			// 
			this.EditPl.Controls.Add(this.EditCropBtn);
			this.EditPl.Controls.Add(this.EditInsertCb);
			this.EditPl.Controls.Add(this.EditEndBtn);
			this.EditPl.Controls.Add(this.EditDeleteBtn);
			this.EditPl.Controls.Add(this.EditStartBtn);
			this.EditPl.Location = new System.Drawing.Point(104, 226);
			this.EditPl.Name = "EditPl";
			this.EditPl.Size = new System.Drawing.Size(295, 26);
			this.EditPl.TabIndex = 22;
			// 
			// EditCropBtn
			// 
			this.EditCropBtn.Location = new System.Drawing.Point(219, 0);
			this.EditCropBtn.Name = "EditCropBtn";
			this.EditCropBtn.Size = new System.Drawing.Size(40, 22);
			this.EditCropBtn.TabIndex = 22;
			this.EditCropBtn.Text = "Crop";
			this.EditCropBtn.UseVisualStyleBackColor = true;
			// 
			// TapePunchForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(504, 262);
			this.Controls.Add(this.EditPl);
			this.Controls.Add(this.EditCb);
			this.Controls.Add(this.ScrollLastBtn);
			this.Controls.Add(this.ScrollFirstBtn);
			this.Controls.Add(this.TapePositionSb);
			this.Controls.Add(this.ScrollRightBtn);
			this.Controls.Add(this.ScrollLeftBtn);
			this.Controls.Add(this.BufferStatusLbl);
			this.Controls.Add(this.SendBtn);
			this.Controls.Add(this.LoadBtn);
			this.Controls.Add(this.SaveBtn);
			this.Controls.Add(this.CloseBtn);
			this.Controls.Add(this.RecvCb);
			this.Controls.Add(this.ClearBtn);
			this.Controls.Add(this.PunchedTapePb);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(520, 300);
			this.Name = "TapePunchForm";
			this.Text = "Puncher";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TapePunchHorizontalForm_FormClosed);
			this.Load += new System.EventHandler(this.PunchTapeForm_Load);
			this.ResizeEnd += new System.EventHandler(this.PunchTapeForm_ResizeEnd);
			((System.ComponentModel.ISupportInitialize)(this.PunchedTapePb)).EndInit();
			this.EditPl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox PunchedTapePb;
		private System.Windows.Forms.Button ClearBtn;
		private System.Windows.Forms.CheckBox RecvCb;
		private System.Windows.Forms.Button CloseBtn;
		private System.Windows.Forms.Button SaveBtn;
		private System.Windows.Forms.Button LoadBtn;
		private System.Windows.Forms.Button SendBtn;
		private System.Windows.Forms.Label BufferStatusLbl;
		private System.Windows.Forms.Button ScrollLeftBtn;
		private System.Windows.Forms.Button ScrollRightBtn;
		private Controls.DelayedHScrollBar TapePositionSb;
		private System.Windows.Forms.Button ScrollFirstBtn;
		private System.Windows.Forms.Button ScrollLastBtn;
		private System.Windows.Forms.CheckBox EditCb;
		private System.Windows.Forms.Button EditDeleteBtn;
		private System.Windows.Forms.CheckBox EditInsertCb;
		private System.Windows.Forms.Button EditStartBtn;
		private System.Windows.Forms.Button EditEndBtn;
		private System.Windows.Forms.Panel EditPl;
		private System.Windows.Forms.Button EditCropBtn;
	}
}