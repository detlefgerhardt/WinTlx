using System.Runtime.Serialization;

namespace WinTlx
{
	public enum CodeStandards { Ita2, UsTTy }

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
		public int OutputSpeed { get; set; }

		[DataMember]
		public CodeStandards CodeStandard { get; set; }

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

		public static CodeStandards StringToCodeStandard(string stdStr)
		{
			switch(stdStr.ToUpper())
			{
				default:
				case "ITA-2":
					return CodeStandards.Ita2;
				case "US-TTY":
					return CodeStandards.UsTTy;
			}
		}

		public static string CodeStandardToString(CodeStandards std)
		{
			switch (std)
			{
				default:
				case CodeStandards.Ita2:
					return "ITA-2";
				case CodeStandards.UsTTy:
					return "US-TTY";
			}
		}

	}
}