using System.Runtime.Serialization;

namespace WinTlx
{
	[DataContract]
	public class ConfigData
	{
		[DataMember]
		public string SubscribeServerAddress { get; set; }

		[DataMember]
		public int SubscribeServerPort { get; set; }

		[DataMember]
		public int SubscribeServerUpdatePin { get; set; }

		[DataMember]
		public string Kennung { get; set; }

		[DataMember]
		public int InactivityTimeout { get; set; }

		[DataMember]
		public int OwnNumber { get; set; }

		[DataMember]
		public int IncomingExtensionNumber { get; set; }

		[DataMember]
		public int IncomingLocalPort { get; set; }

		[DataMember]
		public int IncomingPublicPort { get; set; }

		[DataMember]
		public int OutputSpeed { get; set; }

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
			if (IncomingLocalPort == 0)
			{
				IncomingLocalPort = Constants.DEFAULT_INCOMING_PORT;
			}
		}
	}
}