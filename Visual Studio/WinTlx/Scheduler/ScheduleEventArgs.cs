namespace WinTlx.Scheduler
{
	class ScheduleEventArgs
	{
		public SchedulerItem Item { get; set; }

		public ScheduleEventArgs(SchedulerItem item)
		{
			Item = item;
		}
	}
}
