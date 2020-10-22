namespace WinTlx
{
	partial class LoadTape
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
			this.FilenameTb = new System.Windows.Forms.TextBox();
			this.SelectBtn = new System.Windows.Forms.Button();
			this.LoadBtn = new System.Windows.Forms.Button();
			this.MirrorCb = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// FilenameTb
			// 
			this.FilenameTb.Location = new System.Drawing.Point(13, 13);
			this.FilenameTb.Name = "FilenameTb";
			this.FilenameTb.Size = new System.Drawing.Size(437, 20);
			this.FilenameTb.TabIndex = 0;
			// 
			// SelectBtn
			// 
			this.SelectBtn.Location = new System.Drawing.Point(456, 10);
			this.SelectBtn.Name = "SelectBtn";
			this.SelectBtn.Size = new System.Drawing.Size(75, 23);
			this.SelectBtn.TabIndex = 1;
			this.SelectBtn.Text = "Select file";
			this.SelectBtn.UseVisualStyleBackColor = true;
			// 
			// LoadBtn
			// 
			this.LoadBtn.Location = new System.Drawing.Point(537, 11);
			this.LoadBtn.Name = "LoadBtn";
			this.LoadBtn.Size = new System.Drawing.Size(75, 23);
			this.LoadBtn.TabIndex = 2;
			this.LoadBtn.Text = "Load";
			this.LoadBtn.UseVisualStyleBackColor = true;
			// 
			// MirrorCb
			// 
			this.MirrorCb.AutoSize = true;
			this.MirrorCb.Location = new System.Drawing.Point(456, 43);
			this.MirrorCb.Name = "MirrorCb";
			this.MirrorCb.Size = new System.Drawing.Size(93, 17);
			this.MirrorCb.TabIndex = 3;
			this.MirrorCb.Text = "Mirror bit code";
			this.MirrorCb.UseVisualStyleBackColor = true;
			// 
			// LoadTape
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 75);
			this.Controls.Add(this.MirrorCb);
			this.Controls.Add(this.LoadBtn);
			this.Controls.Add(this.SelectBtn);
			this.Controls.Add(this.FilenameTb);
			this.Name = "LoadTape";
			this.Text = "Load tape";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox FilenameTb;
		private System.Windows.Forms.Button SelectBtn;
		private System.Windows.Forms.Button LoadBtn;
		private System.Windows.Forms.CheckBox MirrorCb;
	}
}