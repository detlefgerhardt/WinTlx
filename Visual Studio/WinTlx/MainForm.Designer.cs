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
			this.MemberCb = new System.Windows.Forms.ComboBox();
			this.PortTb = new System.Windows.Forms.TextBox();
			this.AddressTb = new System.Windows.Forms.TextBox();
			this.MemberLbl = new System.Windows.Forms.Label();
			this.AddressLbl = new System.Windows.Forms.Label();
			this.PortLbl = new System.Windows.Forms.Label();
			this.ExitBtn = new System.Windows.Forms.Button();
			this.SendCarriageReturnBtn = new System.Windows.Forms.Button();
			this.SendLineFeedBtn = new System.Windows.Forms.Button();
			this.ProtocolItelexRb = new System.Windows.Forms.RadioButton();
			this.ProtocolAsciiRb = new System.Windows.Forms.RadioButton();
			this.SendTimeBtn = new System.Windows.Forms.Button();
			this.SendRyBtn = new System.Windows.Forms.Button();
			this.SendFoxBtn = new System.Windows.Forms.Button();
			this.LocalBtn = new System.Windows.Forms.Button();
			this.SendFileBtn = new System.Windows.Forms.Button();
			this.SearchLbl = new System.Windows.Forms.Label();
			this.SearchTb = new System.Windows.Forms.TextBox();
			this.QueryBtn = new System.Windows.Forms.Button();
			this.ExtensionLbl = new System.Windows.Forms.Label();
			this.ExtensionTb = new System.Windows.Forms.TextBox();
			this.SendLineBtn = new System.Windows.Forms.Button();
			this.ClearBtn = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SendNullBtn = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.AnswerbackLbl = new System.Windows.Forms.Label();
			this.AnswerbackTb = new System.Windows.Forms.TextBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.ConnectionStateTb = new System.Windows.Forms.TextBox();
			this.ConnTimeTb = new System.Windows.Forms.TextBox();
			this.LnColTb = new System.Windows.Forms.TextBox();
			this.IdleTimoutTb = new System.Windows.Forms.TextBox();
			this.SendAckTb = new System.Windows.Forms.TextBox();
			this.panel4 = new System.Windows.Forms.Panel();
			this.panel5 = new System.Windows.Forms.Panel();
			this.DateTb = new System.Windows.Forms.TextBox();
			this.TimeTb = new System.Windows.Forms.TextBox();
			this.AboutBtn = new System.Windows.Forms.Button();
			this.UpdateIpAddressBtn = new System.Windows.Forms.Button();
			this.ConfigBtn = new System.Windows.Forms.Button();
			this.TapePunchBtn = new System.Windows.Forms.Button();
			this.EyeballCharCb = new System.Windows.Forms.CheckBox();
			this.RecvOnCb = new System.Windows.Forms.CheckBox();
			this.LinealPnl = new System.Windows.Forms.Panel();
			this.TerminalPb = new System.Windows.Forms.PictureBox();
			this.SchedulerBtn = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.TerminalPb)).BeginInit();
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
			this.SendHereIsBtn.Location = new System.Drawing.Point(9, 40);
			this.SendHereIsBtn.Name = "SendHereIsBtn";
			this.SendHereIsBtn.Size = new System.Drawing.Size(50, 23);
			this.SendHereIsBtn.TabIndex = 13;
			this.SendHereIsBtn.Text = "Here is";
			this.SendHereIsBtn.UseVisualStyleBackColor = true;
			this.SendHereIsBtn.Click += new System.EventHandler(this.SendHereIsBtn_Click);
			// 
			// SendWruBtn
			// 
			this.SendWruBtn.Location = new System.Drawing.Point(9, 11);
			this.SendWruBtn.Name = "SendWruBtn";
			this.SendWruBtn.Size = new System.Drawing.Size(50, 23);
			this.SendWruBtn.TabIndex = 12;
			this.SendWruBtn.Text = "WRU";
			this.SendWruBtn.UseVisualStyleBackColor = true;
			this.SendWruBtn.Click += new System.EventHandler(this.SendWruBtn_Click);
			// 
			// SendBellBtn
			// 
			this.SendBellBtn.Location = new System.Drawing.Point(177, 11);
			this.SendBellBtn.Name = "SendBellBtn";
			this.SendBellBtn.Size = new System.Drawing.Size(50, 23);
			this.SendBellBtn.TabIndex = 18;
			this.SendBellBtn.Text = "Bell";
			this.SendBellBtn.UseVisualStyleBackColor = true;
			this.SendBellBtn.Click += new System.EventHandler(this.SendBellBtn_Click);
			// 
			// SendLettersBtn
			// 
			this.SendLettersBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendLettersBtn.Location = new System.Drawing.Point(65, 11);
			this.SendLettersBtn.Name = "SendLettersBtn";
			this.SendLettersBtn.Size = new System.Drawing.Size(50, 23);
			this.SendLettersBtn.TabIndex = 14;
			this.SendLettersBtn.Text = "A...";
			this.SendLettersBtn.UseVisualStyleBackColor = true;
			this.SendLettersBtn.Click += new System.EventHandler(this.SendLettersBtn_Click);
			// 
			// SendFiguresBtn
			// 
			this.SendFiguresBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendFiguresBtn.Location = new System.Drawing.Point(65, 40);
			this.SendFiguresBtn.Name = "SendFiguresBtn";
			this.SendFiguresBtn.Size = new System.Drawing.Size(50, 23);
			this.SendFiguresBtn.TabIndex = 15;
			this.SendFiguresBtn.Text = "1...";
			this.SendFiguresBtn.UseVisualStyleBackColor = true;
			this.SendFiguresBtn.Click += new System.EventHandler(this.SendFiguresBtn_Click);
			// 
			// MemberCb
			// 
			this.MemberCb.DropDownWidth = 200;
			this.MemberCb.FormattingEnabled = true;
			this.MemberCb.Location = new System.Drawing.Point(174, 23);
			this.MemberCb.Name = "MemberCb";
			this.MemberCb.Size = new System.Drawing.Size(271, 21);
			this.MemberCb.TabIndex = 3;
			this.MemberCb.SelectedIndexChanged += new System.EventHandler(this.PhoneEntryCb_SelectedIndexChanged);
			// 
			// PortTb
			// 
			this.PortTb.Location = new System.Drawing.Point(347, 65);
			this.PortTb.Name = "PortTb";
			this.PortTb.Size = new System.Drawing.Size(46, 20);
			this.PortTb.TabIndex = 5;
			this.PortTb.Leave += new System.EventHandler(this.PortTb_Leave);
			// 
			// AddressTb
			// 
			this.AddressTb.Location = new System.Drawing.Point(174, 65);
			this.AddressTb.Name = "AddressTb";
			this.AddressTb.Size = new System.Drawing.Size(165, 20);
			this.AddressTb.TabIndex = 4;
			this.AddressTb.Leave += new System.EventHandler(this.AddressTb_Leave);
			// 
			// MemberLbl
			// 
			this.MemberLbl.AutoSize = true;
			this.MemberLbl.Location = new System.Drawing.Point(171, 6);
			this.MemberLbl.Name = "MemberLbl";
			this.MemberLbl.Size = new System.Drawing.Size(146, 13);
			this.MemberLbl.TabIndex = 0;
			this.MemberLbl.Text = "Search result (select member)";
			// 
			// AddressLbl
			// 
			this.AddressLbl.AutoSize = true;
			this.AddressLbl.Location = new System.Drawing.Point(171, 48);
			this.AddressLbl.Name = "AddressLbl";
			this.AddressLbl.Size = new System.Drawing.Size(45, 13);
			this.AddressLbl.TabIndex = 0;
			this.AddressLbl.Text = "Address";
			// 
			// PortLbl
			// 
			this.PortLbl.AutoSize = true;
			this.PortLbl.Location = new System.Drawing.Point(344, 48);
			this.PortLbl.Name = "PortLbl";
			this.PortLbl.Size = new System.Drawing.Size(26, 13);
			this.PortLbl.TabIndex = 0;
			this.PortLbl.Text = "Port";
			// 
			// ExitBtn
			// 
			this.ExitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ExitBtn.Location = new System.Drawing.Point(652, 569);
			this.ExitBtn.Name = "ExitBtn";
			this.ExitBtn.Size = new System.Drawing.Size(80, 23);
			this.ExitBtn.TabIndex = 32;
			this.ExitBtn.Text = "Exit";
			this.ExitBtn.UseVisualStyleBackColor = true;
			this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
			// 
			// SendCarriageReturnBtn
			// 
			this.SendCarriageReturnBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendCarriageReturnBtn.Location = new System.Drawing.Point(121, 11);
			this.SendCarriageReturnBtn.Name = "SendCarriageReturnBtn";
			this.SendCarriageReturnBtn.Size = new System.Drawing.Size(50, 23);
			this.SendCarriageReturnBtn.TabIndex = 16;
			this.SendCarriageReturnBtn.Text = "<";
			this.SendCarriageReturnBtn.UseVisualStyleBackColor = true;
			this.SendCarriageReturnBtn.Click += new System.EventHandler(this.SendCarriageReturnBtn_Click);
			// 
			// SendLineFeedBtn
			// 
			this.SendLineFeedBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendLineFeedBtn.Location = new System.Drawing.Point(121, 40);
			this.SendLineFeedBtn.Name = "SendLineFeedBtn";
			this.SendLineFeedBtn.Size = new System.Drawing.Size(50, 23);
			this.SendLineFeedBtn.TabIndex = 17;
			this.SendLineFeedBtn.Text = "=";
			this.SendLineFeedBtn.UseVisualStyleBackColor = true;
			this.SendLineFeedBtn.Click += new System.EventHandler(this.SendLineFeedBtn_Click);
			// 
			// ProtocolItelexRb
			// 
			this.ProtocolItelexRb.AutoSize = true;
			this.ProtocolItelexRb.Checked = true;
			this.ProtocolItelexRb.Location = new System.Drawing.Point(466, 20);
			this.ProtocolItelexRb.Name = "ProtocolItelexRb";
			this.ProtocolItelexRb.Size = new System.Drawing.Size(52, 17);
			this.ProtocolItelexRb.TabIndex = 7;
			this.ProtocolItelexRb.TabStop = true;
			this.ProtocolItelexRb.Text = "i-telex";
			this.ProtocolItelexRb.UseVisualStyleBackColor = true;
			// 
			// ProtocolAsciiRb
			// 
			this.ProtocolAsciiRb.AutoSize = true;
			this.ProtocolAsciiRb.Location = new System.Drawing.Point(466, 39);
			this.ProtocolAsciiRb.Name = "ProtocolAsciiRb";
			this.ProtocolAsciiRb.Size = new System.Drawing.Size(52, 17);
			this.ProtocolAsciiRb.TabIndex = 8;
			this.ProtocolAsciiRb.Text = "ASCII";
			this.ProtocolAsciiRb.UseVisualStyleBackColor = true;
			// 
			// SendTimeBtn
			// 
			this.SendTimeBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SendTimeBtn.Location = new System.Drawing.Point(233, 11);
			this.SendTimeBtn.Name = "SendTimeBtn";
			this.SendTimeBtn.Size = new System.Drawing.Size(50, 23);
			this.SendTimeBtn.TabIndex = 20;
			this.SendTimeBtn.Text = "Time";
			this.SendTimeBtn.UseVisualStyleBackColor = true;
			this.SendTimeBtn.Click += new System.EventHandler(this.SendTimeBtn_Click);
			// 
			// SendRyBtn
			// 
			this.SendRyBtn.Location = new System.Drawing.Point(289, 11);
			this.SendRyBtn.Name = "SendRyBtn";
			this.SendRyBtn.Size = new System.Drawing.Size(50, 23);
			this.SendRyBtn.TabIndex = 22;
			this.SendRyBtn.Text = "RY";
			this.SendRyBtn.UseVisualStyleBackColor = true;
			this.SendRyBtn.Click += new System.EventHandler(this.SendRyBtn_Click);
			// 
			// SendFoxBtn
			// 
			this.SendFoxBtn.Location = new System.Drawing.Point(289, 40);
			this.SendFoxBtn.Name = "SendFoxBtn";
			this.SendFoxBtn.Size = new System.Drawing.Size(50, 23);
			this.SendFoxBtn.TabIndex = 23;
			this.SendFoxBtn.Text = "Fox";
			this.SendFoxBtn.UseVisualStyleBackColor = true;
			this.SendFoxBtn.Click += new System.EventHandler(this.SendFoxBtn_Click);
			// 
			// LocalBtn
			// 
			this.LocalBtn.Location = new System.Drawing.Point(8, 65);
			this.LocalBtn.Name = "LocalBtn";
			this.LocalBtn.Size = new System.Drawing.Size(69, 23);
			this.LocalBtn.TabIndex = 11;
			this.LocalBtn.Text = "Local";
			this.LocalBtn.UseVisualStyleBackColor = true;
			this.LocalBtn.Click += new System.EventHandler(this.LocalBtn_Click);
			// 
			// SendFileBtn
			// 
			this.SendFileBtn.Location = new System.Drawing.Point(652, 257);
			this.SendFileBtn.Name = "SendFileBtn";
			this.SendFileBtn.Size = new System.Drawing.Size(80, 23);
			this.SendFileBtn.TabIndex = 25;
			this.SendFileBtn.Text = "Send file";
			this.SendFileBtn.UseVisualStyleBackColor = true;
			this.SendFileBtn.Click += new System.EventHandler(this.SendFileBtn_Click);
			// 
			// SearchLbl
			// 
			this.SearchLbl.AutoSize = true;
			this.SearchLbl.Location = new System.Drawing.Point(8, 7);
			this.SearchLbl.Name = "SearchLbl";
			this.SearchLbl.Size = new System.Drawing.Size(61, 13);
			this.SearchLbl.TabIndex = 0;
			this.SearchLbl.Text = "Search text";
			// 
			// SearchTb
			// 
			this.SearchTb.Location = new System.Drawing.Point(8, 24);
			this.SearchTb.Name = "SearchTb";
			this.SearchTb.Size = new System.Drawing.Size(103, 20);
			this.SearchTb.TabIndex = 1;
			// 
			// QueryBtn
			// 
			this.QueryBtn.Location = new System.Drawing.Point(115, 21);
			this.QueryBtn.Name = "QueryBtn";
			this.QueryBtn.Size = new System.Drawing.Size(52, 23);
			this.QueryBtn.TabIndex = 2;
			this.QueryBtn.Text = "Search";
			this.QueryBtn.UseVisualStyleBackColor = true;
			this.QueryBtn.Click += new System.EventHandler(this.QueryBtn_Click);
			// 
			// ExtensionLbl
			// 
			this.ExtensionLbl.AutoSize = true;
			this.ExtensionLbl.Location = new System.Drawing.Point(396, 48);
			this.ExtensionLbl.Name = "ExtensionLbl";
			this.ExtensionLbl.Size = new System.Drawing.Size(53, 13);
			this.ExtensionLbl.TabIndex = 0;
			this.ExtensionLbl.Text = "Extension";
			// 
			// ExtensionTb
			// 
			this.ExtensionTb.Location = new System.Drawing.Point(399, 65);
			this.ExtensionTb.Name = "ExtensionTb";
			this.ExtensionTb.Size = new System.Drawing.Size(46, 20);
			this.ExtensionTb.TabIndex = 6;
			// 
			// SendLineBtn
			// 
			this.SendLineBtn.Location = new System.Drawing.Point(233, 40);
			this.SendLineBtn.Name = "SendLineBtn";
			this.SendLineBtn.Size = new System.Drawing.Size(50, 23);
			this.SendLineBtn.TabIndex = 21;
			this.SendLineBtn.Text = "-----";
			this.SendLineBtn.UseVisualStyleBackColor = true;
			this.SendLineBtn.Click += new System.EventHandler(this.SendLineBtn_Click);
			// 
			// ClearBtn
			// 
			this.ClearBtn.Location = new System.Drawing.Point(345, 11);
			this.ClearBtn.Name = "ClearBtn";
			this.ClearBtn.Size = new System.Drawing.Size(69, 23);
			this.ClearBtn.TabIndex = 24;
			this.ClearBtn.Text = "Clear";
			this.ClearBtn.UseVisualStyleBackColor = true;
			this.ClearBtn.Click += new System.EventHandler(this.ClearBtn_Click);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.SendNullBtn);
			this.panel1.Controls.Add(this.ClearBtn);
			this.panel1.Controls.Add(this.SendHereIsBtn);
			this.panel1.Controls.Add(this.SendWruBtn);
			this.panel1.Controls.Add(this.SendLineBtn);
			this.panel1.Controls.Add(this.SendBellBtn);
			this.panel1.Controls.Add(this.SendLettersBtn);
			this.panel1.Controls.Add(this.SendFiguresBtn);
			this.panel1.Controls.Add(this.SendCarriageReturnBtn);
			this.panel1.Controls.Add(this.SendLineFeedBtn);
			this.panel1.Controls.Add(this.SendTimeBtn);
			this.panel1.Controls.Add(this.SendRyBtn);
			this.panel1.Controls.Add(this.SendFoxBtn);
			this.panel1.Location = new System.Drawing.Point(12, 117);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(426, 76);
			this.panel1.TabIndex = 0;
			// 
			// SendNullBtn
			// 
			this.SendNullBtn.Location = new System.Drawing.Point(177, 39);
			this.SendNullBtn.Name = "SendNullBtn";
			this.SendNullBtn.Size = new System.Drawing.Size(50, 23);
			this.SendNullBtn.TabIndex = 19;
			this.SendNullBtn.Text = ".....";
			this.SendNullBtn.UseVisualStyleBackColor = true;
			this.SendNullBtn.Click += new System.EventHandler(this.SendNullBtn_Click);
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.AnswerbackLbl);
			this.panel2.Controls.Add(this.AnswerbackTb);
			this.panel2.Controls.Add(this.ProtocolAsciiRb);
			this.panel2.Controls.Add(this.MemberCb);
			this.panel2.Controls.Add(this.PortTb);
			this.panel2.Controls.Add(this.ExtensionTb);
			this.panel2.Controls.Add(this.AddressTb);
			this.panel2.Controls.Add(this.ExtensionLbl);
			this.panel2.Controls.Add(this.MemberLbl);
			this.panel2.Controls.Add(this.QueryBtn);
			this.panel2.Controls.Add(this.AddressLbl);
			this.panel2.Controls.Add(this.SearchTb);
			this.panel2.Controls.Add(this.PortLbl);
			this.panel2.Controls.Add(this.SearchLbl);
			this.panel2.Controls.Add(this.ProtocolItelexRb);
			this.panel2.Location = new System.Drawing.Point(12, 11);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(540, 100);
			this.panel2.TabIndex = 0;
			// 
			// AnswerbackLbl
			// 
			this.AnswerbackLbl.AutoSize = true;
			this.AnswerbackLbl.Location = new System.Drawing.Point(8, 48);
			this.AnswerbackLbl.Name = "AnswerbackLbl";
			this.AnswerbackLbl.Size = new System.Drawing.Size(82, 13);
			this.AnswerbackLbl.TabIndex = 0;
			this.AnswerbackLbl.Text = "Kennungsgeber";
			// 
			// AnswerbackTb
			// 
			this.AnswerbackTb.Location = new System.Drawing.Point(8, 65);
			this.AnswerbackTb.Name = "AnswerbackTb";
			this.AnswerbackTb.ReadOnly = true;
			this.AnswerbackTb.Size = new System.Drawing.Size(131, 20);
			this.AnswerbackTb.TabIndex = 0;
			this.AnswerbackTb.TabStop = false;
			// 
			// panel3
			// 
			this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel3.Controls.Add(this.ConnectionStateTb);
			this.panel3.Controls.Add(this.ConnTimeTb);
			this.panel3.Controls.Add(this.LnColTb);
			this.panel3.Controls.Add(this.IdleTimoutTb);
			this.panel3.Controls.Add(this.SendAckTb);
			this.panel3.Location = new System.Drawing.Point(444, 117);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(203, 76);
			this.panel3.TabIndex = 0;
			// 
			// ConnectionStateTb
			// 
			this.ConnectionStateTb.Location = new System.Drawing.Point(103, 51);
			this.ConnectionStateTb.Name = "ConnectionStateTb";
			this.ConnectionStateTb.ReadOnly = true;
			this.ConnectionStateTb.Size = new System.Drawing.Size(90, 20);
			this.ConnectionStateTb.TabIndex = 1;
			this.ConnectionStateTb.TabStop = false;
			// 
			// ConnTimeTb
			// 
			this.ConnTimeTb.Location = new System.Drawing.Point(103, 28);
			this.ConnTimeTb.Name = "ConnTimeTb";
			this.ConnTimeTb.ReadOnly = true;
			this.ConnTimeTb.Size = new System.Drawing.Size(90, 20);
			this.ConnTimeTb.TabIndex = 0;
			this.ConnTimeTb.TabStop = false;
			// 
			// LnColTb
			// 
			this.LnColTb.Location = new System.Drawing.Point(8, 28);
			this.LnColTb.Name = "LnColTb";
			this.LnColTb.ReadOnly = true;
			this.LnColTb.Size = new System.Drawing.Size(90, 20);
			this.LnColTb.TabIndex = 0;
			this.LnColTb.TabStop = false;
			// 
			// IdleTimoutTb
			// 
			this.IdleTimoutTb.Location = new System.Drawing.Point(103, 5);
			this.IdleTimoutTb.Name = "IdleTimoutTb";
			this.IdleTimoutTb.ReadOnly = true;
			this.IdleTimoutTb.Size = new System.Drawing.Size(90, 20);
			this.IdleTimoutTb.TabIndex = 0;
			this.IdleTimoutTb.TabStop = false;
			// 
			// SendAckTb
			// 
			this.SendAckTb.Location = new System.Drawing.Point(8, 5);
			this.SendAckTb.Name = "SendAckTb";
			this.SendAckTb.ReadOnly = true;
			this.SendAckTb.Size = new System.Drawing.Size(90, 20);
			this.SendAckTb.TabIndex = 0;
			this.SendAckTb.TabStop = false;
			// 
			// panel4
			// 
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Controls.Add(this.ConnectBtn);
			this.panel4.Controls.Add(this.DisconnectBtn);
			this.panel4.Controls.Add(this.LocalBtn);
			this.panel4.Location = new System.Drawing.Point(558, 11);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(89, 100);
			this.panel4.TabIndex = 0;
			// 
			// panel5
			// 
			this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel5.Controls.Add(this.DateTb);
			this.panel5.Controls.Add(this.TimeTb);
			this.panel5.Location = new System.Drawing.Point(653, 12);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(79, 99);
			this.panel5.TabIndex = 0;
			// 
			// DateTb
			// 
			this.DateTb.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DateTb.Location = new System.Drawing.Point(5, 32);
			this.DateTb.Name = "DateTb";
			this.DateTb.ReadOnly = true;
			this.DateTb.Size = new System.Drawing.Size(67, 22);
			this.DateTb.TabIndex = 47;
			this.DateTb.TabStop = false;
			this.DateTb.Text = "00.00.0000";
			this.DateTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TimeTb
			// 
			this.TimeTb.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TimeTb.Location = new System.Drawing.Point(5, 6);
			this.TimeTb.Name = "TimeTb";
			this.TimeTb.ReadOnly = true;
			this.TimeTb.Size = new System.Drawing.Size(67, 22);
			this.TimeTb.TabIndex = 45;
			this.TimeTb.TabStop = false;
			this.TimeTb.Text = "00:00:00";
			this.TimeTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// AboutBtn
			// 
			this.AboutBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AboutBtn.Location = new System.Drawing.Point(652, 540);
			this.AboutBtn.Name = "AboutBtn";
			this.AboutBtn.Size = new System.Drawing.Size(80, 23);
			this.AboutBtn.TabIndex = 31;
			this.AboutBtn.Text = "About";
			this.AboutBtn.UseVisualStyleBackColor = true;
			this.AboutBtn.Click += new System.EventHandler(this.AboutBtn_Click);
			// 
			// UpdateIpAddressBtn
			// 
			this.UpdateIpAddressBtn.Location = new System.Drawing.Point(652, 146);
			this.UpdateIpAddressBtn.Name = "UpdateIpAddressBtn";
			this.UpdateIpAddressBtn.Size = new System.Drawing.Size(80, 23);
			this.UpdateIpAddressBtn.TabIndex = 27;
			this.UpdateIpAddressBtn.Text = "Update";
			this.UpdateIpAddressBtn.UseVisualStyleBackColor = true;
			this.UpdateIpAddressBtn.Click += new System.EventHandler(this.UpdateIpAddressBtn_Click);
			// 
			// ConfigBtn
			// 
			this.ConfigBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ConfigBtn.Location = new System.Drawing.Point(652, 511);
			this.ConfigBtn.Name = "ConfigBtn";
			this.ConfigBtn.Size = new System.Drawing.Size(80, 23);
			this.ConfigBtn.TabIndex = 30;
			this.ConfigBtn.Text = "Config";
			this.ConfigBtn.UseVisualStyleBackColor = true;
			this.ConfigBtn.Click += new System.EventHandler(this.ConfigBtn_Click);
			// 
			// TapePunchBtn
			// 
			this.TapePunchBtn.Location = new System.Drawing.Point(652, 188);
			this.TapePunchBtn.Name = "TapePunchBtn";
			this.TapePunchBtn.Size = new System.Drawing.Size(80, 23);
			this.TapePunchBtn.TabIndex = 28;
			this.TapePunchBtn.Text = "Tape Punch";
			this.TapePunchBtn.UseVisualStyleBackColor = true;
			this.TapePunchBtn.Click += new System.EventHandler(this.TapePunchBtn_Click);
			// 
			// EyeballCharCb
			// 
			this.EyeballCharCb.Appearance = System.Windows.Forms.Appearance.Button;
			this.EyeballCharCb.Location = new System.Drawing.Point(652, 217);
			this.EyeballCharCb.Name = "EyeballCharCb";
			this.EyeballCharCb.Size = new System.Drawing.Size(80, 23);
			this.EyeballCharCb.TabIndex = 29;
			this.EyeballCharCb.Text = "Eyeball Char";
			this.EyeballCharCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.EyeballCharCb.UseVisualStyleBackColor = true;
			this.EyeballCharCb.CheckedChanged += new System.EventHandler(this.EyeballCharCb_CheckedChanged);
			// 
			// RecvOnCb
			// 
			this.RecvOnCb.Appearance = System.Windows.Forms.Appearance.Button;
			this.RecvOnCb.Location = new System.Drawing.Point(652, 117);
			this.RecvOnCb.Name = "RecvOnCb";
			this.RecvOnCb.Size = new System.Drawing.Size(80, 23);
			this.RecvOnCb.TabIndex = 26;
			this.RecvOnCb.Text = "Recv On";
			this.RecvOnCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RecvOnCb.UseVisualStyleBackColor = true;
			this.RecvOnCb.Click += new System.EventHandler(this.RecvOnCb_Click);
			// 
			// LinealPnl
			// 
			this.LinealPnl.Location = new System.Drawing.Point(13, 201);
			this.LinealPnl.Name = "LinealPnl";
			this.LinealPnl.Size = new System.Drawing.Size(633, 12);
			this.LinealPnl.TabIndex = 0;
			this.LinealPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.LinealPnl_Paint);
			// 
			// TerminalPb
			// 
			this.TerminalPb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TerminalPb.Location = new System.Drawing.Point(12, 217);
			this.TerminalPb.Name = "TerminalPb";
			this.TerminalPb.Size = new System.Drawing.Size(634, 375);
			this.TerminalPb.TabIndex = 55;
			this.TerminalPb.TabStop = false;
			this.TerminalPb.Paint += new System.Windows.Forms.PaintEventHandler(this.TerminalPb_Paint);
			this.TerminalPb.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TerminalPb_MouseClick);
			this.TerminalPb.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TerminalPb_MouseDown);
			// 
			// SchedulerBtn
			// 
			this.SchedulerBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.SchedulerBtn.Location = new System.Drawing.Point(652, 482);
			this.SchedulerBtn.Name = "SchedulerBtn";
			this.SchedulerBtn.Size = new System.Drawing.Size(80, 23);
			this.SchedulerBtn.TabIndex = 56;
			this.SchedulerBtn.Text = "Scheduler";
			this.SchedulerBtn.UseVisualStyleBackColor = true;
			this.SchedulerBtn.Click += new System.EventHandler(this.SchedulerBtn_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(741, 604);
			this.Controls.Add(this.SchedulerBtn);
			this.Controls.Add(this.TerminalPb);
			this.Controls.Add(this.LinealPnl);
			this.Controls.Add(this.RecvOnCb);
			this.Controls.Add(this.EyeballCharCb);
			this.Controls.Add(this.TapePunchBtn);
			this.Controls.Add(this.ConfigBtn);
			this.Controls.Add(this.UpdateIpAddressBtn);
			this.Controls.Add(this.AboutBtn);
			this.Controls.Add(this.panel5);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.SendFileBtn);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.ExitBtn);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(750, 480);
			this.Name = "MainForm";
			this.Text = "WinTlx";
			this.Activated += new System.EventHandler(this.MainForm_Activated);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.TerminalPb)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button ConnectBtn;
		private System.Windows.Forms.Button DisconnectBtn;
		private System.Windows.Forms.Button SendHereIsBtn;
		private System.Windows.Forms.Button SendWruBtn;
		private System.Windows.Forms.Button SendBellBtn;
		private System.Windows.Forms.Button SendLettersBtn;
		private System.Windows.Forms.Button SendFiguresBtn;
		private System.Windows.Forms.ComboBox MemberCb;
		private System.Windows.Forms.TextBox PortTb;
		private System.Windows.Forms.TextBox AddressTb;
		private System.Windows.Forms.Label MemberLbl;
		private System.Windows.Forms.Label AddressLbl;
		private System.Windows.Forms.Label PortLbl;
		private System.Windows.Forms.Button ExitBtn;
		private System.Windows.Forms.Button SendCarriageReturnBtn;
		private System.Windows.Forms.Button SendLineFeedBtn;
		private System.Windows.Forms.RadioButton ProtocolItelexRb;
		private System.Windows.Forms.RadioButton ProtocolAsciiRb;
		private System.Windows.Forms.Button SendTimeBtn;
		private System.Windows.Forms.Button SendRyBtn;
		private System.Windows.Forms.Button SendFoxBtn;
		private System.Windows.Forms.Button LocalBtn;
		private System.Windows.Forms.Button SendFileBtn;
		private System.Windows.Forms.Label SearchLbl;
		private System.Windows.Forms.TextBox SearchTb;
		private System.Windows.Forms.Button QueryBtn;
		private System.Windows.Forms.Label ExtensionLbl;
		private System.Windows.Forms.TextBox ExtensionTb;
		private System.Windows.Forms.Button SendLineBtn;
		private System.Windows.Forms.Button ClearBtn;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Button AboutBtn;
		private System.Windows.Forms.TextBox AnswerbackTb;
		private System.Windows.Forms.Label AnswerbackLbl;
		private System.Windows.Forms.TextBox SendAckTb;
		private System.Windows.Forms.TextBox IdleTimoutTb;
		private System.Windows.Forms.TextBox LnColTb;
		private System.Windows.Forms.TextBox ConnTimeTb;
		private System.Windows.Forms.TextBox TimeTb;
		private System.Windows.Forms.TextBox DateTb;
		private System.Windows.Forms.Button UpdateIpAddressBtn;
		private System.Windows.Forms.Button ConfigBtn;
		private System.Windows.Forms.Button TapePunchBtn;
		private System.Windows.Forms.Button SendNullBtn;
		private System.Windows.Forms.CheckBox EyeballCharCb;
		private System.Windows.Forms.CheckBox RecvOnCb;
		private System.Windows.Forms.Panel LinealPnl;
		private System.Windows.Forms.PictureBox TerminalPb;
		private System.Windows.Forms.TextBox ConnectionStateTb;
		private System.Windows.Forms.Button SchedulerBtn;
	}
}

