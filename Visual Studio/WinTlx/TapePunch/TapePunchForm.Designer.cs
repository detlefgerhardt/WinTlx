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
			this.BufferLbl = new System.Windows.Forms.Label();
			this.ScrollLeftBtn = new System.Windows.Forms.Button();
			this.ScrollRightBtn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.PunchedTapePb)).BeginInit();
			this.SuspendLayout();
			// 
			// PunchedTapePb
			// 
			this.PunchedTapePb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PunchedTapePb.Location = new System.Drawing.Point(12, 80);
			this.PunchedTapePb.Name = "PunchedTapePb";
			this.PunchedTapePb.Size = new System.Drawing.Size(479, 111);
			this.PunchedTapePb.TabIndex = 0;
			this.PunchedTapePb.TabStop = false;
			this.PunchedTapePb.Paint += new System.Windows.Forms.PaintEventHandler(this.PunchedTapePb_Paint);
			// 
			// ClearBtn
			// 
			this.ClearBtn.Location = new System.Drawing.Point(156, 13);
			this.ClearBtn.Name = "ClearBtn";
			this.ClearBtn.Size = new System.Drawing.Size(60, 23);
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
			this.RecvCb.Size = new System.Drawing.Size(60, 24);
			this.RecvCb.TabIndex = 4;
			this.RecvCb.Text = "Recv";
			this.RecvCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RecvCb.UseVisualStyleBackColor = true;
			this.RecvCb.CheckedChanged += new System.EventHandler(this.RecvCb_CheckedChanged);
			this.RecvCb.Click += new System.EventHandler(this.RecvCb_Click);
			// 
			// CloseBtn
			// 
			this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseBtn.Location = new System.Drawing.Point(411, 13);
			this.CloseBtn.Name = "CloseBtn";
			this.CloseBtn.Size = new System.Drawing.Size(80, 23);
			this.CloseBtn.TabIndex = 6;
			this.CloseBtn.Text = "Close";
			this.CloseBtn.UseVisualStyleBackColor = true;
			this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
			// 
			// SaveBtn
			// 
			this.SaveBtn.Location = new System.Drawing.Point(90, 42);
			this.SaveBtn.Name = "SaveBtn";
			this.SaveBtn.Size = new System.Drawing.Size(60, 23);
			this.SaveBtn.TabIndex = 7;
			this.SaveBtn.Text = "Save";
			this.SaveBtn.UseVisualStyleBackColor = true;
			this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
			// 
			// LoadBtn
			// 
			this.LoadBtn.Location = new System.Drawing.Point(90, 13);
			this.LoadBtn.Name = "LoadBtn";
			this.LoadBtn.Size = new System.Drawing.Size(60, 23);
			this.LoadBtn.TabIndex = 8;
			this.LoadBtn.Text = "Load";
			this.LoadBtn.UseVisualStyleBackColor = true;
			this.LoadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
			// 
			// SendBtn
			// 
			this.SendBtn.Location = new System.Drawing.Point(12, 42);
			this.SendBtn.Name = "SendBtn";
			this.SendBtn.Size = new System.Drawing.Size(60, 23);
			this.SendBtn.TabIndex = 9;
			this.SendBtn.Text = "Send";
			this.SendBtn.UseVisualStyleBackColor = true;
			this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
			// 
			// BufferLbl
			// 
			this.BufferLbl.AutoSize = true;
			this.BufferLbl.Location = new System.Drawing.Point(242, 18);
			this.BufferLbl.Name = "BufferLbl";
			this.BufferLbl.Size = new System.Drawing.Size(47, 13);
			this.BufferLbl.TabIndex = 10;
			this.BufferLbl.Text = "0 / 0 / 0";
			// 
			// ScrollLeftBtn
			// 
			this.ScrollLeftBtn.Location = new System.Drawing.Point(12, 197);
			this.ScrollLeftBtn.Name = "ScrollLeftBtn";
			this.ScrollLeftBtn.Size = new System.Drawing.Size(60, 23);
			this.ScrollLeftBtn.TabIndex = 11;
			this.ScrollLeftBtn.Text = "<";
			this.ScrollLeftBtn.UseVisualStyleBackColor = true;
			this.ScrollLeftBtn.Click += new System.EventHandler(this.ScrollLeftBtn_Click);
			// 
			// ScrollRightBtn
			// 
			this.ScrollRightBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ScrollRightBtn.Location = new System.Drawing.Point(431, 197);
			this.ScrollRightBtn.Name = "ScrollRightBtn";
			this.ScrollRightBtn.Size = new System.Drawing.Size(60, 23);
			this.ScrollRightBtn.TabIndex = 12;
			this.ScrollRightBtn.Text = ">";
			this.ScrollRightBtn.UseVisualStyleBackColor = true;
			this.ScrollRightBtn.Click += new System.EventHandler(this.ScrollRightBtn_Click);
			// 
			// TapePunchHorizontalForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(503, 232);
			this.Controls.Add(this.ScrollRightBtn);
			this.Controls.Add(this.ScrollLeftBtn);
			this.Controls.Add(this.BufferLbl);
			this.Controls.Add(this.SendBtn);
			this.Controls.Add(this.LoadBtn);
			this.Controls.Add(this.SaveBtn);
			this.Controls.Add(this.CloseBtn);
			this.Controls.Add(this.RecvCb);
			this.Controls.Add(this.ClearBtn);
			this.Controls.Add(this.PunchedTapePb);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "TapePunchHorizontalForm";
			this.Text = "Puncher";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TapePunchHorizontalForm_FormClosed);
			this.Load += new System.EventHandler(this.PunchTapeForm_Load);
			this.ResizeEnd += new System.EventHandler(this.PunchTapeForm_ResizeEnd);
			((System.ComponentModel.ISupportInitialize)(this.PunchedTapePb)).EndInit();
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
		private System.Windows.Forms.Label BufferLbl;
		private System.Windows.Forms.Button ScrollLeftBtn;
		private System.Windows.Forms.Button ScrollRightBtn;
	}
}