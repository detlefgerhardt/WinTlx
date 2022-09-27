using System;

namespace WinTlx
{
	public class ItelexPacket
	{
		public int Command { get; set; }

		public ItelexCommands CommandType => (ItelexCommands)Command;

		public byte[] Data { get; set; }

		public int Len => Data == null ? 0 : Data.Length;

		public ItelexPacket() { }

		public ItelexPacket(byte[] buffer)
		{
			Command = buffer[0];
			int len = buffer[1];
			if (len > 0)
			{
				Data = new byte[len];
				Buffer.BlockCopy(buffer, 2, Data, 0, len);
			}
			else
			{
				Data = null;
			}
		}

		public string GetDebugData()
		{
			string debStr = "";
			for (int i = 0; i < Len; i++)
			{
				debStr += $" {Data[i]:X2}";
			}
			return debStr.Trim();
		}

		public string GetDebugPacket()
		{
			return $"{Command:X02} {Len:X02} " + GetDebugData();
		}

		public override string ToString()
		{
			return $"{CommandType} {Len}: {GetDebugData()}";
		}

	}
}
