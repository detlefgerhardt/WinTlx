using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Scheduler
{
	class SchedulerItemComparer : IComparer<SchedulerItem>
	{
		public int Compare(SchedulerItem item1, SchedulerItem item2)
		{
			int c = item1.Timestamp.CompareTo(item2.Timestamp);
			if (c != 0)
			{
				return c;
			}

			c = item2.Active.CompareTo(item1.Active);
			if (c != 0)
			{
				return c;
			}

			string d1 = string.IsNullOrEmpty(item1.Destination) ? "" : item1.Destination;
			string d2 = string.IsNullOrEmpty(item2.Destination) ? "" : item2.Destination;
			c = d1.CompareTo(d2);
			return c;
		}

	}
}
