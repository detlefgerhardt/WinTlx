using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using WinTlx.Languages;

namespace WinTlx
{
	class SubscriberServer
	{
		public delegate void MessageEventHandler(string message);
		public event MessageEventHandler Message;

		private const string TAG = nameof(SubscriberServer);

		private TcpClient client = null;
		private NetworkStream stream = null;

		public bool Connect(string address, int port)
		{
			try
			{
				client = new TcpClient(address, port);
				stream = client.GetStream();
				return true;
			}
			catch(Exception ex)
			{
				string errStr = $"error connecting to subscribe server {address}:{port}";
				Logging.Instance.Error(TAG, nameof(Connect), errStr, ex);
				Message?.Invoke(errStr);
				stream?.Close();
				client?.Close();
				client = null;
				return false;
			}
		}

		public bool Disconnect()
		{
			stream?.Close();
			client?.Close();
			return true;
		}

#if false
		public AsciiQueryResult PeerQueryAscii(string number)
		{
			Logging.Instance.Log(LogTypes.Debug, TAG, nameof(PeerQueryAscii), $"number='{number}'");

			if (client == null)
			{
				Logging.Instance.Error(TAG, nameof(PeerQueryAscii), "no server connection");
				return null;
			}

			if (string.IsNullOrEmpty(number))
			{
				return null;
			}
			number = number.Replace(" ", "");

			try
			{

				// query number
				byte[] data = Encoding.ASCII.GetBytes($"q{number}\r\n");
				stream.Write(data, 0, data.Length);

				data = new byte[4096];
				string responseData = string.Empty;

				Int32 bytes = stream.Read(data, 0, data.Length);
				responseData = Encoding.ASCII.GetString(data, 0, bytes);

				string[] responseList = responseData.Split(new string[] { "\r\n" }, StringSplitOptions.None);
				if (responseList.Length < 8)
					return null;
				if (responseList[0] != "ok")
					return null;
				if (responseList[7] != "+++")
					return null;

				QueryResult queryData = new QueryResult()
				{
					Number = responseList[1],
					Name = responseList[2],
					Type = GetValue(responseList[3]),
					HostName = responseList[4],
					Port = GetValue(responseList[5]),
					ExtensionNumber = GetValue(responseList[6]),
				};

				return queryData;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}
			finally
			{
				stream?.Close();
				client?.Close();
			}
		}
		private int? GetValue(string valStr)
		{
			int value;
			if (int.TryParse(valStr, out value))
			{
				return value;
			}
			return null;
		}
#endif

		/// <summary>
		/// Query for number
		/// </summary>
		/// <param name="number"></param>
		/// <returns>peer or null</returns>
		public PeerQueryReply SendPeerQuery(string number)
		{
			Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendPeerQuery), $"number='{number}'");

			PeerQueryReply reply = new PeerQueryReply();

