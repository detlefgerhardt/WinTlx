using System.Runtime.Serialization;

namespace WinTelex
{
	[DataContract]
	public class ConfigData
	{
		[DataMember]
		public string SubscribeServerAddress { get; set; }

		[DataMember]
		public int SubscribeServerPort { get; set; }

		[DataMember]
		public string Kennung { get; set; }

		[DataMember]
		public int InactivityTimeout { get; set; }

		[DataMember]
		public int IncomingPort { get; set; }

		public void SetDefaults()
		{
			if (string.IsNullOrWhiteSpace(Kennung))
			{
				Kennung = Constants.DEFAULT_KENNUNG;
			}
			if (InactivityTimeout == 0)
			{
				InactivityTimeout = Constants.DEFAULT_INACTIVITY_TIMEOUT;
			}
			if (IncomingPort == 0)
			{
				IncomingPort = Constants.DEFAULT_INCOMING_PORT;
			}
		}
	}
}