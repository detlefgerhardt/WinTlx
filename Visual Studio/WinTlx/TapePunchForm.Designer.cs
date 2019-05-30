namespace WinTlx
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
			this.OnCb = new System.Windows.Forms.CheckBox();
			this.OffCb = new System.Windows.Forms.CheckBox();
			this.CloseBtn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.PunchedTapePb)).BeginInit();
			this.SuspendLayout();
			// 
			// PunchedTapePb
			// 
			this.PunchedTapePb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.PunchedTapePb.Location = new System.Drawing.Point(12, 12);
			this.PunchedTapePb.Name = "PunchedTapePb";
			this.PunchedTapePb.Size = new System.Drawing.Size(97, 426);
			this.PunchedTapePb.TabIndex = 0;
			this.PunchedTapePb.TabStop = false;
			this.PunchedTapePb.Paint += new System.Windows.Forms.PaintEventHandler(this.PunchedTapePb_Paint);
			// 
			// ClearBtn
			// 
			this.ClearBtn.Location = new System.Drawing.Point(115, 72);
			this.ClearBtn.Name = "ClearBtn";
			this.ClearBtn.Size = new System.Drawing.Size(41, 23);
			this.ClearBtn.TabIndex = 3;
			this.ClearBtn.Text = "Clear";
			this.ClearBtn.UseVisualStyleBackColor = true;
			this.ClearBtn.Click += new System.EventHandler(this.ClearBtn_Click);
			// 
			// OnCb
			// 
			this.OnCb.Appearance = System.Windows.Forms.Appearance.Button;
			this.OnCb.Location = new System.Drawing.Point(115, 12);
			this.OnCb.Name = "OnCb";
			this.OnCb.Size = new System.Drawing.Size(41, 24);
			this.OnCb.TabIndex = 4;
			this.OnCb.Text = "On";
			this.OnCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.OnCb.UseVisualStyleBackColor = true;
			this.OnCb.CheckedChanged += new System.EventHandler(this.OnCb_CheckedChanged);
			this.OnCb.Click += new System.EventHandler(this.OnCb_Click);
			// 
			// OffCb
			// 
			this.OffCb.Appearance = System.Windows.Forms.Appearance.Button;
			this.OffCb.Location = new System.Drawing.Point(115, 42);
			this.OffCb.Name = "OffCb";
			this.OffCb.Size = new System.Drawing.Size(41, 24);
			this.OffCb.TabIndex = 5;
			this.OffCb.Text = "Off";
			this.OffCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.OffCb.UseVisualStyleBackColor = true;
			this.OffCb.CheckedChanged += new System.EventHandler(this.OffCb_CheckedChanged);
			this.OffCb.Click += new System.EventHandler(this.OffCb_Click);
			// 
			// CloseBtn
			// 
			this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.CloseBtn.Location = new System.Drawing.Point(115, 415);
			this.CloseBtn.Name = "CloseBtn";
			this.CloseBtn.Size = new System.Drawing.Size(41, 23);
			this.CloseBtn.TabIndex = 6;
			this.CloseBtn.Text = "Close";
			this.CloseBtn.UseVisualStyleBackColor = true;
			this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
			// 
			// TapePunchForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(163, 450);
			this.Controls.Add(this.CloseBtn);
			this.Controls.Add(this.OffCb);
			this.Controls.Add(this.OnCb);
			this.Controls.Add(this.ClearBtn);
			this.Controls.Add(this.PunchedTapePb);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "TapePunchForm";
			this.Text = "Puncher";
			this.Load += new System.EventHandler(this.PunchTapeForm_Load);
			this.ResizeEnd += new System.EventHandler(this.PunchTapeForm_ResizeEnd);
			((System.ComponentModel.ISupportInitialize)(this.PunchedTapePb)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox PunchedTapePb;
		private System.Windows.Forms.Button ClearBtn;
		private System.Windows.Forms.CheckBox OnCb;
		private System.Windows.Forms.CheckBox OffCb;
		private System.Windows.Forms.Button CloseBtn;
	}
}