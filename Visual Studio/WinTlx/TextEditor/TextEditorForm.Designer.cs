namespace WinTlx.TextEditor
{
	partial class TextEditorForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditorForm));
			this.EditorRtb = new WinTlx.Controls.RicherTextBox2();
			this.LinealPnl = new System.Windows.Forms.Panel();
			this.ClearBtn = new System.Windows.Forms.Button();
			this.SendBtn = new System.Windows.Forms.Button();
			this.CloseBtn = new System.Windows.Forms.Button();
			this.LoadBtn = new System.Windows.Forms.Button();
			this.SaveBtn = new System.Windows.Forms.Button();
			this.LineNrLbl = new System.Windows.Forms.Label();
			this.ColumnNrLbl = new System.Windows.Forms.Label();
			this.AlignBlockBtn = new System.Windows.Forms.Button();
			this.AlignLeftBtn = new System.Windows.Forms.Button();
			this.CharWidthTb = new System.Windows.Forms.TextBox();
			this.ConvertRttyArtBtn = new System.Windows.Forms.Button();
			this.ShiftLeftBtn = new System.Windows.Forms.Button();
			this.ShiftRightBtn = new System.Windows.Forms.Button();
			this.UndoBtn = new System.Windows.Forms.Button();
			this.CharWidthLbl = new System.Windows.Forms.Label();
			this.RedoBtn = new System.Windows.Forms.Button();
			this.ConvertToBaudotBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// EditorRtb
			// 
			this.EditorRtb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.EditorRtb.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EditorRtb.Location = new System.Drawing.Point(13, 84);
			this.EditorRtb.Name = "EditorRtb";
			this.EditorRtb.Size = new System.Drawing.Size(659, 366);
			this.EditorRtb.TabIndex = 0;
			this.EditorRtb.Text = "";
			this.EditorRtb.CursorPositionChanged += new System.EventHandler(this.EditorRtb_CursorPositionChanged);
			this.EditorRtb.SelectionChanged += new System.EventHandler(this.EditorRtb_SelectionChanged);
			this.EditorRtb.TextChanged += new System.EventHandler(this.EditorRtb_TextChanged);
			this.EditorRtb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditorRtb_KeyPress);
			this.EditorRtb.Leave += new System.EventHandler(this.EditorRtb_Leave);
			// 
			// LinealPnl
			// 
			this.LinealPnl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.LinealPnl.Location = new System.Drawing.Point(13, 70);
			this.LinealPnl.Name = "LinealPnl";
			this.LinealPnl.Size = new System.Drawing.Size(658, 12);
			this.LinealPnl.TabIndex = 1;
			this.LinealPnl.Paint += new System.Windows.Forms.PaintEventHandler(this.LinealPnl_Paint);
			// 
			// ClearBtn
			// 
			this.ClearBtn.Location = new System.Drawing.Point(13, 12);
			this.ClearBtn.Name = "ClearBtn";
			this.ClearBtn.Size = new System.Drawing.Size(65, 23);
			this.ClearBtn.TabIndex = 2;
			this.ClearBtn.Text = "Clear";
			this.ClearBtn.UseVisualStyleBackColor = true;
			this.ClearBtn.Click += new System.EventHandler(this.ClearBtn_Click);
			// 
			// SendBtn
			// 
			this.SendBtn.Location = new System.Drawing.Point(535, 12);
			this.SendBtn.Name = "SendBtn";
			this.SendBtn.Size = new System.Drawing.Size(65, 23);
			this.SendBtn.TabIndex = 3;
			this.SendBtn.Text = "Send";
			this.SendBtn.UseVisualStyleBackColor = true;
			this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
			// 
			// CloseBtn
			// 
			this.CloseBtn.Location = new System.Drawing.Point(606, 12);
			this.CloseBtn.Name = "CloseBtn";
			this.CloseBtn.Size = new System.Drawing.Size(65, 23);
			this.CloseBtn.TabIndex = 4;
			this.CloseBtn.Text = "Close";
			this.CloseBtn.UseVisualStyleBackColor = true;
			this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
			// 
			// LoadBtn
			// 
			this.LoadBtn.Location = new System.Drawing.Point(84, 12);
			this.LoadBtn.Name = "LoadBtn";
			this.LoadBtn.Size = new System.Drawing.Size(65, 23);
			this.LoadBtn.TabIndex = 5;
			this.LoadBtn.Text = "Load";
			this.LoadBtn.UseVisualStyleBackColor = true;
			this.LoadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
			// 
			// SaveBtn
			// 
			this.SaveBtn.Location = new System.Drawing.Point(84, 41);
			this.SaveBtn.Name = "SaveBtn";
			this.SaveBtn.Size = new System.Drawing.Size(65, 23);
			this.SaveBtn.TabIndex = 6;
			this.SaveBtn.Text = "Save";
			this.SaveBtn.UseVisualStyleBackColor = true;
			this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
			// 
			// LineNrLbl
			// 
			this.LineNrLbl.AutoSize = true;
			this.LineNrLbl.Location = new System.Drawing.Point(339, 17);
			this.LineNrLbl.Name = "LineNrLbl";
			this.LineNrLbl.Size = new System.Drawing.Size(40, 13);
			this.LineNrLbl.TabIndex = 7;
			this.LineNrLbl.Text = "Ln:000";
			// 
			// ColumnNrLbl
			// 
			this.ColumnNrLbl.AutoSize = true;
			this.ColumnNrLbl.Location = new System.Drawing.Point(382, 17);
			this.ColumnNrLbl.Name = "ColumnNrLbl";
			this.ColumnNrLbl.Size = new System.Drawing.Size(37, 13);
			this.ColumnNrLbl.TabIndex = 8;
			this.ColumnNrLbl.Text = "Col:00";
			// 
			// AlignBlockBtn
			// 
			this.AlignBlockBtn.Location = new System.Drawing.Point(238, 12);
			this.AlignBlockBtn.Name = "AlignBlockBtn";
			this.AlignBlockBtn.Size = new System.Drawing.Size(50, 23);
			this.AlignBlockBtn.TabIndex = 9;
			this.AlignBlockBtn.Text = "Block";
			this.AlignBlockBtn.UseVisualStyleBackColor = true;
			this.AlignBlockBtn.Click += new System.EventHandler(this.AlignBlockBtn_Click);
			// 
			// AlignLeftBtn
			// 
			this.AlignLeftBtn.Location = new System.Drawing.Point(238, 41);
			this.AlignLeftBtn.Name = "AlignLeftBtn";
			this.AlignLeftBtn.Size = new System.Drawing.Size(50, 23);
			this.AlignLeftBtn.TabIndex = 10;
			this.AlignLeftBtn.Text = "Left";
			this.AlignLeftBtn.UseVisualStyleBackColor = true;
			this.AlignLeftBtn.Click += new System.EventHandler(this.AlignLeftBtn_Click);
			// 
			// CharWidthTb
			// 
			this.CharWidthTb.Location = new System.Drawing.Point(379, 44);
			this.CharWidthTb.Name = "CharWidthTb";
			this.CharWidthTb.Size = new System.Drawing.Size(30, 20);
			this.CharWidthTb.TabIndex = 11;
			this.CharWidthTb.Text = "68";
			this.CharWidthTb.Leave += new System.EventHandler(this.CharWidthTb_Leave);
			// 
			// ConvertRttyArtBtn
			// 
			this.ConvertRttyArtBtn.Location = new System.Drawing.Point(171, 42);
			this.ConvertRttyArtBtn.Name = "ConvertRttyArtBtn";
			this.ConvertRttyArtBtn.Size = new System.Drawing.Size(50, 23);
			this.ConvertRttyArtBtn.TabIndex = 12;
			this.ConvertRttyArtBtn.Text = "RttyArt";
			this.ConvertRttyArtBtn.UseVisualStyleBackColor = true;
			this.ConvertRttyArtBtn.Click += new System.EventHandler(this.ConvertToRttyArtBtn_Click);
			// 
			// ShiftLeftBtn
			// 
			this.ShiftLeftBtn.Location = new System.Drawing.Point(294, 12);
			this.ShiftLeftBtn.Name = "ShiftLeftBtn";
			this.ShiftLeftBtn.Size = new System.Drawing.Size(30, 23);
			this.ShiftLeftBtn.TabIndex = 13;
			this.ShiftLeftBtn.Text = "<";
			this.ShiftLeftBtn.UseVisualStyleBackColor = true;
			this.ShiftLeftBtn.Click += new System.EventHandler(this.ShiftLeftBtn_Click);
			// 
			// ShiftRightBtn
			// 
			this.ShiftRightBtn.Location = new System.Drawing.Point(294, 41);
			this.ShiftRightBtn.Name = "ShiftRightBtn";
			this.ShiftRightBtn.Size = new System.Drawing.Size(30, 23);
			this.ShiftRightBtn.TabIndex = 14;
			this.ShiftRightBtn.Text = ">";
			this.ShiftRightBtn.UseVisualStyleBackColor = true;
			this.ShiftRightBtn.Click += new System.EventHandler(this.ShiftRightBtn_Click);
			// 
			// UndoBtn
			// 
			this.UndoBtn.Location = new System.Drawing.Point(471, 12);
			this.UndoBtn.Name = "UndoBtn";
			this.UndoBtn.Size = new System.Drawing.Size(50, 23);
			this.UndoBtn.TabIndex = 15;
			this.UndoBtn.Text = "Undo";
			this.UndoBtn.UseVisualStyleBackColor = true;
			this.UndoBtn.Visible = false;
			this.UndoBtn.Click += new System.EventHandler(this.UndoBtn_Click);
			// 
			// CharWidthLbl
			// 
			this.CharWidthLbl.AutoSize = true;
			this.CharWidthLbl.Location = new System.Drawing.Point(339, 46);
			this.CharWidthLbl.Name = "CharWidthLbl";
			this.CharWidthLbl.Size = new System.Drawing.Size(38, 13);
			this.CharWidthLbl.TabIndex = 16;
			this.CharWidthLbl.Text = "Width:";
			// 
			// RedoBtn
			// 
			this.RedoBtn.Location = new System.Drawing.Point(471, 41);
			this.RedoBtn.Name = "RedoBtn";
			this.RedoBtn.Size = new System.Drawing.Size(50, 23);
			this.RedoBtn.TabIndex = 17;
			this.RedoBtn.Text = "Redo";
			this.RedoBtn.UseVisualStyleBackColor = true;
			this.RedoBtn.Visible = false;
			this.RedoBtn.Click += new System.EventHandler(this.RedoBtn_Click);
			// 
			// ConvertToBaudotBtn
			// 
			this.ConvertToBaudotBtn.Location = new System.Drawing.Point(171, 12);
			this.ConvertToBaudotBtn.Name = "ConvertToBaudotBtn";
			this.ConvertToBaudotBtn.Size = new System.Drawing.Size(50, 23);
			this.ConvertToBaudotBtn.TabIndex = 18;
			this.ConvertToBaudotBtn.Text = "Baudot";
			this.ConvertToBaudotBtn.UseVisualStyleBackColor = true;
			this.ConvertToBaudotBtn.Click += new System.EventHandler(this.ConvertToBaudotBtn_Click);
			// 
			// TextEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(684, 462);
			this.Controls.Add(this.ConvertToBaudotBtn);
			this.Controls.Add(this.RedoBtn);
			this.Controls.Add(this.CharWidthLbl);
			this.Controls.Add(this.UndoBtn);
			this.Controls.Add(this.ShiftRightBtn);
			this.Controls.Add(this.ShiftLeftBtn);
			this.Controls.Add(this.ConvertRttyArtBtn);
			this.Controls.Add(this.CharWidthTb);
			this.Controls.Add(this.AlignLeftBtn);
			this.Controls.Add(this.AlignBlockBtn);
			this.Controls.Add(this.ColumnNrLbl);
			this.Controls.Add(this.LineNrLbl);
			this.Controls.Add(this.SaveBtn);
			this.Controls.Add(this.LoadBtn);
			this.Controls.Add(this.CloseBtn);
			this.Controls.Add(this.SendBtn);
			this.Controls.Add(this.ClearBtn);
			this.Controls.Add(this.LinealPnl);
			this.Controls.Add(this.EditorRtb);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "TextEditorForm";
			this.Text = "TextEditor";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TextEditorForm_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Controls.RicherTextBox2 EditorRtb;
		private System.Windows.Forms.Panel LinealPnl;
		private System.Windows.Forms.Button ClearBtn;
		private System.Windows.Forms.Button SendBtn;
		private System.Windows.Forms.Button CloseBtn;
		private System.Windows.Forms.Button LoadBtn;
		private System.Windows.Forms.Button SaveBtn;
		private System.Windows.Forms.Label LineNrLbl;
		private System.Windows.Forms.Label ColumnNrLbl;
		private System.Windows.Forms.Button AlignBlockBtn;
		private System.Windows.Forms.Button AlignLeftBtn;
		private System.Windows.Forms.TextBox CharWidthTb;
		private System.Windows.Forms.Button ConvertRttyArtBtn;
		private System.Windows.Forms.Button ShiftLeftBtn;
		private System.Windows.Forms.Button ShiftRightBtn;
		private System.Windows.Forms.Button UndoBtn;
		private System.Windows.Forms.Label CharWidthLbl;
		private System.Windows.Forms.Button RedoBtn;
		private System.Windows.Forms.Button ConvertToBaudotBtn;
	}
}