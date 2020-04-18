using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinTlx.Languages
{
	/// <summary>
	/// This class manages the language definition and change of language at runtime.
	/// On startup it looks for all *.lng files in the exe directory and loads them.
	/// The languages "en" and "de" are hardcoded and do not need a language file, but the hardcoded definition
	/// can be overwritten by a language file.
	/// </summary>

	class LanguageManager
	{
		private const string TAG = nameof(LanguageManager);

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static LanguageManager instance;

		public static LanguageManager Instance => instance ?? (instance = new LanguageManager());

		public delegate void LanguageChangedEventHandler();
		/// <summary>
		/// Language changed event
		/// </summary>
		public event LanguageChangedEventHandler LanguageChanged;

		public List<Language> LanguageList { get; private set; }

		private LanguageManager()
		{
			LanguageList = new List<Language>();
			LanguageList.Add(LanguageEnglish.GetLng());
			LanguageList.Add(LanguageDeutsch.GetLng());

#if DEBUG
			SaveLanguage(LanguageEnglish.GetLng());
			SaveLanguage(LanguageDeutsch.GetLng());
#endif
		}

		public Language CurrentLanguage { get; private set; }

		public Language DefaultLanguage => GetLanguage(Constants.DEFAULT_LANGUAGE);

		/// <summary>
		/// Get a list of all language keys
		/// </summary>
		/// <returns></returns>
		//public List<string> GetLanguageKeys()
		//{
		//	return (from l in LanguageList orderby l.Key select $"{l.Key} {l.DisplayName}").ToList();
		//}

		public string GetText(LngKeys textKey)
		{
			if (CurrentLanguage == null)
			{
				SetDefaultLanguage();
			}

			string keyStr = textKey.ToString();

			// get text from current language
			if (CurrentLanguage.Items.ContainsKey(textKey))
			{
				return (from l in CurrentLanguage.Items where l.Key == textKey select l.Value).FirstOrDefault();
			}

			// get text from default language
			if (DefaultLanguage.Items.ContainsKey(textKey))
			{
				return (from l in DefaultLanguage.Items where l.Key == textKey select l.Value).FirstOrDefault();
			}

			// get language text key
			return keyStr;
		}

		public void ChangeLanguage(string lngKey, bool force=false)
		{
			Language newLng = GetLanguage(lngKey);
			if (newLng != null)
			{
				CurrentLanguage = newLng;
			}
			else
			{
				SetDefaultLanguage();
			}
			Logging.Instance.Info(TAG, nameof(ChangeLanguage), $"language changed to {CurrentLanguage.Key} {CurrentLanguage.DisplayName}");

			LanguageChanged?.Invoke();
		}

		private void SetDefaultLanguage()
		{
			CurrentLanguage = GetLanguage(Constants.DEFAULT_LANGUAGE);
		}

		private Language GetLanguage(string lngKey)
		{
			return (from l in LanguageList where string.Compare(lngKey, l.Key, true) == 0 select l).FirstOrDefault();
		}

		/// <summary>
		/// Load all language files (*.lng) found in the exe directory, replace default language definitions (de/en)
		/// </summary>
		public void LoadAllLanguageFiles()
		{
			string path = Helper.GetExePath();
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			FileInfo[] files = dirInfo.GetFiles("*.lng");
			foreach(FileInfo file in files)
			{
				Language newLng = LoadLanguage(file.FullName);
				if (newLng==null)
				{
					// not a valid language file
					continue;
				}

				// replace or add
				Language oldLng = GetLanguage(newLng.Key);
				if (oldLng==null)
				{	// add
					LanguageList.Add(newLng);
				}
				else
				{
					// replace existing
					LanguageList.Remove(oldLng);
					LanguageList.Add(newLng);
				}
			}
		}

		public Language LoadLanguage(string filename)
		{
			const char REPLACE_CHAR = '\x01';

			try
			{
				Language language = new Language();
				string[] lines;
				try
				{
					lines = File.ReadAllLines(filename);
				}
				catch(Exception ex)
				{
					Logging.Instance.Error(TAG, nameof(LoadLanguage), $"Error loading {filename}", ex);
					return null;
				}
				for (int i=0; i<lines.Length; i++)
				{
					string line = lines[i];
					if (string.IsNullOrWhiteSpace(line))
					{
						// empty line
						continue;
					}
					line = line.Trim();
					if (line[0]==';')
					{
						// comment line
						continue;
					}

					line = ReplaceQuotedBlanks(line, REPLACE_CHAR);
					string[] words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (words.Length<2)
					{
						Logging.Instance.Error(TAG, nameof(LoadLanguage), $"File {filename}: error in line #{line} ({line})");
						return null;
					}
					string cmd = words[0].ToLower();
					string prm = words[1].Replace(REPLACE_CHAR, ' ');
					switch (words[0].ToLower())
					{
						case "key":
							language.Key = prm.ToLower();
							break;
						case "version":
							if (prm != "1")
							{
								Logging.Instance.Error(TAG, nameof(LoadLanguage), $"File {filename}: Version {prm} not supported");
								return null;
							}
							language.Version = prm;
							break;
						case "name":
							language.DisplayName = prm;
							break;
						case "text":
							if (words.Length<3)
							{
								Logging.Instance.Error(TAG, nameof(LoadLanguage), $"File {filename}: error in line #{line} ({line})");
							}
							LngKeys lngKey =Language.StringToLngKey(prm);
							if (lngKey==LngKeys.Invalid)
							{
								Logging.Instance.Error(TAG, nameof(LoadLanguage), $"File {filename}: invalid key {prm} in line #{line}");
							}
							string text = words[2].Replace(REPLACE_CHAR, ' ');
							language.Items.Add(lngKey, text);
							break;
					}
				}

				if (string.IsNullOrWhiteSpace(language.Key))
				{
					Logging.Instance.Error(TAG, nameof(LoadLanguage), $"File {filename}: missing key");
					return null;
				}
				if (string.IsNullOrWhiteSpace(language.DisplayName))
				{
					Logging.Instance.Error(TAG, nameof(LoadLanguage), $"File {filename}: missing name");
					return null;
				}
				if (language.Items.Count==0)
				{
					Logging.Instance.Error(TAG, nameof(LoadLanguage), $"File {filename}: not text items");
					return null;
				}
				return language;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, "LoadLanguage", $"language file {filename} not found", ex);
				return null;
			}
		}

		/// <summary>
		/// replace all spaces inside quotes with replaceChar
		/// </summary>
		/// <param name="line"></param>
		/// <param name="replaceChr"></param>
		/// <returns></returns>
		private string ReplaceQuotedBlanks(string line, char replaceChr)
		{
			string newLine = "";
			bool quote = false;
			for (int i=0; i<line.Length; i++)
			{
				char chr = line[i];
				if (!quote)
				{
					if (chr=='\"')
					{
						quote = true;
						continue;
					}
				}
				else
				{
					if (chr == '\"')
					{
						quote = false;
						continue;
					}
					if (chr==' ')
					{
						chr = replaceChr;
					}
				}
				newLine += chr;
			}
			return newLine;
		}

#if DEBUG
		public bool SaveLanguage(Language lng)
		{
			List<string> lines = new List<string>();
			lines.Add($"; {Constants.PROGRAM_NAME} language file");
			lines.Add($"version 1");
			lines.Add($"key \"{lng.Key}\"");
			lines.Add($"name \"{lng.DisplayName}\"");
			lines.Add($";");
			foreach(var item in lng.Items)
			{
				lines.Add($"text {item.Key} \"{item.Value}\"");
			}
			lines.Add($";");
			lines.Add($"; end of file");

			try
			{
				File.WriteAllLines($"{lng.Key}_{lng.DisplayName}.lng", lines);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
#endif
	}

}
