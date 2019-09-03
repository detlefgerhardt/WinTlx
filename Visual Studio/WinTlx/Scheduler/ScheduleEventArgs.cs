using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Scheduler
{
	class ScheduleEventArgs
	{
		public SchedulerItem Item { get; set; }
		//public bool Done { get; set; }

		public ScheduleEventArgs(SchedulerItem item)
		{
			Item = item;
			//Done = false;
		}
	}
}
