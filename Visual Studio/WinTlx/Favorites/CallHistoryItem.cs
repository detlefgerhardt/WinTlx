using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WinTlx.Favorites
{
	[DataContract(Namespace = "")]
	class CallHistoryItem
	{
		[DataMember]
		public string Number { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public DateTime TimeStamp { get; set; }

		[DataMember]
		public string Result { get; set; }
	}

	class CallHistoryItemSorter : IComparer<CallHistoryItem>
	{
		public CallHistoryItemSorter()
		{
		}

		public int Compare(CallHistoryItem item1, CallHistoryItem item2)
		{
			return item2.TimeStamp.CompareTo(item1.TimeStamp);
		}
	}

}
