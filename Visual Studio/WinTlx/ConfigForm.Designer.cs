namespace WinTlx
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
			this.KennungLbl = new System.Windows.Forms.Label();
			this.KennungTb = new System.Windows.Forms.TextBox();
			this.IncommingLocalPortTb = new System.Windows.Forms.TextBox();
			this.SubscribeServerAddressTb = new System.Windows.Forms.TextBox();
			this.SubscribeServerPortTb = new System.Windows.Forms.TextBox();
			this.IncomingLocalPortLbl = new System.Windows.Forms.Label();
			this.SubscribeServerAddressLbl = new System.Windows.Forms.Label();
			this.SubscribeServerPortLbl = new System.Windows.Forms.Label();
			this.SaveBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.OwnNumberTb = new System.Windows.Forms.TextBox();
			this.OwnNumberLbl = new System.Windows.Forms.Label();
			this.SubscribeServerUpdatePinTb = new System.Windows.Forms.TextBox();
			this.SubscribeServerUpdatePinLbl = new System.Windows.Forms.Label();
			this.IncomingPublicPortTb = new System.Windows.Forms.TextBox();
			this.IncomingPublicPortLbl = new System.Windows.Forms.Label();
			this.InactivityTimeoutTb = new System.Windows.Forms.TextBox();
			this.InactivityTimeoutLbl = new System.Windows.Forms.Label();
			this.OutputSpeedLbl = new System.Windows.Forms.Label();
			this.OutputSpeedTb = new System.Windows.Forms.TextBox();
			this.IncomingGb = new System.Windows.Forms.GroupBox();
			this.ExtensionNumberLbl = new System.Windows.Forms.Label();
			this.ExtensionNumberTb = new System.Windows.Forms.TextBox();
			this.GeneralGb = new System.Windows.Forms.GroupBox();
			this.IncomingGb.SuspendLayout();
			this.GeneralGb.SuspendLayout();
			this.SuspendLayout();
			// 
			// KennungLbl
			// 
			this.KennungLbl.AutoSize = true;
			this.KennungLbl.Location = new System.Drawing.Point(9, 20);
			this.KennungLbl.Name = "KennungLbl";
			this.KennungLbl.Size = new System.Drawing.Size(50, 13);
			this.KennungLbl.TabIndex = 0;
			this.KennungLbl.Text = "Kennung";
			// 
			// KennungTb
			// 
			this.KennungTb.Location = new System.Drawing.Point(141, 17);
			this.KennungTb.Name = "KennungTb";
			this.KennungTb.Size = new System.Drawing.Size(136, 20);
			this.KennungTb.TabIndex = 1;
			// 
			// IncommingLocalPortTb
			// 
			this.IncommingLocalPortTb.Location = new System.Drawing.Point(141, 97);
			this.IncommingLocalPortTb.Name = "IncommingLocalPortTb";
			this.IncommingLocalPortTb.Size = new System.Drawing.Size(54, 20);
			this.IncommingLocalPortTb.TabIndex = 3;
			// 
			// SubscribeServerAddressTb
			// 
			this.SubscribeServerAddressTb.Location = new System.Drawing.Point(141, 95);
			this.SubscribeServerAddressTb.Name = "SubscribeServerAddressTb";
			this.SubscribeServerAddressTb.Size = new System.Drawing.Size(136, 20);
			this.SubscribeServerAddressTb.TabIndex = 5;
			// 
			// SubscribeServerPortTb
			// 
			this.SubscribeServerPortTb.Location = new System.Drawing.Point(141, 121);
			this.SubscribeServerPortTb.Name = "SubscribeServerPortTb";
			this.SubscribeServerPortTb.Size = new System.Drawing.Size(54, 20);
			this.SubscribeServerPortTb.TabIndex = 7;
			// 
			// IncomingLocalPortLbl
			// 
			this.IncomingLocalPortLbl.AutoSize = true;
			this.IncomingLocalPortLbl.Location = new System.Drawing.Point(9, 100);
			this.IncomingLocalPortLbl.Name = "IncomingLocalPortLbl";
			this.IncomingLocalPortLbl.Size = new System.Drawing.Size(96, 13);
			this.IncomingLocalPortLbl.TabIndex = 2;
			this.IncomingLocalPortLbl.Text = "Incoming local port";
			// 
			// SubscribeServerAddressLbl
			// 
			this.SubscribeServerAddressLbl.AutoSize = true;
			this.SubscribeServerAddressLbl.Location = new System.Drawing.Point(9, 98);
			this.SubscribeServerAddressLbl.Name = "SubscribeServerAddressLbl";
			this.SubscribeServerAddressLbl.Size = new System.Drawing.Size(126, 13);
			this.SubscribeServerAddressLbl.TabIndex = 4;
			this.SubscribeServerAddressLbl.Text = "Subscribe server address";
			// 
			// SubscribeServerPortLbl
			// 
			this.SubscribeServerPortLbl.AutoSize = true;
			this.SubscribeServerPortLbl.Location = new System.Drawing.Point(9, 124);
			this.SubscribeServerPortLbl.Name = "SubscribeServerPortLbl";
			this.SubscribeServerPortLbl.Size = new System.Drawing.Size(107, 13);
			this.SubscribeServerPortLbl.TabIndex = 6;
			this.SubscribeServerPortLbl.Text = "Subscribe server port";
			// 
			// SaveBtn
			// 
			this.SaveBtn.Location = new System.Drawing.Point(19, 342);
			this.SaveBtn.Name = "SaveBtn";
			this.SaveBtn.Size = new System.Drawing.Size(75, 23);
			this.SaveBtn.TabIndex = 8;
			this.SaveBtn.Text = "Save";
			this.SaveBtn.UseVisualStyleBackColor = true;
			this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
			// 
			// CancelBtn
			// 
			this.CancelBtn.Location = new System.Drawing.Point(205, 342);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 9;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
			// 
			// OwnNumberTb
			// 
			this.OwnNumberTb.Location = new System.Drawing.Point(141, 45);
			this.OwnNumberTb.Name = "OwnNumberTb";
			this.OwnNumberTb.Size = new System.Drawing.Size(136, 20);
			this.OwnNumberTb.TabIndex = 10;
			// 
			// OwnNumberLbl
			// 
			this.OwnNumberLbl.AutoSize = true;
			this.OwnNumberLbl.Location = new System.Drawing.Point(9, 48);
			this.OwnNumberLbl.Name = "OwnNumberLbl";
			this.OwnNumberLbl.Size = new System.Drawing.Size(67, 13);
			this.OwnNumberLbl.TabIndex = 11;
			this.OwnNumberLbl.Text = "Own number";
			// 
			// SubscribeServerUpdatePinTb
			// 
			this.SubscribeServerUpdatePinTb.Location = new System.Drawing.Point(141, 19);
			this.SubscribeServerUpdatePinTb.Name = "SubscribeServerUpdatePinTb";
			this.SubscribeServerUpdatePinTb.Size = new System.Drawing.Size(54, 20);
			this.SubscribeServerUpdatePinTb.TabIndex = 12;
			// 
			// SubscribeServerUpdatePinLbl
			// 
			this.SubscribeServerUpdatePinLbl.AutoSize = true;
			this.SubscribeServerUpdatePinLbl.Location = new System.Drawing.Point(9, 22);
			this.SubscribeServerUpdatePinLbl.Name = "SubscribeServerUpdatePinLbl";
			this.SubscribeServerUpdatePinLbl.Size = new System.Drawing.Size(103, 13);
			this.SubscribeServerUpdatePinLbl.TabIndex = 13;
			this.SubscribeServerUpdatePinLbl.Text = "Subscribe server pin";
			// 
			// IncomingPublicPortTb
			// 
			this.IncomingPublicPortTb.Location = new System.Drawing.Point(141, 123);
			this.IncomingPublicPortTb.Name = "IncomingPublicPortTb";
			this.IncomingPublicPortTb.Size = new System.Drawing.Size(54, 20);
			this.IncomingPublicPortTb.TabIndex = 14;
			// 
			// IncomingPublicPortLbl
			// 
			this.IncomingPublicPortLbl.AutoSize = true;
			this.IncomingPublicPortLbl.Location = new System.Drawing.Point(10, 126);
			this.IncomingPublicPortLbl.Name = "IncomingPublicPortLbl";
			this.IncomingPublicPortLbl.Size = new System.Drawing.Size(102, 13);
			this.IncomingPublicPortLbl.TabIndex = 15;
			this.IncomingPublicPortLbl.Text = "Incoming public port";
			// 
			// InactivityTimeoutTb
			// 
			this.InactivityTimeoutTb.Location = new System.Drawing.Point(141, 43);
			this.InactivityTimeoutTb.Name = "InactivityTimeoutTb";
			this.InactivityTimeoutTb.Size = new System.Drawing.Size(54, 20);
			this.InactivityTimeoutTb.TabIndex = 16;
			// 
			// InactivityTimeoutLbl
			// 
			this.InactivityTimeoutLbl.AutoSize = true;
			this.InactivityTimeoutLbl.Location = new System.Drawing.Point(9, 46);
			this.InactivityTimeoutLbl.Name = "InactivityTimeoutLbl";
			this.InactivityTimeoutLbl.Size = new System.Drawing.Size(112, 13);
			this.InactivityTimeoutLbl.TabIndex = 17;
			this.InactivityTimeoutLbl.Text = "Inactivity timeout (sec)";
			// 
			// OutputSpeedLbl
			// 
			this.OutputSpeedLbl.AutoSize = true;
			this.OutputSpeedLbl.Location = new System.Drawing.Point(9, 72);
			this.OutputSpeedLbl.Name = "OutputSpeedLbl";
			this.OutputSpeedLbl.Size = new System.Drawing.Size(105, 13);
			this.OutputSpeedLbl.TabIndex = 18;
			this.OutputSpeedLbl.Text = "Output speed (Baud)";
			// 
			// OutputSpeedTb
			// 
			this.OutputSpeedTb.Location = new System.Drawing.Point(141, 69);
			this.OutputSpeedTb.Name = "OutputSpeedTb";
			this.OutputSpeedTb.Size = new System.Drawing.Size(54, 20);
			this.OutputSpeedTb.TabIndex = 19;
			// 
			// IncomingGb
			// 
			this.IncomingGb.Controls.Add(this.ExtensionNumberLbl);
			this.IncomingGb.Controls.Add(this.ExtensionNumberTb);
			this.IncomingGb.Controls.Add(this.SubscribeServerUpdatePinLbl);
			this.IncomingGb.Controls.Add(this.IncommingLocalPortTb);
			this.IncomingGb.Controls.Add(this.IncomingLocalPortLbl);
			this.IncomingGb.Controls.Add(this.OwnNumberTb);
			this.IncomingGb.Controls.Add(this.OwnNumberLbl);
			this.IncomingGb.Controls.Add(this.IncomingPublicPortLbl);
			this.IncomingGb.Controls.Add(this.SubscribeServerUpdatePinTb);
			this.IncomingGb.Controls.Add(this.IncomingPublicPortTb);
			this.IncomingGb.Location = new System.Drawing.Point(12, 172);
			this.IncomingGb.Name = "IncomingGb";
			this.IncomingGb.Size = new System.Drawing.Size(283, 152);
			this.IncomingGb.TabIndex = 20;
			this.IncomingGb.TabStop = false;
			this.IncomingGb.Text = "Incoming connection";
			// 
			// ExtensionNumberLbl
			// 
			this.ExtensionNumberLbl.AutoSize = true;
			this.ExtensionNumberLbl.Location = new System.Drawing.Point(9, 74);
			this.ExtensionNumberLbl.Name = "ExtensionNumberLbl";
			this.ExtensionNumberLbl.Size = new System.Drawing.Size(91, 13);
			this.ExtensionNumberLbl.TabIndex = 17;
			this.ExtensionNumberLbl.Text = "Extension number";
			// 
			// ExtensionNumberTb
			// 
			this.ExtensionNumberTb.Location = new System.Drawing.Point(141, 71);
			this.ExtensionNumberTb.Name = "ExtensionNumberTb";
			this.ExtensionNumberTb.Size = new System.Drawing.Size(54, 20);
			this.ExtensionNumberTb.TabIndex = 16;
			// 
			// GeneralGb
			// 
			this.GeneralGb.Controls.Add(this.OutputSpeedTb);
			this.GeneralGb.Controls.Add(this.SubscribeServerAddressTb);
			this.GeneralGb.Controls.Add(this.InactivityTimeoutLbl);
			this.GeneralGb.Controls.Add(this.SubscribeServerPortTb);
			this.GeneralGb.Controls.Add(this.InactivityTimeoutTb);
			this.GeneralGb.Controls.Add(this.OutputSpeedLbl);
			this.GeneralGb.Controls.Add(this.SubscribeServerAddressLbl);
			this.GeneralGb.Controls.Add(this.SubscribeServerPortLbl);
			this.GeneralGb.Controls.Add(this.KennungTb);
			this.GeneralGb.Controls.Add(this.KennungLbl);
			this.GeneralGb.Location = new System.Drawing.Point(12, 12);
			this.GeneralGb.Name = "GeneralGb";
			this.GeneralGb.Size = new System.Drawing.Size(283, 154);
			this.GeneralGb.TabIndex = 21;
			this.GeneralGb.TabStop = false;
			this.GeneralGb.Text = "General";
			// 
			// ConfigForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(307, 384);
			this.Controls.Add(this.GeneralGb);
			this.Controls.Add(this.IncomingGb);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.SaveBtn);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ConfigForm";
			this.Text = "WinTlx Configuration";
			this.Load += new System.EventHandler(this.ConfigForm_Load);
			this.IncomingGb.ResumeLayout(false);
			this.IncomingGb.PerformLayout();
			this.GeneralGb.ResumeLayout(false);
			this.GeneralGb.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label KennungLbl;
		private System.Windows.Forms.TextBox KennungTb;
		private System.Windows.Forms.TextBox IncommingLocalPortTb;
		private System.Windows.Forms.TextBox SubscribeServerAddressTb;
		private System.Windows.Forms.TextBox SubscribeServerPortTb;
		private System.Windows.Forms.Label IncomingLocalPortLbl;
		private System.Windows.Forms.Label SubscribeServerAddressLbl;
		private System.Windows.Forms.Label SubscribeServerPortLbl;
		private System.Windows.Forms.Button SaveBtn;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.TextBox OwnNumberTb;
		private System.Windows.Forms.Label OwnNumberLbl;
		private System.Windows.Forms.TextBox SubscribeServerUpdatePinTb;
		private System.Windows.Forms.Label SubscribeServerUpdatePinLbl;
		private System.Windows.Forms.TextBox IncomingPublicPortTb;
		private System.Windows.Forms.Label IncomingPublicPortLbl;
		private System.Windows.Forms.TextBox InactivityTimeoutTb;
		private System.Windows.Forms.Label InactivityTimeoutLbl;
		private System.Windows.Forms.Label OutputSpeedLbl;
		private System.Windows.Forms.TextBox OutputSpeedTb;
		private System.Windows.Forms.GroupBox IncomingGb;
		private System.Windows.Forms.GroupBox GeneralGb;
		private System.Windows.Forms.TextBox ExtensionNumberTb;
		private System.Windows.Forms.Label ExtensionNumberLbl;
	}
}