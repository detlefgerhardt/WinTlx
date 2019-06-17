namespace WinTlx
{
	partial class SendFileForm
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
			this.TextTb = new System.Windows.Forms.TextBox();
			this.LoadBtn = new System.Windows.Forms.Button();
			this.LineLengthLbl = new System.Windows.Forms.Label();
			this.LineLengthTb = new System.Windows.Forms.TextBox();
			this.CropRightRb = new System.Windows.Forms.RadioButton();
			this.CropCenterRb = new System.Windows.Forms.RadioButton();
			this.CropLeftRb = new System.Windows.Forms.RadioButton();
			this.CroppingGb = new System.Windows.Forms.GroupBox();
			this.ConvertCb = new System.Windows.Forms.CheckBox();
			this.SendBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.RulerPnl = new System.Windows.Forms.Panel();
			this.CroppingGb.SuspendLayout();
			this.SuspendLayout();
			// 
			// TextTb
			// 
			this.TextTb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextTb.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TextTb.Location = new System.Drawing.Point(13, 20);
			this.TextTb.Multiline = true;
			this.TextTb.Name = "TextTb";
			this.TextTb.Size = new System.Drawing.Size(497, 286);
			this.TextTb.TabIndex = 0;
			// 
			// LoadBtn
			// 
			this.LoadBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LoadBtn.Location = new System.Drawing.Point(516, 12);
			this.LoadBtn.Name = "LoadBtn";
			this.LoadBtn.Size = new System.Drawing.Size(71, 23);
			this.LoadBtn.TabIndex = 1;
			this.LoadBtn.Text = "Load file";
			this.LoadBtn.UseVisualStyleBackColor = true;
			this.LoadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
			// 
			// LineLengthLbl
			// 
			this.LineLengthLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LineLengthLbl.AutoSize = true;
			this.LineLengthLbl.Location = new System.Drawing.Point(525, 52);
			this.LineLengthLbl.Name = "LineLengthLbl";
			this.LineLengthLbl.Size = new System.Drawing.Size(59, 13);
			this.LineLengthLbl.TabIndex = 2;
			this.LineLengthLbl.Text = "Line length";
			// 
			// LineLengthTb
			// 
			this.LineLengthTb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LineLengthTb.Location = new System.Drawing.Point(528, 69);
			this.LineLengthTb.Name = "LineLengthTb";
			this.LineLengthTb.Size = new System.Drawing.Size(46, 20);
			this.LineLengthTb.TabIndex = 3;
			this.LineLengthTb.Leave += new System.EventHandler(this.LineLengthTb_Leave);
			// 
			// CropRightRb
			// 
			this.CropRightRb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CropRightRb.AutoSize = true;
			this.CropRightRb.Location = new System.Drawing.Point(6, 19);
			this.CropRightRb.Name = "CropRightRb";
			this.CropRightRb.Size = new System.Drawing.Size(50, 17);
			this.CropRightRb.TabIndex = 4;
			this.CropRightRb.TabStop = true;
			this.CropRightRb.Text = "Right";
			this.CropRightRb.UseVisualStyleBackColor = true;
			// 
			// CropCenterRb
			// 
			this.CropCenterRb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CropCenterRb.AutoSize = true;
			this.CropCenterRb.Location = new System.Drawing.Point(6, 42);
			this.CropCenterRb.Name = "CropCenterRb";
			this.CropCenterRb.Size = new System.Drawing.Size(56, 17);
			this.CropCenterRb.TabIndex = 6;
			this.CropCenterRb.TabStop = true;
			this.CropCenterRb.Text = "Center";
			this.CropCenterRb.UseVisualStyleBackColor = true;
			// 
			// CropLeftRb
			// 
			this.CropLeftRb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CropLeftRb.AutoSize = true;
			this.CropLeftRb.Location = new System.Drawing.Point(6, 65);
			this.CropLeftRb.Name = "CropLeftRb";
			this.CropLeftRb.Size = new System.Drawing.Size(43, 17);
			this.CropLeftRb.TabIndex = 7;
			this.CropLeftRb.TabStop = true;
			this.CropLeftRb.Text = "Left";
			this.CropLeftRb.UseVisualStyleBackColor = true;
			// 
			// CroppingGb
			// 
			this.CroppingGb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CroppingGb.Controls.Add(this.CropRightRb);
			this.CroppingGb.Controls.Add(this.CropLeftRb);
			this.CroppingGb.Controls.Add(this.CropCenterRb);
			this.CroppingGb.Location = new System.Drawing.Point(516, 95);
			this.CroppingGb.Name = "CroppingGb";
			this.CroppingGb.Size = new System.Drawing.Size(80, 89);
			this.CroppingGb.TabIndex = 8;
			this.CroppingGb.TabStop = false;
			this.CroppingGb.Text = "Cropping";
			// 
			// ConvertCb
			// 
			this.ConvertCb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ConvertCb.Appearance = System.Windows.Forms.Appearance.Button;
			this.ConvertCb.Location = new System.Drawing.Point(525, 207);
			this.ConvertCb.Name = "ConvertCb";
			this.ConvertCb.Size = new System.Drawing.Size(71, 24);
			this.ConvertCb.TabIndex = 9;
			this.ConvertCb.Text = "Convert";
			this.ConvertCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.ConvertCb.UseVisualStyleBackColor = true;
			this.ConvertCb.CheckedChanged += new System.EventHandler(this.ConvertCb_CheckedChanged);
			// 
			// SendBtn
			// 
			this.SendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SendBtn.Location = new System.Drawing.Point(525, 254);
			this.SendBtn.Name = "SendBtn";
			this.SendBtn.Size = new System.Drawing.Size(71, 23);
			this.SendBtn.TabIndex = 10;
			this.SendBtn.Text = "Send";
			this.SendBtn.UseVisualStyleBackColor = true;
			this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
			// 
			// CancelBtn
			// 
			this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelBtn.Location = new System.Drawing.Point(525, 283);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(71, 23);
			this.CancelBtn.TabIndex = 11;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
			// 
			// RulerPnl
			// 
			this.RulerPnl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.RulerPnl.Location = new System.Drawing.Point(16, 8);
			this.RulerPnl.Name = "RulerPnl";
			this.RulerPnl.Size = new System.Drawing.Size(493, 10);
			this.RulerPnl.TabIndex = 55;
			this.RulerPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.RulerPnl_Paint);
			// 
			// SendFileForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(608, 318);
			this.Controls.Add(this.RulerPnl);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.SendBtn);
			this.Controls.Add(this.ConvertCb);
			this.Controls.Add(this.CroppingGb);
			this.Controls.Add(this.LineLengthTb);
			this.Controls.Add(this.LineLengthLbl);
			this.Controls.Add(this.LoadBtn);
			this.Controls.Add(this.TextTb);
			this.Name = "SendFileForm";
			this.Text = "Send text file";
			this.CroppingGb.ResumeLayout(false);
			this.CroppingGb.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TextTb;
		private System.Windows.Forms.Button LoadBtn;
		private System.Windows.Forms.Label LineLengthLbl;
		private System.Windows.Forms.TextBox LineLengthTb;
		private System.Windows.Forms.RadioButton CropRightRb;
		private System.Windows.Forms.RadioButton CropCenterRb;
		private System.Windows.Forms.RadioButton CropLeftRb;
		private System.Windows.Forms.GroupBox CroppingGb;
		private System.Windows.Forms.CheckBox ConvertCb;
		private System.Windows.Forms.Button SendBtn;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.Panel RulerPnl;
	}
}