using System.Runtime.Serialization;

namespace WinTlx.Config
{
	public enum CodeStandards { Ita2, UsTTy }

	[DataContract(Namespace = "")]
	public class ConfigData
	{
		[DataMember]
		public string Language { get; set; }

		[DataMember]
		public string SubscribeServerAddress { get; set; }

		[DataMember]
		public int SubscribeServerPort { get; set; }

		[DataMember]
		public int SubscribeServerUpdatePin { get; set; }

		[DataMember]
		public string Answerback { get; set; }

		[DataMember]
		public int OutputSpeed { get; set; }

		[DataMember]
		public CodeStandards CodeStandard { get; set; }

		[DataMember]
		public int IdleTimeout { get; set; }

		[DataMember]
		public int OwnNumber { get; set; }

		[DataMember]
		public int IncomingExtensionNumber { get; set; }

		[DataMember]
		public int IncomingLocalPort { get; set; }

		[DataMember]
		public int IncomingPublicPort { get; set; }

		/// <summary>
		/// Set default values for empty fields
		/// </summary>
		public void SetDefaults()
		{
			if (string.IsNullOrWhiteSpace(Language))
			{
				Language = Constants.DEFAULT_LANGUAGE;
			}
			if (string.IsNullOrWhiteSpace(Answerback))
			{
				Answerback = Constants.DEFAULT_ANSWERBACK;
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