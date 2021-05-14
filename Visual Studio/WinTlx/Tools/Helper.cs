using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace WinTlx
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
			//string buildTime = ConfigurationManager.AppSettings.Get("builddate") + " Debug";
#else
			// show only date in release version
			string buildTime = Properties.Resources.BuildDate.Trim(new char[] { '\n', '\r' });
			buildTime = buildTime.Substring(0, 10);
#endif
			return $"{Constants.PROGRAM_NAME}  V{Application.ProductVersion}  (Build={buildTime})";
		}

		public static string GetVersionStr()
		{
			return Application.ProductVersion;
		}

		public static string GetExePath()
		{
			return Application.StartupPath;
		}

		public static DateTime? BuildTime()
		{
			//string dateStr = ConfigurationManager.AppSettings.Get("builddate");
			string dateStr = Properties.Resources.BuildDate.Trim();
			if (!DateTime.TryParse(dateStr, out DateTime dt))
			{
				// invalid build time
				return null;
			}
			return dt;
		}

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
			}
			return list;
		}

		public static byte[] AddByte(byte[] arr, byte addByte)
		{
			if (arr == null)
			{
				return null;
			}
			byte[] newArr = new byte[arr.Length + 1];
			Buffer.BlockCopy(arr, 0, newArr, 0, arr.Length);
			newArr[arr.Length] = addByte;
			return newArr;
		}

		public static byte[] AddBytes(byte[] arr, byte[] addArr)
		{
			if (arr==null)
			{
				return null;
			}
			if (addArr==null)
			{
				return arr;
			}
			byte[] newArr = new byte[arr.Length + addArr.Length];
			Buffer.BlockCopy(arr, 0, newArr, 0, arr.Length);
			Buffer.BlockCopy(addArr, 0, newArr, arr.Length, addArr.Length);
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
			if (int.TryParse(dataStr, out int value))
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

		public static Point CenterForm(Form form, Rectangle parentPos)
		{
			int screenNr = GetScreenNr(parentPos);
			Rectangle sc = Screen.AllScreens[screenNr].WorkingArea;

			int x = sc.Left + (sc.Width - form.Width) / 2;
			int y = sc.Top + (sc.Height - form.Height) / 2;

			return new Point(x, y);
		}

		// Error if parentPos = Fullscreen
		public static int GetScreenNr(Rectangle parentPos)
		{
			int mx = parentPos.Left + (parentPos.Right - parentPos.Left) / 2;

			Screen[] screens = Screen.AllScreens;
			int screenNr = 1;
			for (int i = 0; i < screens.Length; i++)
			{
				Rectangle scrnBounds = screens[i].WorkingArea;
				if (mx >= scrnBounds.Left && mx <= scrnBounds.Left + scrnBounds.Width)
					screenNr = i;
			}
			return screenNr;
		}

		public static void PaintRuler(Graphics g, int screenWidth, float scale)
		{
			for (int i = 0; i < screenWidth + 3; i++)
			{
				float x = (float)(2 + i * scale);
				Pen pen = (i == screenWidth) ? new Pen(Color.Red, 2) : new Pen(Color.Black, 1);
				pen.StartCap = LineCap.Square;
				pen.EndCap = LineCap.Square;
				if (i % 10 == 0)
				{
					g.DrawLine(pen, x, 0, x, 10);
				}
				else
				{
					g.DrawLine(pen, x, 5, x, 10);
				}
			}
		}

		public static void SetToolTip(Control ctrl, string text)
		{
			ToolTip toolTip = (ToolTip)ctrl.Tag;
			if (toolTip != null)
			{
				toolTip.RemoveAll();
			}
			else
			{
				toolTip = new ToolTip()
				{
					AutoPopDelay = 5000,
					InitialDelay = 1000,
					ReshowDelay = 500,
					ShowAlways = true
				};
				ctrl.Tag = toolTip;
			}

			toolTip.SetToolTip(ctrl, text);
		}

		public static long MilliTicks()
		{
			return DateTime.Now.Ticks / 10000;
		}

		public static long MilliDiff(DateTime dt)
		{
			return (DateTime.Now - dt).Ticks / 10000;
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

		public static string SerializeObject2<T>(T objectToSerialize)
		{
			XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
			XmlWriterSettings writerSettings = new XmlWriterSettings
			{
				// Formatierung des XML -- Zeilenumbrüche
				Indent = true,
				Encoding = Encoding.UTF8
			};

			string xml;
			using (StringWriterUtf8 stringWriter = new StringWriterUtf8())
			{
				using (var xmlWriter = XmlWriter.Create(stringWriter, writerSettings))
				{
					xmlserializer.Serialize(xmlWriter, objectToSerialize);
					xml = stringWriter.ToString();
				}
			}
			return xml;
		}
	}

	public class StringWriterUtf8 : StringWriter
	{
		public override Encoding Encoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}
	}
}
