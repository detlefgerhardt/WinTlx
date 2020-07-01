using System;
using System.IO;
using WinTlx.Codes;

namespace WinTlx.Config
{
	class ConfigManager
	{
		private const string TAG = nameof(ConfigManager);

		private const string CONFIG_NAME = Constants.PROGRAM_NAME + ".cfg";

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static ConfigManager instance;

		public static ConfigManager Instance => instance ?? (instance = new ConfigManager());

		private ConfigManager()
		{
		}

		public ConfigData Config { get; set; }

		public ConfigData GetDefaultConfig()
		{
			return new ConfigData()
			{
				Language = "en",
				Answerback = @"\r\n" + Constants.DEFAULT_ANSWERBACK,
				IdleTimeout = Constants.DEFAULT_IDLE_TIMEOUT,
				OutputSpeed = 0,
				CodeSet = CodeSets.ITA2,
				IncomingLocalPort = Constants.DEFAULT_INCOMING_PORT,
				IncomingPublicPort = Constants.DEFAULT_INCOMING_PORT,
				IncomingExtensionNumber = 0,
				LimitedClient = false,
				RemoteServerPort = Constants.CENTRALEX_PORT,
			};
		}

		public bool LoadConfig()
		{
			try
			{
				if (!File.Exists(CONFIG_NAME))
				{
					Config = GetDefaultConfig();
					Logging.Instance.Info(TAG, nameof(LoadConfig), "No config file found");
					return false;
				}

				string configXml = File.ReadAllText(CONFIG_NAME);
				Config = Helper.Deserialize<ConfigData>(configXml);
				Config.SetDefaults();
				return true;
			}
			catch(Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(LoadConfig), "Error reading config file", ex);
				Config = GetDefaultConfig();
				return false;
			}
		}

		public bool SaveConfig()
		{
			try
			{
				string configXml = Helper.SerializeObject<ConfigData>(Config);
				File.WriteAllText(CONFIG_NAME, configXml);
				Logging.Instance.Info(TAG, nameof(SaveConfig), $"Config saved to {CONFIG_NAME}");
				return true;
			}
			catch(Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SaveConfig), "Error writing config file", ex);
				return false;
			}
		}

	}
}
