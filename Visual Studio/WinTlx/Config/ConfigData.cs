using System.IO;
using System.Runtime.Serialization;
using WinTlx.Codes;

namespace WinTlx.Config
{
	[DataContract(Namespace = "")]
	public class ConfigData
	{
		[DataMember]
		public string Language { get; set; }

		[DataMember]
		public string LogfilePath { get; set; }

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
		public CodeSets CodeSet { get; set; }

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
			if (string.IsNullOrEmpty(Language))
			{
				Language = "en";
			}
		}

		public static CodeSets StringToCodeSet(string stdStr)
		{
			switch(stdStr.ToUpper())
			{
				default:
				case "ITA-2":
					return CodeSets.ITA2;
				//case "ITA-2 EXT":
				//	return CodeSets.ITA2EXT;
				case "US-TTY":
					return CodeSets.USTTY;
			}
		}

		public static string CodeSetToString(CodeSets std)
		{
			switch (std)
			{
				default:
				case CodeSets.ITA2:
					return "ITA-2";
				//case CodeSets.ITA2EXT:
				//	return "ITA-2 EXT";
				case CodeSets.USTTY:
					return "US-TTY";
			}
		}

		public static string FormatLogPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return "";
			}
			if (!path.EndsWith("\\"))
			{
				path += "\\";
			};
			return path;
		}
	}
}