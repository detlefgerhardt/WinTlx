using System.Windows.Forms;

namespace WinTlx.Controls
{
	public partial class DelayedHScrollBar : HScrollBar
	{
		private const int DELAY = 200;

		public delegate void DelayedChangedEventHandler();
		public event DelayedChangedEventHandler DelayedScroll;

		private System.Timers.Timer _timer;

		public DelayedHScrollBar()
		{
			//InitializeComponent();
			this.Scroll += DelayedHScrollBar_Scroll;
			_timer = new System.Timers.Timer(DELAY);
			_timer.Elapsed += _timer_Elapsed;
		}


		private void DelayedHScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			_timer.Start();
		}

		private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			DelayedScroll?.Invoke();
			_timer.Stop();
		}
	}
}