			if (client==null)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), "no server connection");
				reply.Error = "no server connection";
				return reply;
			}

			if (string.IsNullOrEmpty(number))
			{
				reply.Error = "no query number";
				return reply;
			}

			// convert number to Int32
			number = number.Replace(" ", "");
			UInt32 num;
			if (!UInt32.TryParse(number, out num))
			{
				reply.Error = "invalid number";
				return reply;
			}

			byte[] sendData = new byte[2 + 5];
			sendData[0] = 0x03; // Peer_query
			sendData[1] = 0x05; // length
			byte[] numData = BitConverter.GetBytes(num);
			Buffer.BlockCopy(numData, 0, sendData, 2, 4);
			sendData[6] = 0x01; ; // version 1

			try
			{
				stream.Write(sendData, 0, sendData.Length);
			}
			catch(Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error sending data to subscribe server", ex);
				reply.Error = "reply server error";
				return reply;
			}

			byte[] recvData = new byte[102];
			int recvLen;
			try
			{
				recvLen = stream.Read(recvData, 0, recvData.Length);
			}
			catch(Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error receiving data from subscribe server", ex);
				reply.Error = "reply server error";
				return reply;
			}

			if (recvLen == 0)
			{
				reply.Error = $"no data received";
				return reply;
			}

			if (recvData[0] == 0x04)
			{
				// peer not found
				Logging.Instance.Log(LogTypes.Info, TAG, nameof(SendPeerSearch), $"peer not found");
				reply.Error = $"peer not found {number}";
				reply.Valid = true;
				return reply;
			}

			if (recvData[0] != 0x05)
			{
				// invalid packet
				Logging.Instance.Log(LogTypes.Error, TAG, nameof(SendPeerSearch), $"invalid packet id ({recvData[0]:X02})");
				reply.Error = $"invalid packet id ({recvData[0]:X02})";
				return reply;
			}

			if (recvLen < 2+0x64)
			{
				Logging.Instance.Log(LogTypes.Error, TAG, nameof(SendPeerSearch), $"received data to short ({recvLen} bytes)");
				reply.Error = $"received data to short ({recvLen} bytes)";
				return reply;
			}

			if (recvData[1]!=0x64)
			{
				Logging.Instance.Log(LogTypes.Error, TAG, nameof(SendPeerSearch), $"invalid length value ({recvData[1]})");
				reply.Error = $"invalid length value ({recvData[1]})";
				return reply;
			}

			reply.Data =  ByteArrayToPeerData(recvData, 2);

			reply.Valid = true;
			reply.Error = "ok";

			return reply;
		}

		/// <summary>
		/// Query for search string
		/// </summary>
		/// <param name="name"></param>
		/// <returns>search reply with list of peers</returns>
		public PeerSearchReply SendPeerSearch(string name)
		{
			Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendPeerSearch), $"name='{name}'");
			PeerSearchReply reply = new PeerSearchReply();

			if (client == null)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerSearch), "no server connection");
				reply.Error = "no server connection";
				return reply;
			}

			if (string.IsNullOrEmpty(name))
			{
				reply.Error = "no search name";
				return reply;
			}

			byte[] sendData = new byte[43];
			sendData[0] = 0x0A; // Peer_search
			sendData[1] = 0x29; // length
			sendData[2] = 0x01; ; // version 1
			byte[] txt = Encoding.ASCII.GetBytes(name);
			Buffer.BlockCopy(txt, 0, sendData, 3, txt.Length);
			try
			{
				stream.Write(sendData, 0, sendData.Length);
			}
			catch(Exception ex)
			{
				Message?.Invoke(LngText(LngKeys.Message_SubscribeServerError));
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error sending data to subscribe server", ex);
				reply.Valid = false;
				reply.Error = "reply server error";
				return reply;
			}

			byte[] ack = new byte[] { 0x08, 0x00 };
			List<PeerQueryData> list = new List<PeerQueryData>();
			while (true)
			{
				byte[] recvData = new byte[102];
				int recvLen = 0;
				try
				{
					recvLen = stream.Read(recvData, 0, recvData.Length);
				}
				catch(Exception ex)
				{
					Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error receiving data from subscribe server", ex);
					reply.Valid = false;
					reply.Error = "reply server error";
					return reply;
				}
				//Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendPeerSearch), $"recvLen={recvLen}");

				if (recvLen == 0)
				{
					Logging.Instance.Log(LogTypes.Error, TAG, nameof(SendPeerSearch), $"recvLen=0");
					reply.Error = $"no data received";
					return reply;
				}

				if (recvData[0] == 0x09)
				{
					// end of list
					break;
				}

				if (recvLen < 2 + 0x64)
				{
					Logging.Instance.Log(LogTypes.Warn, TAG, nameof(SendPeerSearch), $"received data to short ({recvLen} bytes)");
					reply.Error = $"received data to short ({recvLen} bytes)";
					continue;
				}

				if (recvData[1] != 0x64)
				{
					Logging.Instance.Log(LogTypes.Warn, TAG, nameof(SendPeerSearch), $"invalid length value ({recvData[1]})");
					reply.Error = $"invalid length value ({recvData[1]})";
					continue;
				}

				PeerQueryData data = ByteArrayToPeerData(recvData, 2);
				Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendPeerSearch), $"found {data}");

				list.Add(data);

				// send ack
				try
				{
					stream.Write(ack, 0, ack.Length);
				}
				catch(Exception ex)
				{
					Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error sending data to subscribe server", ex);
					return null;
				}
			}

			reply.List = list.ToArray();
			reply.Valid = true;
			reply.Error = "ok";

			return reply;
		}

		private PeerQueryData ByteArrayToPeerData(byte[] bytes, int offset)
		{
			if (offset + 100 > bytes.Length)
			{
				return null;
			}

			PeerQueryData data = new PeerQueryData();
			data.Number = BitConverter.ToUInt32(bytes, offset).ToString();
			data.LongName = Encoding.ASCII.GetString(bytes, offset+4, 40).Trim(new char[] { '\x00' });
			data.SpecialAttribute = BitConverter.ToUInt16(bytes, offset+44);
			data.PeerType = bytes[offset+46];
			data.HostName = Encoding.ASCII.GetString(bytes, offset+47, 40).Trim(new char[] { '\x00' });
			data.IpAddress = $"{bytes[offset+87]}.{bytes[offset + 88]}.{bytes[offset + 89]}.{bytes[offset + 90]}";
			data.PortNumber = BitConverter.ToUInt16(bytes, offset + 91);
			data.ExtensionNumber = bytes[offset + 93];
			data.Pin = BitConverter.ToUInt16(bytes, offset + 94);

			UInt32 timestamp = BitConverter.ToUInt32(bytes, offset + 96);
			DateTime dt = new DateTime(1900, 1, 1, 0, 0, 0, 0);
			data.LastChange = dt.AddSeconds(timestamp);

			return data;
		}

		/// <summary>
		/// Update own ip-number
		/// </summary>
		/// <param name="number"></param>
		/// <returns>peer or null</returns>
		public ClientUpdateReply SendClientUpdate(int number, int pin, int port)
		{
			Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendClientUpdate), $"number='{number}'");
			ClientUpdateReply reply = new ClientUpdateReply();

			if (client == null)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), "no server connection");
				reply.Error = "no server connection";
				return reply;
			}

			byte[] sendData = new byte[2 + 8];
			sendData[0] = 0x01; // packet type: client update
			sendData[1] = 0x08; // length
			byte[] data = BitConverter.GetBytes((UInt32)number);
			Buffer.BlockCopy(data, 0, sendData, 2, 4);
			data = BitConverter.GetBytes((UInt16)pin);
			Buffer.BlockCopy(data, 0, sendData, 6, 2);
			data = BitConverter.GetBytes((UInt16)port);
			Buffer.BlockCopy(data, 0, sendData, 8, 2);

			try
			{
				stream.Write(sendData, 0, sendData.Length);
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error sending data to subscribe server", ex);
				reply.Error = "reply server error";
				return reply;
			}

			byte[] recvData = new byte[6];
			int recvLen = 0;
			try
			{
				recvLen = stream.Read(recvData, 0, recvData.Length);
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error receiving data from subscribe server", ex);
				reply.Error = "reply server error";
				return reply;
			}

			if (recvLen != 6)
			{
				Logging.Instance.Log(LogTypes.Info, TAG, nameof(SendPeerSearch), $"wrong reply packet size ({recvLen})");
				reply.Error = "error";
				return reply;
			}

			if (recvData[0] != 0x02)
			{
				// peer not found
				Logging.Instance.Log(LogTypes.Info, TAG, nameof(SendPeerSearch), $"wrong reply packet, type={recvData[0]:X2}");
				reply.Error = "error";
				return reply;
			}

			reply.Success = true;
			reply.IpAddress = $"{recvData[2]}.{recvData[3]}.{recvData[4]}.{recvData[5]}";
			reply.Error = "ok";

			return reply;
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}
	}

	/*
	class AsciiQueryResult
	{
		public string Number { get; set; }
		public string Name { get; set; }
		public int? Type { get; set; }
		public string HostName { get; set; }
		public int? Port { get; set; }
		public int? ExtensionNumber { get; set; }

		public override string ToString()
		{
			return $"{Number} '{Name}' {Type} {HostName} {Port} {ExtensionNumber}";
		}
	}
	*/

	class ClientUpdateReply
	{
		public bool Success { get; set; } = false;
		public string Error { get; set; }
		public string IpAddress { get; set; }
	}

	class PeerQueryReply
	{
		public bool Valid { get; set; } = false;
		public string Error { get; set; }
		public PeerQueryData Data { get; set; }
	}

	class PeerSearchReply
	{
		public bool Valid { get; set; } = false;
		public string Error { get; set; }
		public PeerQueryData[] List { get; set; }
	}

	class PeerQueryData
	{
		public string Number { get; set; }
		public string LongName { get; set; }
		public UInt16 SpecialAttribute { get; set; }
		public int PeerType { get; set; }
		public string IpAddress { get; set; }
		public string HostName { get; set; }
		public int PortNumber { get; set; }
		public int ExtensionNumber { get; set; }
		public int Pin { get; set; }
		public DateTime LastChange { get; set; }

		public string Address => !string.IsNullOrEmpty(HostName) ? HostName : IpAddress;

		public string Display => $"{Number} {LongName}";

		public override string ToString()
		{
			return $"{Number} {LongName} {PeerType} {Address} {PortNumber} {ExtensionNumber}";
		}
	}
}
