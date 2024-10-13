using System;
using System.IO;

namespace WinTlx
{
	public delegate void RecvLogEvent(object sender, LogArgs e);

	public enum LogTypes { None = 0, Fatal = 1, Error = 2, Warn = 3, Info = 4, Debug = 5 };

	[Serializable]
	class Logging
	{
		private const string TAG = nameof(Logging);

		public event RecvLogEvent RecvLog;

		public LogTypes LogLevel { get; set; }

		public string LogfilePath { get; set; }

		private object _lock = new object();

		private static Logging instance;

		public static Logging Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Logging();
				}
				return instance;
			}
		}

		private Logging()
		{
			LogLevel = LogTypes.Debug;
		}

		public void Debug(string section, string method, string text)
		{
			Log(LogTypes.Debug, section, method, text);
		}

		public void Info(string section, string method, string text)
		{
			Log(LogTypes.Info, section, method, text);
		}

		public void Warn(string section, string method, string text)
		{
			Log(LogTypes.Warn, section, method, text);
		}

		public void Error(string section, string method, string text)
		{
			Log(LogTypes.Error, section, method, text);
		}

		public void Error(string section, string method, string text, Exception ex=null)
		{
			if (ex!=null)
			{
				text += " " + ex.Message;
			}
			Log(LogTypes.Error, section, method, text);
		}

		public void Fatal(string section, string method, string text)
		{
			Log(LogTypes.Fatal, section, method, text);
		}

		public void Log(LogTypes logType, string section, string method, string text, bool show = true)
		{
			if (IsActiveLevel(logType))
			{
				AppendLog(logType, section, method, text);
				OnLog(new LogArgs(logType, section, method, text));
			}
		}

		public void OnLog(LogArgs e)
		{
			RecvLog?.Invoke(this, e);
		}

		private void AppendLog(LogTypes logType, string section, string method, string text)
		{
			lock(_lock)
			{
				string fullName = "";
				string prefix = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} {logType.ToString().PadRight(5)} [{section}]";
				string logStr = $"{prefix} [{method}] {text}\r\n";
				try
				{
					string path = string.IsNullOrWhiteSpace(LogfilePath) ? Helper.GetExePath() : LogfilePath;
					fullName = Path.Combine(path, Constants.DEBUG_LOG);
					File.AppendAllText(fullName, logStr);
				}
				catch
				{
					// try to log in program directory
					/*
					string newName = Path.Combine(Helper.GetExePath(), Constants.DEBUG_LOG);
					File.AppendAllText(newName, $"{prefix} [AppendLog] Error writing logfile to {fullName}\r\n");
					File.AppendAllText(newName, logStr);
					*/
				}
			}
		}

		public enum BinaryModes { Recv, Send }

		public void AppendBinary(byte[] data, BinaryModes mode)
		{
			if (mode==BinaryModes.Send)
			{
				// copy data and set bit 7
				byte[] sendData = new byte[data.Length];
				for(int i=0; i<data.Length; i++)
				{
					sendData[i] = (byte)(data[i] | 0x80);
				}
				data = sendData;
			}

			string fullName = "";
			try
			{
				string path = string.IsNullOrWhiteSpace(LogfilePath) ? Helper.GetExePath() : LogfilePath;
				fullName = Path.Combine(path, Constants.BINARY_LOG);
				AppendBinary(fullName, data);
			}
			catch(Exception ex)
			{
				Error(TAG, nameof(AppendBinary), $"Error writing binary data to {fullName}", ex);

				// try to log in program directory
				fullName = Path.Combine(Helper.GetExePath(), Constants.BINARY_LOG);
				AppendBinary(fullName, data);
			}
		}

		public void AppendBinary(string fullName, byte[] data)
		{
			using (var fileStream = new FileStream(fullName, FileMode.Append, FileAccess.Write, FileShare.None))
			using (var bw = new BinaryWriter(fileStream))
			{
				bw.Write(data);
			}
		}

		private bool IsActiveLevel(LogTypes current)
		{
			return (int)current <= (int)LogLevel;
		}
	}

	public class LogArgs : EventArgs
	{
		public LogTypes LogType { get; set; }

		public string Section { get; set; }

		public string Method { get; set; }

		public string Message { get; set; }

		public LogArgs(LogTypes logType, string section, string method, string msg)
		{
			LogType = logType;
			Section = section;
			Method = method;
			Message = msg;
		}
	}

}
