using System;
using System.Drawing;
using System.Windows.Forms;

class SelectablePictureBox : PictureBox
{
	public delegate void EnterEventHandler();
	public event EnterEventHandler Enter;

	public delegate void LeaveEventHandler();
	public event LeaveEventHandler Leave;


	public SelectablePictureBox()
	{
		this.SetStyle(ControlStyles.Selectable, true);
		this.TabStop = true;
	}
	protected override void OnMouseDown(MouseEventArgs e)
	{
		this.Focus();
		base.OnMouseDown(e);
	}
	protected override void OnEnter(EventArgs e)
	{
		this.Invalidate();
		Enter?.Invoke();
		base.OnEnter(e);
	}
	protected override void OnLeave(EventArgs e)
	{
		this.Invalidate();
		Leave?.Invoke();
		base.OnLeave(e);
	}
	protected override void OnPaint(PaintEventArgs pe)
	{
		base.OnPaint(pe);
		if (this.Focused)
		{
			var rc = this.ClientRectangle;
			rc.Inflate(-2, -2);
			ControlPaint.DrawFocusRectangle(pe.Graphics, rc);
		}
	}
}