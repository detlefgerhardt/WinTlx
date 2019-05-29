namespace WinTelex
{
	partial class ConfigForm
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
			this.KennungLbl = new System.Windows.Forms.Label();
			this.KennungTb = new System.Windows.Forms.TextBox();
			this.IncommingPortTb = new System.Windows.Forms.TextBox();
			this.SubscribeServerAddressTb = new System.Windows.Forms.TextBox();
			this.SubscribeServerPortTb = new System.Windows.Forms.TextBox();
			this.IncomingPortLbl = new System.Windows.Forms.Label();
			this.SubscribeServerAddressLbl = new System.Windows.Forms.Label();
			this.SubscribeServerPortLbl = new System.Windows.Forms.Label();
			this.SaveBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// KennungLbl
			// 
			this.KennungLbl.AutoSize = true;
			this.KennungLbl.Location = new System.Drawing.Point(13, 19);
			this.KennungLbl.Name = "KennungLbl";
			this.KennungLbl.Size = new System.Drawing.Size(50, 13);
			this.KennungLbl.TabIndex = 0;
			this.KennungLbl.Text = "Kennung";
			// 
			// KennungTb
			// 
			this.KennungTb.Location = new System.Drawing.Point(145, 16);
			this.KennungTb.Name = "KennungTb";
			this.KennungTb.Size = new System.Drawing.Size(132, 20);
			this.KennungTb.TabIndex = 1;
			// 
			// IncommingPortTb
			// 
			this.IncommingPortTb.Location = new System.Drawing.Point(145, 42);
			this.IncommingPortTb.Name = "IncommingPortTb";
			this.IncommingPortTb.Size = new System.Drawing.Size(132, 20);
			this.IncommingPortTb.TabIndex = 3;
			// 
			// SubscribeServerAddressTb
			// 
			this.SubscribeServerAddressTb.Location = new System.Drawing.Point(145, 68);
			this.SubscribeServerAddressTb.Name = "SubscribeServerAddressTb";
			this.SubscribeServerAddressTb.Size = new System.Drawing.Size(132, 20);
			this.SubscribeServerAddressTb.TabIndex = 5;
			// 
			// SubscribeServerPortTb
			// 
			this.SubscribeServerPortTb.Location = new System.Drawing.Point(145, 94);
			this.SubscribeServerPortTb.Name = "SubscribeServerPortTb";
			this.SubscribeServerPortTb.Size = new System.Drawing.Size(132, 20);
			this.SubscribeServerPortTb.TabIndex = 7;
			// 
			// IncomingPortLbl
			// 
			this.IncomingPortLbl.AutoSize = true;
			this.IncomingPortLbl.Location = new System.Drawing.Point(13, 42);
			this.IncomingPortLbl.Name = "IncomingPortLbl";
			this.IncomingPortLbl.Size = new System.Drawing.Size(71, 13);
			this.IncomingPortLbl.TabIndex = 2;
			this.IncomingPortLbl.Text = "Incoming port";
			// 
			// SubscribeServerAddressLbl
			// 
			this.SubscribeServerAddressLbl.AutoSize = true;
			this.SubscribeServerAddressLbl.Location = new System.Drawing.Point(13, 71);
			this.SubscribeServerAddressLbl.Name = "SubscribeServerAddressLbl";
			this.SubscribeServerAddressLbl.Size = new System.Drawing.Size(126, 13);
			this.SubscribeServerAddressLbl.TabIndex = 4;
			this.SubscribeServerAddressLbl.Text = "Subscribe server address";
			// 
			// SubscribeServerPortLbl
			// 
			this.SubscribeServerPortLbl.AutoSize = true;
			this.SubscribeServerPortLbl.Location = new System.Drawing.Point(13, 97);
			this.SubscribeServerPortLbl.Name = "SubscribeServerPortLbl";
			this.SubscribeServerPortLbl.Size = new System.Drawing.Size(107, 13);
			this.SubscribeServerPortLbl.TabIndex = 6;
			this.SubscribeServerPortLbl.Text = "Subscribe server port";
			// 
			// SaveBtn
			// 
			this.SaveBtn.Location = new System.Drawing.Point(16, 137);
			this.SaveBtn.Name = "SaveBtn";
			this.SaveBtn.Size = new System.Drawing.Size(75, 23);
			this.SaveBtn.TabIndex = 8;
			this.SaveBtn.Text = "Save";
			this.SaveBtn.UseVisualStyleBackColor = true;
			this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
			// 
			// CancelBtn
			// 
			this.CancelBtn.Location = new System.Drawing.Point(202, 137);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 9;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
			// 
			// ConfigForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(304, 172);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.SaveBtn);
			this.Controls.Add(this.SubscribeServerPortLbl);
			this.Controls.Add(this.SubscribeServerAddressLbl);
			this.Controls.Add(this.IncomingPortLbl);
			this.Controls.Add(this.SubscribeServerPortTb);
			this.Controls.Add(this.SubscribeServerAddressTb);
			this.Controls.Add(this.IncommingPortTb);
			this.Controls.Add(this.KennungTb);
			this.Controls.Add(this.KennungLbl);
			this.Name = "ConfigForm";
			this.Text = "WinTelex Configuration";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label KennungLbl;
		private System.Windows.Forms.TextBox KennungTb;
		private System.Windows.Forms.TextBox IncommingPortTb;
		private System.Windows.Forms.TextBox SubscribeServerAddressTb;
		private System.Windows.Forms.TextBox SubscribeServerPortTb;
		private System.Windows.Forms.Label IncomingPortLbl;
		private System.Windows.Forms.Label SubscribeServerAddressLbl;
		private System.Windows.Forms.Label SubscribeServerPortLbl;
		private System.Windows.Forms.Button SaveBtn;
		private System.Windows.Forms.Button CancelBtn;
	}
}