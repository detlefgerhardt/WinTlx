using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace WinTlx.Tools
{
	internal class TcpTester
	{
		private string address = "itelex.srvdns.de";
		private int port = 40015;
		int connTimeMs = 1;

		byte[] data;
		string dataStr =
			"16 03 01 00 EE 01 00 00 " +
			"EA 03 03 57 88 E6 D9 CF " +
			"96 61 80 26 3B 21 54 EC " +
			"86 C3 B2 FF E6 C0 D5 51 " +
			"B2 FC CE 12 67 18 4E 8E " +
			"60 4A 11 20 F2 60 E2 3C " +
			"9B 1C D7 49 B0 FF 3C 07 " +
			"7C 7B 59 98 84 93 55 28 " +
			"F3 17 7A BB 2D DA D1 57 " +
			"94 FA 92 9A 00 26 CC A8 " +
			"CC A9 C0 2F C0 30 C0 2B " +
			"C0 2C C0 13 C0 09 C0 14 " +
			"C0 0A 00 9C 00 9D 00 2F " +
			"00 35 C0 12 00 0A 13 03 " +
			"13 01 13 02 01 00 00 7B " +
			"00 05 00 05 01 00 00 00 " +
			"00 00 0A 00 0A 00 08 00 " +
			"1D 00 17 00 18 00 19 00 " +
			"0B 00 02 01 00 00 0D 00 " +
			"1A 00 18 08 04 04 03 08 " +
			"07 08 05 08 06 04 01 05 " +
			"01 06 01 05 03 06 03 02 " +
			"01 02 03 FF 01 00 01 00 " +
			"00 12 00 00 00 2B 00 09 " +
			"08 03 04 03 03 03 02 03 " +
			"01 00 33 00 26 00 24 00 " +
			"1D 00 20 93 62 51 47 28 " +
			"04 9C D5 79 4E CB 07 31 " +
			"2E 4F 1E 46 0B 71 88 D6 " +
			"FA 50 07 3D 0A E1 8A D7 " +
			"CF 1A 4F";


		private readonly System.Timers.Timer timer;

		public TcpTester()
		{
			SetData();

			timer = new System.Timers.Timer(1000 * 10);
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
		}

		private void SetData()
		{
			List<byte> dataList = new List<byte>();
			string[] parts = dataStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in parts)
			{
				byte val = (byte)Convert.ToInt32(s, 16);
				dataList.Add(val);
			}
			data = dataList.ToArray();
		}

		private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			bool ok = await TcpTest(connTimeMs);
			//if (ok) connTimeMs += 1000;
		}

		public async Task<bool> TcpTest(int connTime)
		{
			return await Task.Run(() =>
			{
				TcpClient[] tcpClient = new TcpClient[3];

				for (int i = 0; i < 3; i++)
				{
					Debug.WriteLine($"connect {i} {connTime}");

					tcpClient[i] = new TcpClient();
					tcpClient[i].ReceiveTimeout = 2000;
					if (!tcpClient[i].ConnectAsync(address, port).Wait(2000)) continue;

					Debug.WriteLine($"write {i}");
					NetworkStream stream = tcpClient[i].GetStream();
					stream.Write(data, 0, data.Length);
				}

				TickTimer wait = new TickTimer();
				while (!wait.IsElapsedMilliseconds(connTime))
				{
					Thread.Sleep(1);
				}

				for (int i = 0; i < 3; i++)
				{
					tcpClient[i].Close();
					Debug.WriteLine($"disconnect {i}");
				}
				return true;
			});
		}
	}
}
