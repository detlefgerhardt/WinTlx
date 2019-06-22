using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx
{
	class ConfigManager
	{
		private const string CONFIG_NAME = Constants.PROGRAM_NAME + ".cfg";

		private const string TAG = nameof(ConfigManager);

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static ConfigManager instance;

		public static ConfigManager Instance => instance ?? (instance = new ConfigManager());

		public ConfigData Config { get; set; }

		public ConfigData GetDefaultConfig()
		{
			return new ConfigData()
			{
				Kennung = Constants.DEFAULT_KENNUNG,
				InactivityTimeout = Constants.DEFAULT_INACTIVITY_TIMEOUT,
				OutputSpeed = 0,
				CodeStandard = CodeStandards.Ita2,
				IncomingLocalPort = Constants.DEFAULT_INCOMING_PORT,
				IncomingPublicPort = Constants.DEFAULT_INCOMING_PORT,
				IncomingExtensionNumber = 0,
			};
		}

		public bool LoadConfig()
		{
			try
			{
				string configXml = File.ReadAllText(CONFIG_NAME);
				Config = Deserialize<ConfigData>(configXml);
				return true;
			}
			catch(Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SaveConfig), "", ex);
				Config = GetDefaultConfig();
				return false;
			}
		}

		public bool SaveConfig()
		{
			try
			{
				string configXml = SerializeObject<ConfigData>(Config);
				File.WriteAllText(CONFIG_NAME, configXml);
				return true;
			}
			catch(Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SaveConfig), "", ex);
				return false;
			}
		}

		public static string SerializeObject<T>(T objectToSerialize)
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var reader = new StreamReader(memoryStream))
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(T));
					serializer.WriteObject(memoryStream, objectToSerialize);
					memoryStream.Position = 0;
					var readToEnd = reader.ReadToEnd();
					return readToEnd;
				}
			}
		}

		public static T Deserialize<T>(string xml)
		{
			using (Stream stream = new MemoryStream())
			{
				byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
				stream.Write(data, 0, data.Length);
				stream.Position = 0;
				DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
				return (T)deserializer.ReadObject(stream);
			}
		}
	}
}
