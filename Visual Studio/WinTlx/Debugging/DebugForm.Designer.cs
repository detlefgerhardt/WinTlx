
namespace WinTlx.Debugging
{
	partial class DebugForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugForm));
			this.TextRtb = new System.Windows.Forms.RichTextBox();
			this.RecvLbl = new System.Windows.Forms.Label();
			this.SendLbl = new System.Windows.Forms.Label();
			this.MsgLbl = new System.Windows.Forms.Label();
			this.ScrollCb = new System.Windows.Forms.CheckBox();
			this.ShowHeartbeatCb = new System.Windows.Forms.CheckBox();
			this.ShowAckCb = new System.Windows.Forms.CheckBox();
			this.RecvBufTb = new System.Windows.Forms.TextBox();
			this.ConnectionStateTb = new System.Windows.Forms.TextBox();
			this.SendAckTb = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// TextRtb
			// 
			this.TextRtb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextRtb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TextRtb.Location = new System.Drawing.Point(13, 56);
			this.TextRtb.Name = "TextRtb";
			this.TextRtb.ReadOnly = true;
			this.TextRtb.Size = new System.Drawing.Size(561, 377);
			this.TextRtb.TabIndex = 0;
			this.TextRtb.Text = "";
			// 
			// RecvLbl
			// 
			this.RecvLbl.AutoSize = true;
			this.RecvLbl.Location = new System.Drawing.Point(12, 6);
			this.RecvLbl.Name = "RecvLbl";
			this.RecvLbl.Size = new System.Drawing.Size(33, 13);
			this.RecvLbl.TabIndex = 1;
			this.RecvLbl.Text = "Recv";
			// 
			// SendLbl
			// 
			this.SendLbl.AutoSize = true;
			this.SendLbl.ForeColor = System.Drawing.Color.Red;
			this.SendLbl.Location = new System.Drawing.Point(51, 6);
			this.SendLbl.Name = "SendLbl";
			this.SendLbl.Size = new System.Drawing.Size(32, 13);
			this.SendLbl.TabIndex = 2;
			this.SendLbl.Text = "Send";
			// 
			// MsgLbl
			// 
			this.MsgLbl.AutoSize = true;
			this.MsgLbl.ForeColor = System.Drawing.Color.Blue;
			this.MsgLbl.Location = new System.Drawing.Point(89, 6);
			this.MsgLbl.Name = "MsgLbl";
			this.MsgLbl.Size = new System.Drawing.Size(27, 13);
			this.MsgLbl.TabIndex = 3;
			this.MsgLbl.Text = "Msg";
			// 
			// ScrollCb
			// 
			this.ScrollCb.AutoSize = true;
			this.ScrollCb.Location = new System.Drawing.Point(139, 7);
			this.ScrollCb.Name = "ScrollCb";
			this.ScrollCb.Size = new System.Drawing.Size(52, 17);
			this.ScrollCb.TabIndex = 4;
			this.ScrollCb.Text = "Scroll";
			this.ScrollCb.UseVisualStyleBackColor = true;
			// 
			// ShowHeartbeatCb
			// 
			this.ShowHeartbeatCb.AutoSize = true;
			this.ShowHeartbeatCb.Location = new System.Drawing.Point(197, 7);
			this.ShowHeartbeatCb.Name = "ShowHeartbeatCb";
			this.ShowHeartbeatCb.Size = new System.Drawing.Size(103, 17);
			this.ShowHeartbeatCb.TabIndex = 5;
			this.ShowHeartbeatCb.Text = "Show Heartbeat";
			this.ShowHeartbeatCb.UseVisualStyleBackColor = true;
			// 
			// ShowAckCb
			// 
			this.ShowAckCb.AutoSize = true;
			this.ShowAckCb.Location = new System.Drawing.Point(306, 7);
			this.ShowAckCb.Name = "ShowAckCb";
			this.ShowAckCb.Size = new System.Drawing.Size(75, 17);
			this.ShowAckCb.TabIndex = 6;
			this.ShowAckCb.Text = "Show Ack";
			this.ShowAckCb.UseVisualStyleBackColor = true;
			// 
			// RecvBufTb
			// 
			this.RecvBufTb.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RecvBufTb.Location = new System.Drawing.Point(12, 30);
			this.RecvBufTb.Name = "RecvBufTb";
			this.RecvBufTb.ReadOnly = true;
			this.RecvBufTb.Size = new System.Drawing.Size(149, 20);
			this.RecvBufTb.TabIndex = 2;
			this.RecvBufTb.TabStop = false;
			// 
			// ConnectionStateTb
			// 
			this.ConnectionStateTb.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConnectionStateTb.Location = new System.Drawing.Point(322, 30);
			this.ConnectionStateTb.Name = "ConnectionStateTb";
			this.ConnectionStateTb.ReadOnly = true;
			this.ConnectionStateTb.Size = new System.Drawing.Size(100, 20);
			this.ConnectionStateTb.TabIndex = 1;
			this.ConnectionStateTb.TabStop = false;
			// 
			// SendAckTb
			// 
			this.SendAckTb.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendAckTb.Location = new System.Drawing.Point(167, 30);
			this.SendAckTb.Name = "SendAckTb";
			this.SendAckTb.ReadOnly = true;
			this.SendAckTb.Size = new System.Drawing.Size(149, 20);
			this.SendAckTb.TabIndex = 0;
			this.SendAckTb.TabStop = false;
			// 
			// DebugForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(586, 445);
			this.Controls.Add(this.RecvBufTb);
			this.Controls.Add(this.SendAckTb);
			this.Controls.Add(this.ConnectionStateTb);
			this.Controls.Add(this.ShowAckCb);
			this.Controls.Add(this.ShowHeartbeatCb);
			this.Controls.Add(this.ScrollCb);
			this.Controls.Add(this.MsgLbl);
			this.Controls.Add(this.SendLbl);
			this.Controls.Add(this.RecvLbl);
			this.Controls.Add(this.TextRtb);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DebugForm";
			this.Text = "WinTlx Debug Form";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DebugForm_FormClosed);
			this.Load += new System.EventHandler(this.DebugForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox TextRtb;
		private System.Windows.Forms.Label RecvLbl;
		private System.Windows.Forms.Label SendLbl;
		private System.Windows.Forms.Label MsgLbl;
		private System.Windows.Forms.CheckBox ScrollCb;
		private System.Windows.Forms.CheckBox ShowHeartbeatCb;
		private System.Windows.Forms.CheckBox ShowAckCb;
		private System.Windows.Forms.TextBox RecvBufTb;
		private System.Windows.Forms.TextBox ConnectionStateTb;
		private System.Windows.Forms.TextBox SendAckTb;
	}
}