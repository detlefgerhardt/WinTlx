using WinTlx.Controls;

namespace WinTlx
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.ConnectBtn = new System.Windows.Forms.Button();
			this.DisconnectBtn = new System.Windows.Forms.Button();
			this.SendHereIsBtn = new System.Windows.Forms.Button();
			this.SendWruBtn = new System.Windows.Forms.Button();
			this.SendBellBtn = new System.Windows.Forms.Button();
			this.SendLettersBtn = new System.Windows.Forms.Button();
			this.SendFiguresBtn = new System.Windows.Forms.Button();
			this.TlnNameCb = new System.Windows.Forms.ComboBox();
			this.TlnPortTb = new System.Windows.Forms.TextBox();
			this.TlnAddressTb = new System.Windows.Forms.TextBox();
			this.TlnMemberLbl = new System.Windows.Forms.Label();
			this.TlnAddressLbl = new System.Windows.Forms.Label();
			this.TlnPortLbl = new System.Windows.Forms.Label();
			this.SendCarriageReturnBtn = new System.Windows.Forms.Button();
			this.SendLineFeedBtn = new System.Windows.Forms.Button();
			this.SearchLbl = new System.Windows.Forms.Label();
			this.QueryBtn = new System.Windows.Forms.Button();
			this.TlnExtensionLbl = new System.Windows.Forms.Label();
			this.TlnExtensionTb = new System.Windows.Forms.TextBox();
			this.SendHBtn = new System.Windows.Forms.Button();
			this.SendGBtn = new System.Windows.Forms.Button();
			this.SendFBtn = new System.Windows.Forms.Button();
			this.SendNullBtn = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.TlnTypeCb = new System.Windows.Forms.ComboBox();
			this.SearchCb = new System.Windows.Forms.ComboBox();
			this.TlnPeerTypeLbl = new System.Windows.Forms.Label();
			this.AnswerbackLbl = new System.Windows.Forms.Label();
			this.AnswerbackTb = new System.Windows.Forms.TextBox();
			this.panel4 = new System.Windows.Forms.Panel();
			this.DateTb = new System.Windows.Forms.TextBox();
			this.TimeTb = new System.Windows.Forms.TextBox();
			this.LinealPnl = new System.Windows.Forms.Panel();
			this.TerminalPb = new WinTlx.Controls.SelectablePictureBox();
			this.ScrollStartBtn = new System.Windows.Forms.Button();
			this.ScrollUpBtn = new System.Windows.Forms.Button();
			this.ScrollEndBtn = new System.Windows.Forms.Button();
			this.ScrollDownBtn = new System.Windows.Forms.Button();
			this.LineLbl = new System.Windows.Forms.Label();
			this.ColumnLbl = new System.Windows.Forms.Label();
			this.ConnTimeLbl = new System.Windows.Forms.Label();
			this.TimeoutLbl = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.ConnectStatusLbl = new System.Windows.Forms.Label();
			this.ReceiveStatusLbl = new System.Windows.Forms.Label();
			this.SendBufferStatusLbl = new System.Windows.Forms.Label();
			this.CharSetLbl = new System.Windows.Forms.Label();
			this.RemoteBufferStatusLbl = new System.Windows.Forms.Label();
			this.LocalBufferStatusLbl = new System.Windows.Forms.Label();
			this.panel2.SuspendLayout();
			this.panel4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.TerminalPb)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ConnectBtn
			// 
			this.ConnectBtn.Location = new System.Drawing.Point(8, 7);
			this.ConnectBtn.Name = "ConnectBtn";
			this.ConnectBtn.Size = new System.Drawing.Size(69, 23);
			this.ConnectBtn.TabIndex = 9;
			this.ConnectBtn.Text = "Connect";
			this.ConnectBtn.UseVisualStyleBackColor = true;
			this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
			// 
			// DisconnectBtn
			// 
			this.DisconnectBtn.Location = new System.Drawing.Point(8, 36);
			this.DisconnectBtn.Name = "DisconnectBtn";
			this.DisconnectBtn.Size = new System.Drawing.Size(69, 23);
			this.DisconnectBtn.TabIndex = 10;
			this.DisconnectBtn.Text = "Disconnect";
			this.DisconnectBtn.UseVisualStyleBackColor = true;
			this.DisconnectBtn.Click += new System.EventHandler(this.DisconnectBtn_Click);
			// 
			// SendHereIsBtn
			// 
			this.SendHereIsBtn.Location = new System.Drawing.Point(688, 166);
			this.SendHereIsBtn.Name = "SendHereIsBtn";
			this.SendHereIsBtn.Size = new System.Drawing.Size(97, 23);
			this.SendHereIsBtn.TabIndex = 13;
			this.SendHereIsBtn.Text = "Here is";
			this.SendHereIsBtn.UseVisualStyleBackColor = true;
			this.SendHereIsBtn.Click += new System.EventHandler(this.SendHereIsBtn_Click);
			// 
			// SendWruBtn
			// 
			this.SendWruBtn.Location = new System.Drawing.Point(688, 137);
			this.SendWruBtn.Name = "SendWruBtn";
			this.SendWruBtn.Size = new System.Drawing.Size(97, 23);
			this.SendWruBtn.TabIndex = 12;
			this.SendWruBtn.Text = "WRU";
			this.SendWruBtn.UseVisualStyleBackColor = true;
			this.SendWruBtn.Click += new System.EventHandler(this.SendWruBtn_Click);
			// 
			// SendBellBtn
			// 
			this.SendBellBtn.Location = new System.Drawing.Point(688, 358);
			this.SendBellBtn.Name = "SendBellBtn";
			this.SendBellBtn.Size = new System.Drawing.Size(97, 23);
			this.SendBellBtn.TabIndex = 18;
			this.SendBellBtn.Text = "Bell";
			this.SendBellBtn.UseVisualStyleBackColor = true;
			this.SendBellBtn.Click += new System.EventHandler(this.SendBellBtn_Click);
			// 
			// SendLettersBtn
			// 
			this.SendLettersBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendLettersBtn.Location = new System.Drawing.Point(688, 201);
			this.SendLettersBtn.Name = "SendLettersBtn";
			this.SendLettersBtn.Size = new System.Drawing.Size(97, 23);
			this.SendLettersBtn.TabIndex = 14;
			this.SendLettersBtn.Text = "A...";
			this.SendLettersBtn.UseVisualStyleBackColor = true;
			this.SendLettersBtn.Click += new System.EventHandler(this.SendLettersBtn_Click);
			// 
			// SendFiguresBtn
			// 
			this.SendFiguresBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendFiguresBtn.Location = new System.Drawing.Point(688, 230);
			this.SendFiguresBtn.Name = "SendFiguresBtn";
			this.SendFiguresBtn.Size = new System.Drawing.Size(97, 23);
			this.SendFiguresBtn.TabIndex = 15;
			this.SendFiguresBtn.Text = "1...";
			this.SendFiguresBtn.UseVisualStyleBackColor = true;
			this.SendFiguresBtn.Click += new System.EventHandler(this.SendFiguresBtn_Click);
			// 
			// TlnNameCb
			// 
			this.TlnNameCb.DropDownWidth = 200;
			this.TlnNameCb.FormattingEnabled = true;
			this.TlnNameCb.Location = new System.Drawing.Point(212, 23);
			this.TlnNameCb.Name = "TlnNameCb";
			this.TlnNameCb.Size = new System.Drawing.Size(230, 21);
			this.TlnNameCb.TabIndex = 3;
			this.TlnNameCb.SelectedIndexChanged += new System.EventHandler(this.TlnNameCb_SelectedIndexChanged);
			// 
			// TlnPortTb
			// 
			this.TlnPortTb.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TlnPortTb.Location = new System.Drawing.Point(396, 65);
			this.TlnPortTb.Name = "TlnPortTb";
			this.TlnPortTb.Size = new System.Drawing.Size(46, 22);
			this.TlnPortTb.TabIndex = 5;
			this.TlnPortTb.Leave += new System.EventHandler(this.PortTb_Leave);
			// 
			// TlnAddressTb
			// 
			this.TlnAddressTb.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TlnAddressTb.Location = new System.Drawing.Point(212, 65);
			this.TlnAddressTb.Name = "TlnAddressTb";
			this.TlnAddressTb.Size = new System.Drawing.Size(178, 22);
			this.TlnAddressTb.TabIndex = 4;
			this.TlnAddressTb.Leave += new System.EventHandler(this.AddressTb_Leave);
			// 
			// TlnMemberLbl
			// 
			this.TlnMemberLbl.AutoSize = true;
			this.TlnMemberLbl.Location = new System.Drawing.Point(210, 7);
			this.TlnMemberLbl.Name = "TlnMemberLbl";
			this.TlnMemberLbl.Size = new System.Drawing.Size(146, 13);
			this.TlnMemberLbl.TabIndex = 0;
			this.TlnMemberLbl.Text = "Search result (select member)";
			// 
			// TlnAddressLbl
			// 
			this.TlnAddressLbl.AutoSize = true;
			this.TlnAddressLbl.Location = new System.Drawing.Point(210, 49);
			this.TlnAddressLbl.Name = "TlnAddressLbl";
			this.TlnAddressLbl.Size = new System.Drawing.Size(45, 13);
			this.TlnAddressLbl.TabIndex = 0;
			this.TlnAddressLbl.Text = "Address";
			// 
			// TlnPortLbl
			// 
			this.TlnPortLbl.AutoSize = true;
			this.TlnPortLbl.Location = new System.Drawing.Point(393, 49);
			this.TlnPortLbl.Name = "TlnPortLbl";
			this.TlnPortLbl.Size = new System.Drawing.Size(26, 13);
			this.TlnPortLbl.TabIndex = 0;
			this.TlnPortLbl.Text = "Port";
			// 
			// SendCarriageReturnBtn
			// 
			this.SendCarriageReturnBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendCarriageReturnBtn.Location = new System.Drawing.Point(688, 294);
			this.SendCarriageReturnBtn.Name = "SendCarriageReturnBtn";
			this.SendCarriageReturnBtn.Size = new System.Drawing.Size(97, 23);
			this.SendCarriageReturnBtn.TabIndex = 16;
			this.SendCarriageReturnBtn.Text = "<";
			this.SendCarriageReturnBtn.UseVisualStyleBackColor = true;
			this.SendCarriageReturnBtn.Click += new System.EventHandler(this.SendCarriageReturnBtn_Click);
			// 
			// SendLineFeedBtn
			// 
			this.SendLineFeedBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendLineFeedBtn.Location = new System.Drawing.Point(688, 323);
			this.SendLineFeedBtn.Name = "SendLineFeedBtn";
			this.SendLineFeedBtn.Size = new System.Drawing.Size(97, 23);
			this.SendLineFeedBtn.TabIndex = 17;
			this.SendLineFeedBtn.Text = "=";
			this.SendLineFeedBtn.UseVisualStyleBackColor = true;
			this.SendLineFeedBtn.Click += new System.EventHandler(this.SendLineFeedBtn_Click);
			// 
			// SearchLbl
			// 
			this.SearchLbl.AutoSize = true;
			this.SearchLbl.Location = new System.Drawing.Point(6, 8);
			this.SearchLbl.Name = "SearchLbl";
			this.SearchLbl.Size = new System.Drawing.Size(61, 13);
			this.SearchLbl.TabIndex = 0;
			this.SearchLbl.Text = "Search text";
			// 
			// QueryBtn
			// 
			this.QueryBtn.Location = new System.Drawing.Point(154, 22);
			this.QueryBtn.Name = "QueryBtn";
			this.QueryBtn.Size = new System.Drawing.Size(52, 23);
			this.QueryBtn.TabIndex = 2;
			this.QueryBtn.Text = "Search";
			this.QueryBtn.UseVisualStyleBackColor = true;
			this.QueryBtn.Click += new System.EventHandler(this.QueryBtn_Click);
			// 
			// TlnExtensionLbl
			// 
			this.TlnExtensionLbl.AutoSize = true;
			this.TlnExtensionLbl.Location = new System.Drawing.Point(445, 49);
			this.TlnExtensionLbl.Name = "TlnExtensionLbl";
			this.TlnExtensionLbl.Size = new System.Drawing.Size(53, 13);
			this.TlnExtensionLbl.TabIndex = 0;
			this.TlnExtensionLbl.Text = "Extension";
			// 
			// TlnExtensionTb
			// 
			this.TlnExtensionTb.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TlnExtensionTb.Location = new System.Drawing.Point(448, 65);
			this.TlnExtensionTb.Name = "TlnExtensionTb";
			this.TlnExtensionTb.Size = new System.Drawing.Size(46, 22);
			this.TlnExtensionTb.TabIndex = 6;
			// 
			// SendHBtn
			// 
			this.SendHBtn.Location = new System.Drawing.Point(755, 387);
			this.SendHBtn.Name = "SendHBtn";
			this.SendHBtn.Size = new System.Drawing.Size(30, 23);
			this.SendHBtn.TabIndex = 22;
			this.SendHBtn.Text = "^H";
			this.SendHBtn.UseVisualStyleBackColor = true;
			this.SendHBtn.Click += new System.EventHandler(this.SendHBtn_Click);
			// 
			// SendGBtn
			// 
			this.SendGBtn.Location = new System.Drawing.Point(721, 387);
			this.SendGBtn.Name = "SendGBtn";
			this.SendGBtn.Size = new System.Drawing.Size(30, 23);
			this.SendGBtn.TabIndex = 21;
			this.SendGBtn.Text = "^G";
			this.SendGBtn.UseVisualStyleBackColor = true;
			this.SendGBtn.Click += new System.EventHandler(this.SendGBtn_Click);
			// 
			// SendFBtn
			// 
			this.SendFBtn.Location = new System.Drawing.Point(688, 387);
			this.SendFBtn.Name = "SendFBtn";
			this.SendFBtn.Size = new System.Drawing.Size(30, 23);
			this.SendFBtn.TabIndex = 20;
			this.SendFBtn.Text = "^F";
			this.SendFBtn.UseVisualStyleBackColor = true;
			this.SendFBtn.Click += new System.EventHandler(this.SendFBtn_Click);
			// 
			// SendNullBtn
			// 
			this.SendNullBtn.Location = new System.Drawing.Point(688, 259);
			this.SendNullBtn.Name = "SendNullBtn";
			this.SendNullBtn.Size = new System.Drawing.Size(97, 23);
			this.SendNullBtn.TabIndex = 19;
			this.SendNullBtn.Text = "NUL";
			this.SendNullBtn.UseVisualStyleBackColor = true;
			this.SendNullBtn.Click += new System.EventHandler(this.SendNullBtn_Click);
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.TlnTypeCb);
			this.panel2.Controls.Add(this.SearchCb);
			this.panel2.Controls.Add(this.TlnPeerTypeLbl);
			this.panel2.Controls.Add(this.AnswerbackLbl);
			this.panel2.Controls.Add(this.AnswerbackTb);
			this.panel2.Controls.Add(this.TlnNameCb);
			this.panel2.Controls.Add(this.TlnPortTb);
			this.panel2.Controls.Add(this.TlnExtensionTb);
			this.panel2.Controls.Add(this.TlnAddressTb);
			this.panel2.Controls.Add(this.TlnExtensionLbl);
			this.panel2.Controls.Add(this.TlnMemberLbl);
			this.panel2.Controls.Add(this.QueryBtn);
			this.panel2.Controls.Add(this.TlnAddressLbl);
			this.panel2.Controls.Add(this.TlnPortLbl);
			this.panel2.Controls.Add(this.SearchLbl);
			this.panel2.Location = new System.Drawing.Point(12, 26);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(567, 100);
			this.panel2.TabIndex = 0;
			// 
			// TlnTypeCb
			// 
			this.TlnTypeCb.FormattingEnabled = true;
			this.TlnTypeCb.Location = new System.Drawing.Point(448, 23);
			this.TlnTypeCb.Name = "TlnTypeCb";
			this.TlnTypeCb.Size = new System.Drawing.Size(112, 21);
			this.TlnTypeCb.TabIndex = 82;
			this.TlnTypeCb.MouseHover += new System.EventHandler(this.TlnTypeCb_MouseHover);
			// 
			// SearchCb
			// 
			this.SearchCb.FormattingEnabled = true;
			this.SearchCb.Location = new System.Drawing.Point(8, 23);
			this.SearchCb.Name = "SearchCb";
			this.SearchCb.Size = new System.Drawing.Size(140, 21);
			this.SearchCb.TabIndex = 81;
			this.SearchCb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SearchCb_KeyPress);
			// 
			// TlnPeerTypeLbl
			// 
			this.TlnPeerTypeLbl.AutoSize = true;
			this.TlnPeerTypeLbl.Location = new System.Drawing.Point(445, 7);
			this.TlnPeerTypeLbl.Name = "TlnPeerTypeLbl";
			this.TlnPeerTypeLbl.Size = new System.Drawing.Size(56, 13);
			this.TlnPeerTypeLbl.TabIndex = 10;
			this.TlnPeerTypeLbl.Text = "Peer Type";
			// 
			// AnswerbackLbl
			// 
			this.AnswerbackLbl.AutoSize = true;
			this.AnswerbackLbl.Location = new System.Drawing.Point(7, 49);
			this.AnswerbackLbl.Name = "AnswerbackLbl";
			this.AnswerbackLbl.Size = new System.Drawing.Size(66, 13);
			this.AnswerbackLbl.TabIndex = 0;
			this.AnswerbackLbl.Text = "Answerback";
			// 
			// AnswerbackTb
			// 
			this.AnswerbackTb.Font = new System.Drawing.Font("Consolas", 9F);
			this.AnswerbackTb.Location = new System.Drawing.Point(8, 65);
			this.AnswerbackTb.Name = "AnswerbackTb";
			this.AnswerbackTb.ReadOnly = true;
			this.AnswerbackTb.Size = new System.Drawing.Size(198, 22);
			this.AnswerbackTb.TabIndex = 0;
			this.AnswerbackTb.TabStop = false;
			this.AnswerbackTb.WordWrap = false;
			// 
			// panel4
			// 
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Controls.Add(this.ConnectBtn);
			this.panel4.Controls.Add(this.DisconnectBtn);
			this.panel4.Location = new System.Drawing.Point(585, 26);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(89, 100);
			this.panel4.TabIndex = 0;
			// 
			// DateTb
			// 
			this.DateTb.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DateTb.Location = new System.Drawing.Point(6, 37);
			this.DateTb.Name = "DateTb";
			this.DateTb.ReadOnly = true;
			this.DateTb.Size = new System.Drawing.Size(97, 25);
			this.DateTb.TabIndex = 47;
			this.DateTb.TabStop = false;
			this.DateTb.Text = "00.00.0000";
			this.DateTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TimeTb
			// 
			this.TimeTb.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TimeTb.Location = new System.Drawing.Point(6, 6);
			this.TimeTb.Name = "TimeTb";
			this.TimeTb.ReadOnly = true;
			this.TimeTb.Size = new System.Drawing.Size(97, 25);
			this.TimeTb.TabIndex = 45;
			this.TimeTb.TabStop = false;
			this.TimeTb.Text = "00:00:00";
			this.TimeTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// LinealPnl
			// 
			this.LinealPnl.Location = new System.Drawing.Point(13, 137);
			this.LinealPnl.Name = "LinealPnl";
			this.LinealPnl.Size = new System.Drawing.Size(633, 12);
			this.LinealPnl.TabIndex = 0;
			this.LinealPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.LinealPnl_Paint);
			// 
			// TerminalPb
			// 
			this.TerminalPb.Location = new System.Drawing.Point(12, 155);
			this.TerminalPb.Name = "TerminalPb";
			this.TerminalPb.Size = new System.Drawing.Size(634, 346);
			this.TerminalPb.TabIndex = 57;
			this.TerminalPb.Paint += new System.Windows.Forms.PaintEventHandler(this.TerminalPb_Paint);
			this.TerminalPb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TerminalPb_MouseDown);
			// 
			// ScrollStartBtn
			// 
			this.ScrollStartBtn.Location = new System.Drawing.Point(652, 154);
			this.ScrollStartBtn.Name = "ScrollStartBtn";
			this.ScrollStartBtn.Size = new System.Drawing.Size(22, 23);
			this.ScrollStartBtn.TabIndex = 58;
			this.ScrollStartBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.ScrollStartBtn.UseVisualStyleBackColor = true;
			this.ScrollStartBtn.Click += new System.EventHandler(this.ScrollStartBtn_Click);
			// 
			// ScrollUpBtn
			// 
			this.ScrollUpBtn.Location = new System.Drawing.Point(652, 183);
			this.ScrollUpBtn.Name = "ScrollUpBtn";
			this.ScrollUpBtn.Size = new System.Drawing.Size(22, 23);
			this.ScrollUpBtn.TabIndex = 59;
			this.ScrollUpBtn.Text = "^";
			this.ScrollUpBtn.UseVisualStyleBackColor = true;
			this.ScrollUpBtn.Click += new System.EventHandler(this.ScrollUpBtn_Click);
			// 
			// ScrollEndBtn
			// 
			this.ScrollEndBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ScrollEndBtn.Location = new System.Drawing.Point(652, 478);
			this.ScrollEndBtn.Name = "ScrollEndBtn";
			this.ScrollEndBtn.Size = new System.Drawing.Size(22, 23);
			this.ScrollEndBtn.TabIndex = 60;
			this.ScrollEndBtn.Text = "v";
			this.ScrollEndBtn.UseVisualStyleBackColor = true;
			this.ScrollEndBtn.Click += new System.EventHandler(this.ScrollEndBtn_Click);
			// 
			// ScrollDownBtn
			// 
			this.ScrollDownBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ScrollDownBtn.Location = new System.Drawing.Point(652, 449);
			this.ScrollDownBtn.Name = "ScrollDownBtn";
			this.ScrollDownBtn.Size = new System.Drawing.Size(22, 23);
			this.ScrollDownBtn.TabIndex = 61;
			this.ScrollDownBtn.Text = "v";
			this.ScrollDownBtn.UseVisualStyleBackColor = true;
			this.ScrollDownBtn.Click += new System.EventHandler(this.ScrollDownBtn_Click);
			// 
			// LineLbl
			// 
			this.LineLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.LineLbl.AutoSize = true;
			this.LineLbl.Location = new System.Drawing.Point(12, 506);
			this.LineLbl.Name = "LineLbl";
			this.LineLbl.Size = new System.Drawing.Size(33, 13);
			this.LineLbl.TabIndex = 69;
			this.LineLbl.Text = "Zeile:";
			// 
			// ColumnLbl
			// 
			this.ColumnLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ColumnLbl.AutoSize = true;
			this.ColumnLbl.Location = new System.Drawing.Point(72, 506);
			this.ColumnLbl.Name = "ColumnLbl";
			this.ColumnLbl.Size = new System.Drawing.Size(40, 13);
			this.ColumnLbl.TabIndex = 71;
			this.ColumnLbl.Text = "Spalte:";
			// 
			// ConnTimeLbl
			// 
			this.ConnTimeLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ConnTimeLbl.AutoSize = true;
			this.ConnTimeLbl.Location = new System.Drawing.Point(372, 506);
			this.ConnTimeLbl.Name = "ConnTimeLbl";
			this.ConnTimeLbl.Size = new System.Drawing.Size(61, 13);
			this.ConnTimeLbl.TabIndex = 73;
			this.ConnTimeLbl.Text = "Conn.Time:";
			// 
			// TimeoutLbl
			// 
			this.TimeoutLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TimeoutLbl.AutoSize = true;
			this.TimeoutLbl.Location = new System.Drawing.Point(474, 506);
			this.TimeoutLbl.Name = "TimeoutLbl";
			this.TimeoutLbl.Size = new System.Drawing.Size(48, 13);
			this.TimeoutLbl.TabIndex = 75;
			this.TimeoutLbl.Text = "Timeout:";
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.TimeTb);
			this.panel1.Controls.Add(this.DateTb);
			this.panel1.Location = new System.Drawing.Point(681, 26);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(111, 100);
			this.panel1.TabIndex = 77;
			// 
			// ConnectStatusLbl
			// 
			this.ConnectStatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ConnectStatusLbl.AutoSize = true;
			this.ConnectStatusLbl.Location = new System.Drawing.Point(651, 506);
			this.ConnectStatusLbl.Name = "ConnectStatusLbl";
			this.ConnectStatusLbl.Size = new System.Drawing.Size(37, 13);
			this.ConnectStatusLbl.TabIndex = 78;
			this.ConnectStatusLbl.Text = "Offline";
			this.ConnectStatusLbl.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ReceiveStatusLbl
			// 
			this.ReceiveStatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ReceiveStatusLbl.AutoSize = true;
			this.ReceiveStatusLbl.Location = new System.Drawing.Point(575, 506);
			this.ReceiveStatusLbl.Name = "ReceiveStatusLbl";
			this.ReceiveStatusLbl.Size = new System.Drawing.Size(64, 13);
			this.ReceiveStatusLbl.TabIndex = 79;
			this.ReceiveStatusLbl.Text = "Receive Off";
			this.ReceiveStatusLbl.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// SendBufferStatusLbl
			// 
			this.SendBufferStatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SendBufferStatusLbl.AutoSize = true;
			this.SendBufferStatusLbl.Location = new System.Drawing.Point(150, 506);
			this.SendBufferStatusLbl.Name = "SendBufferStatusLbl";
			this.SendBufferStatusLbl.Size = new System.Drawing.Size(48, 13);
			this.SendBufferStatusLbl.TabIndex = 80;
			this.SendBufferStatusLbl.Text = "ToSend:";
			// 
			// CharSetLbl
			// 
			this.CharSetLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.CharSetLbl.AutoSize = true;
			this.CharSetLbl.Location = new System.Drawing.Point(720, 506);
			this.CharSetLbl.Name = "CharSetLbl";
			this.CharSetLbl.Size = new System.Drawing.Size(45, 13);
			this.CharSetLbl.TabIndex = 81;
			this.CharSetLbl.Text = "CharSet";
			this.CharSetLbl.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// RemoteBufferStatusLbl
			// 
			this.RemoteBufferStatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.RemoteBufferStatusLbl.AutoSize = true;
			this.RemoteBufferStatusLbl.Location = new System.Drawing.Point(226, 506);
			this.RemoteBufferStatusLbl.Name = "RemoteBufferStatusLbl";
			this.RemoteBufferStatusLbl.Size = new System.Drawing.Size(48, 13);
			this.RemoteBufferStatusLbl.TabIndex = 82;
			this.RemoteBufferStatusLbl.Text = "RemBuf:";
			// 
			// LocalBufferStatusLbl
			// 
			this.LocalBufferStatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.LocalBufferStatusLbl.AutoSize = true;
			this.LocalBufferStatusLbl.Location = new System.Drawing.Point(297, 506);
			this.LocalBufferStatusLbl.Name = "LocalBufferStatusLbl";
			this.LocalBufferStatusLbl.Size = new System.Drawing.Size(44, 13);
			this.LocalBufferStatusLbl.TabIndex = 83;
			this.LocalBufferStatusLbl.Text = "LocBuf:";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(804, 525);
			this.Controls.Add(this.LocalBufferStatusLbl);
			this.Controls.Add(this.RemoteBufferStatusLbl);
			this.Controls.Add(this.CharSetLbl);
			this.Controls.Add(this.SendBufferStatusLbl);
			this.Controls.Add(this.ReceiveStatusLbl);
			this.Controls.Add(this.ConnectStatusLbl);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.TimeoutLbl);
			this.Controls.Add(this.ConnTimeLbl);
			this.Controls.Add(this.ColumnLbl);
			this.Controls.Add(this.LineLbl);
			this.Controls.Add(this.SendHBtn);
			this.Controls.Add(this.SendGBtn);
			this.Controls.Add(this.SendFBtn);
			this.Controls.Add(this.ScrollDownBtn);
			this.Controls.Add(this.SendNullBtn);
			this.Controls.Add(this.SendBellBtn);
			this.Controls.Add(this.SendHereIsBtn);
			this.Controls.Add(this.SendLineFeedBtn);
			this.Controls.Add(this.SendCarriageReturnBtn);
			this.Controls.Add(this.SendFiguresBtn);
			this.Controls.Add(this.SendLettersBtn);
			this.Controls.Add(this.ScrollEndBtn);
			this.Controls.Add(this.SendWruBtn);
			this.Controls.Add(this.ScrollUpBtn);
			this.Controls.Add(this.ScrollStartBtn);
			this.Controls.Add(this.TerminalPb);
			this.Controls.Add(this.LinealPnl);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.panel2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(750, 480);
			this.Name = "MainForm";
			this.Text = "WinTlx";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			this.Leave += new System.EventHandler(this.MainForm_Leave);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.TerminalPb)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button ConnectBtn;
		private System.Windows.Forms.Button DisconnectBtn;
		private System.Windows.Forms.Button SendHereIsBtn;
		private System.Windows.Forms.Button SendWruBtn;
		private System.Windows.Forms.Button SendBellBtn;
		private System.Windows.Forms.Button SendLettersBtn;
		private System.Windows.Forms.Button SendFiguresBtn;
		private System.Windows.Forms.ComboBox TlnNameCb;
		private System.Windows.Forms.TextBox TlnPortTb;
		private System.Windows.Forms.TextBox TlnAddressTb;
		private System.Windows.Forms.Label TlnMemberLbl;
		private System.Windows.Forms.Label TlnAddressLbl;
		private System.Windows.Forms.Label TlnPortLbl;
		private System.Windows.Forms.Button SendCarriageReturnBtn;
		private System.Windows.Forms.Button SendLineFeedBtn;
		private System.Windows.Forms.Label SearchLbl;
		private System.Windows.Forms.Button QueryBtn;
		private System.Windows.Forms.Label TlnExtensionLbl;
		private System.Windows.Forms.TextBox TlnExtensionTb;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.TextBox AnswerbackTb;
		private System.Windows.Forms.Label AnswerbackLbl;
		private System.Windows.Forms.TextBox TimeTb;
		private System.Windows.Forms.TextBox DateTb;
		private System.Windows.Forms.Button SendNullBtn;
		private System.Windows.Forms.Panel LinealPnl;
		private SelectablePictureBox TerminalPb;
		private System.Windows.Forms.Button ScrollStartBtn;
		private System.Windows.Forms.Button ScrollUpBtn;
		private System.Windows.Forms.Button ScrollEndBtn;
		private System.Windows.Forms.Button ScrollDownBtn;
		private System.Windows.Forms.Label TlnPeerTypeLbl;
		private System.Windows.Forms.Button SendFBtn;
		private System.Windows.Forms.Button SendHBtn;
		private System.Windows.Forms.Button SendGBtn;
		private System.Windows.Forms.Label LineLbl;
		private System.Windows.Forms.Label ColumnLbl;
		private System.Windows.Forms.Label ConnTimeLbl;
		private System.Windows.Forms.Label TimeoutLbl;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label ConnectStatusLbl;
		private System.Windows.Forms.Label ReceiveStatusLbl;
		private System.Windows.Forms.Label SendBufferStatusLbl;
		private System.Windows.Forms.ComboBox SearchCb;
		private System.Windows.Forms.ComboBox TlnTypeCb;
		private System.Windows.Forms.Label CharSetLbl;
		private System.Windows.Forms.Label RemoteBufferStatusLbl;
		private System.Windows.Forms.Label LocalBufferStatusLbl;
	}
}

