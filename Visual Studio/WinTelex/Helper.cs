using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace WinTelex
{
	static class Helper
	{
		/// <summary>
		/// get systems ticks in ms
		/// </summary>
		public static long GetTicksMs()
		{
			return DateTime.Now.Ticks / 10000;
		}

		public static string GetVersion()
		{
#if DEBUG
			// show date and time in debug version
			string buildTime = Properties.Resources.BuildDate.Trim(new char[] { '\n', '\r' }) + " Debug";
			//int? expireDays = ExpireDays();
			//string expireStr = expireDays != null ? $" expires in {expireDays} days" : "";
			string expireStr = "";
#else
			// show only date in release version
			string buildTime = Properties.Resources.BuildDate.Trim(new char[] { '\n', '\r' });
			buildTime = buildTime.Substring(0, 10);
			string expireStr = "";
#endif
			return $"{Constants.PROGRAM_NAME}  V{Application.ProductVersion}  (Build={buildTime}) {expireStr}";
		}

#if DEBUG
		public static bool IsExpired()
		{
			int? expireDays = ExpireDays();
			if (expireDays==null)
			{
				// invalid expire date
				return false;
			}
			return expireDays == 0;
		}

		public static int? ExpireDays()
		{
			DateTime? expireDate = ExpireDate();
			if (expireDate == null)
			{
				// invalid build date, no expire
				return null;
			}
			int expireDays = (int)Math.Ceiling(expireDate.Value.Subtract(DateTime.Now).TotalDays);
			if (expireDays < 0)
				expireDays = 0;
			return expireDays;
		}

		public static DateTime? ExpireDate()
		{
			DateTime? buildTime = BuildTime();
			if (buildTime == null)
			{
				// invalid build date
				return null;
			}
			return buildTime.Value.AddDays(Constants.EXPIRE_DAYS);
		}
		public static DateTime? BuildTime()
		{
			string dateStr = Properties.Resources.BuildDate.Trim();
			DateTime dt;
			if (!DateTime.TryParse(dateStr, out dt))
			{
				// invalid build time
				return null;
			}
			return dt;
		}
#endif

		public static List<string> DumpByteArray(byte[] buffer, int pos, int len = -1)
		{
			if (len == -1)
				len = buffer.Length;
			else if (pos + len > buffer.Length)
				len = buffer.Length - pos;

			List<string> list = new List<string>();
			int p = 0;
			while (p < len)
			{
				string l1 = "";
				string l2 = "";
				string line = $"{p:X3}: ";
				for (int x = 0; x < 8; x++)
				{
					if (!string.IsNullOrEmpty(l1))
						l1 += " ";
					if (p >= len)
					{
						l1 += "  ";
						continue;
					}
					else
					{
						byte b = buffer[pos + p];
						l1 += b.ToString("X2");
						l2 += b >= 32 && b < 127 ? (char)b : '.';
					}
					p++;
				}
				line += l1 + " " + l2;
				list.Add(line);
				Debug.WriteLine(line);
			}
			return list;
		}

		public static byte[] AddByte(byte[] arr, byte data)
		{
			byte[] newArr = new byte[arr.Length + 1];
			Buffer.BlockCopy(arr, 0, newArr, 0, arr.Length);
			newArr[arr.Length] = data;
			return newArr;
		}

		public static byte[] StringToByteArr(string str)
		{
			if (string.IsNullOrEmpty(str))
				return new byte[0];
			return Encoding.ASCII.GetBytes(str);
		}

		public static int? ToInt(string dataStr)
		{
			int value;
			if (int.TryParse(dataStr, out value))
				return value;
			else
				return null;
		}

		/// <summary>
		/// Helper method to determin if invoke required, if so will rerun method on correct thread.
		/// if not do nothing.
		/// </summary>
		/// <param name="c">Control that might require invoking</param>
		/// <param name="a">action to preform on control thread if so.</param>
		/// <returns>true if invoke required</returns>
		public static void ControlInvokeRequired(Control c, Action a)
		{
			if (c.InvokeRequired)
			{
				c.Invoke(new MethodInvoker(delegate { a(); }));
			}
			else
			{
				a();
			}
		}
	}
}
