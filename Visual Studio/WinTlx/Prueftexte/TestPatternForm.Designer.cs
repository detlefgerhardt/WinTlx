
namespace WinTlx.Prueftexte
{
	partial class TestPatternForm
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
			this.SendBtn = new System.Windows.Forms.Button();
			this.PatternSelectCb = new System.Windows.Forms.ComboBox();
			this.LineCountTb = new System.Windows.Forms.TextBox();
			this.PatternSelectLbl = new System.Windows.Forms.Label();
			this.LineCountLbl = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// SendBtn
			// 
			this.SendBtn.Location = new System.Drawing.Point(306, 28);
			this.SendBtn.Name = "SendBtn";
			this.SendBtn.Size = new System.Drawing.Size(50, 24);
			this.SendBtn.TabIndex = 27;
			this.SendBtn.Text = "Send";
			this.SendBtn.UseVisualStyleBackColor = true;
			this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
			// 
			// PatternSelectCb
			// 
			this.PatternSelectCb.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PatternSelectCb.FormattingEnabled = true;
			this.PatternSelectCb.Location = new System.Drawing.Point(12, 29);
			this.PatternSelectCb.Name = "PatternSelectCb";
			this.PatternSelectCb.Size = new System.Drawing.Size(237, 22);
			this.PatternSelectCb.TabIndex = 26;
			// 
			// LineCountTb
			// 
			this.LineCountTb.Location = new System.Drawing.Point(255, 29);
			this.LineCountTb.Multiline = true;
			this.LineCountTb.Name = "LineCountTb";
			this.LineCountTb.Size = new System.Drawing.Size(45, 22);
			this.LineCountTb.TabIndex = 28;
			this.LineCountTb.Leave += new System.EventHandler(this.LineCountTb_Leave);
			// 
			// PatternSelectLbl
			// 
			this.PatternSelectLbl.AutoSize = true;
			this.PatternSelectLbl.Location = new System.Drawing.Point(12, 11);
			this.PatternSelectLbl.Name = "PatternSelectLbl";
			this.PatternSelectLbl.Size = new System.Drawing.Size(47, 13);
			this.PatternSelectLbl.TabIndex = 29;
			this.PatternSelectLbl.Text = "Auswahl";
			// 
			// LineCountLbl
			// 
			this.LineCountLbl.AutoSize = true;
			this.LineCountLbl.Location = new System.Drawing.Point(256, 10);
			this.LineCountLbl.Name = "LineCountLbl";
			this.LineCountLbl.Size = new System.Drawing.Size(39, 13);
			this.LineCountLbl.TabIndex = 30;
			this.LineCountLbl.Text = "Anzahl";
			// 
			// TestPatternForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(366, 71);
			this.Controls.Add(this.LineCountLbl);
			this.Controls.Add(this.PatternSelectLbl);
			this.Controls.Add(this.LineCountTb);
			this.Controls.Add(this.SendBtn);
			this.Controls.Add(this.PatternSelectCb);
			this.Name = "TestPatternForm";
			this.Text = "Prueftexte";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TestPatternForm_FormClosed);
			this.Load += new System.EventHandler(this.TestPatternForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button SendBtn;
		private System.Windows.Forms.ComboBox PatternSelectCb;
		private System.Windows.Forms.TextBox LineCountTb;
		private System.Windows.Forms.Label PatternSelectLbl;
		private System.Windows.Forms.Label LineCountLbl;
	}
}