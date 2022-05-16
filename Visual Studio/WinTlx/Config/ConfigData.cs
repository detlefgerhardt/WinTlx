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
		public string SubscribeServerAddress2 { get; set; }

		[DataMember]
		public string SubscribeServerAddress3 { get; set; }

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
		public bool DefaultProtocolAscii { get; set; } 

		[DataMember]
		public int IdleTimeout { get; set; }

		[DataMember]
		public int RemoteBufferSize { get; set; }

		[DataMember]
		public int OwnNumber { get; set; }

		[DataMember]
		public int IncomingExtensionNumber { get; set; }

		[DataMember]
		public int IncomingLocalPort { get; set; }

		[DataMember]
		public int IncomingPublicPort { get; set; }

		[DataMember]
		public bool LimitedClient { get; set; }

		[DataMember]
		public string RemoteServerAddress { get; set; }

		[DataMember]
		public int RemoteServerPort { get; set; }

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
				Answerback = @"\r\n" + Constants.DEFAULT_ANSWERBACK;
			}
			if (RemoteBufferSize == 0)
			{
				RemoteBufferSize = Constants.DEFAULT_REMOTE_BUFFER_SIZE;
			}
			if (string.IsNullOrEmpty(Language))
			{
				Language = "en";
			}
			if (RemoteServerPort == 0)
			{
				RemoteServerPort = Constants.CENTRALEX_PORT;
			}
		}

		public string[] SubscribeServerAddresses
		{
			get
			{
				string[] addresses = new string[3];
				addresses[0] = SubscribeServerAddress;
				addresses[1] = SubscribeServerAddress2;
				addresses[2] = SubscribeServerAddress3;
				return addresses;
			}
		}

		public bool SubscribeServerAddressExists
		{
			get
			{
				string[] addresses = SubscribeServerAddresses;
				foreach(string addr in addresses)
				{
					if (!string.IsNullOrWhiteSpace(addr)) return true;
				}
				return false;
			}
		}

		/*
		public string AnswerbackPlain
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Answerback))
				{
					return Answerback;
				}
				else
				{
					int len = Answerback.Length;
					if (Answerback[len-1]=='-')
					{
						return Answerback.Substring(0, len - 1);
					}
					else
					{
						return Answerback;
					}
				}
			}
		}

		public bool AnswerbackTweak
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Answerback) && Answerback[Answerback.Length - 1] == '-';
			}
		}
		*/

		public string AnswerbackWinTlx
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Answerback))
				{
					// add "wintlx" as default for empty answerback
					return "wintlx";
				}
				else if (Answerback[Answerback.Length - 1] == '-')
				{
					// answerback ends with '-': tweak to prohibit "wintlx"
					return Answerback.Substring(0, Answerback.Length - 1);
				}
				else if (Answerback.Contains("wintlx"))
				{
					// answerback contains "wintlx", no need to add "wintlx"
					return Answerback;
				}
				else
				{
					// add "wintlx"
					return Answerback + " (wintlx)";
				}
			}
		}

		public static CodeSets StringToCodeSet(string stdStr)
		{
			switch(stdStr.ToUpper())
			{
				default:
				case "ITA-2":
					return CodeSets.ITA2;
				case "US-TTY":
					return CodeSets.USTTY;
				case "CYRILL":
					return CodeSets.CYRILL;
			}
		}

		public static string CodeSetToString(CodeSets std)
		{
			switch (std)
			{
				default:
				case CodeSets.ITA2:
					return "ITA-2";
				case CodeSets.USTTY:
					return "US-TTY";
				case CodeSets.CYRILL:
					return "CYRILL";
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