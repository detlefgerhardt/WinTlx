using System;
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
		public bool ShowTechnicalMessages { get; set; }

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
		public bool UpperCaseChar { get; set; }

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

		private string _optionsStr;

		[DataMember]
		public string Options {
			get
			{
				return _optionsStr;
			}
			set
			{
				string optionsStr = value.Trim();
				_optionsStr = optionsStr;
				ParseOptions(optionsStr);
			}
		}

		public string OptionVersion { get; set; }

		public bool OptionHide { get; set; }


		/// <summary>
		/// Set default values for empty fields
		/// </summary>
		public void SetDefaults()
		{
			if (string.IsNullOrWhiteSpace(Language))
			{
				Language = Constants.DEFAULT_LANGUAGE;
			}
			//if (string.IsNullOrWhiteSpace(Answerback))
			//{
			//	Answerback = @"\r\n" + Constants.DEFAULT_ANSWERBACK;
			//}
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

		public string AnswerbackWinTlx
		{
			get
			{
				string answerback;
				string defaultAnswerback;
				if (!UpperCaseChar)
				{
					answerback = Answerback.ToLower();
					defaultAnswerback = Constants.DEFAULT_ANSWERBACK.ToLower();
				}
				else
				{
					answerback = Answerback.ToUpper();
					answerback = answerback.Replace("\\R", "\\r");
					answerback = answerback.Replace("\\N", "\\n");
					defaultAnswerback = Constants.DEFAULT_ANSWERBACK.ToUpper();
					defaultAnswerback = defaultAnswerback.Replace("\\R", "\\r");
					defaultAnswerback = defaultAnswerback.Replace("\\N", "\\n");
				}

				if (!OptionHide && string.IsNullOrWhiteSpace(answerback))
				{
					return defaultAnswerback;
				}
				else if (OptionHide || answerback.Contains(defaultAnswerback))
				{
					return answerback;
				}
				else
				{
					return answerback + $" ({defaultAnswerback})";
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

		public static string CheckLogPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path)) return "";

			if (!path.EndsWith("\\")) path += "\\";
			path = Path.GetDirectoryName(path);
			if (!path.EndsWith("\\")) path += "\\";

			try
			{
				string testName = Path.Combine(path, "test");
				File.WriteAllText(testName, "test");
				File.Delete(testName);
				return path;
			}
			catch (Exception)
			{
				// error writing log-file
				return "";
			}
		}

		private void ParseOptions(string optionsStr)
		{
			// defaults
			OptionVersion = null;
			OptionHide = false;

			if (string.IsNullOrEmpty(optionsStr)) return;

			string[] options = optionsStr.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
			if (options.Length == 0) return;
			foreach (string option in options)
			{
				string key = option.Substring(0, 1).ToLower();
				string param = option.Length > 1 ? option.Substring(1) : null;
				switch (key)
				{
					case "v": // version
						OptionVersion = param;
						break;
					case "h": // hide
						OptionHide = true;
						break;
				}
			}
		}
	}
}