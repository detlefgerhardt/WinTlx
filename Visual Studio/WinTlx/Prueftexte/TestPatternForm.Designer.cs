
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
			this.SelectionLbl = new System.Windows.Forms.Label();
			this.CountLbl = new System.Windows.Forms.Label();
			this.PreViewTb = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// SendBtn
			// 
			this.SendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SendBtn.Location = new System.Drawing.Point(375, 28);
			this.SendBtn.Name = "SendBtn";
			this.SendBtn.Size = new System.Drawing.Size(65, 24);
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
			this.PatternSelectCb.SelectedIndexChanged += new System.EventHandler(this.PatternSelectCb_SelectedIndexChanged);
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
			// SelectionLbl
			// 
			this.SelectionLbl.AutoSize = true;
			this.SelectionLbl.Location = new System.Drawing.Point(12, 11);
			this.SelectionLbl.Name = "SelectionLbl";
			this.SelectionLbl.Size = new System.Drawing.Size(47, 13);
			this.SelectionLbl.TabIndex = 29;
			this.SelectionLbl.Text = "Auswahl";
			// 
			// CountLbl
			// 
			this.CountLbl.AutoSize = true;
			this.CountLbl.Location = new System.Drawing.Point(256, 10);
			this.CountLbl.Name = "CountLbl";
			this.CountLbl.Size = new System.Drawing.Size(39, 13);
			this.CountLbl.TabIndex = 30;
			this.CountLbl.Text = "Anzahl";
			// 
			// PreViewTb
			// 
			this.PreViewTb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PreViewTb.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PreViewTb.Location = new System.Drawing.Point(12, 58);
			this.PreViewTb.Multiline = true;
			this.PreViewTb.Name = "PreViewTb";
			this.PreViewTb.ReadOnly = true;
			this.PreViewTb.Size = new System.Drawing.Size(427, 210);
			this.PreViewTb.TabIndex = 31;
			// 
			// TestPatternForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(449, 280);
			this.Controls.Add(this.PreViewTb);
			this.Controls.Add(this.CountLbl);
			this.Controls.Add(this.SelectionLbl);
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
		private System.Windows.Forms.Label SelectionLbl;
		private System.Windows.Forms.Label CountLbl;
		private System.Windows.Forms.TextBox PreViewTb;
	}
}