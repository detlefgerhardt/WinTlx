using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WinTlx.Scheduler
{
	[DataContract(Namespace = "")]
	class ScheduleData
	{
		[DataMember]
		public List<SchedulerItem> SchedulerList { get; set; }

		public ScheduleData()
		{
			SchedulerList = new List<SchedulerItem>();
		}
	}
}