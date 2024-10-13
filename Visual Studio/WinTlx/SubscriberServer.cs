using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WinTlx.Debugging;
using WinTlx.Languages;
using WinTlx.Tools;

namespace WinTlx
{
	class SubscriberServer
	{
		private const string TAG = nameof(SubscriberServer);

		public delegate void MessageEventHandler(string message, bool isTechMsg);
		public event MessageEventHandler Message;

		private const int TIMEOUT = 2000;

		private TcpClientWithTimeout _client = null;
		private TcpClient _tcpClient = null;
		private NetworkStream _stream = null;

		public ClientUpdateReply DoClientUpdate(string[] serverAddresses, int serverPort, int clientNumber, int clientPin, int clientPort)
		{
			if (!ConnectToAnyServer(serverAddresses, serverPort)) return null;
			ClientUpdateReply reply = SendClientUpdate(clientNumber, clientPin, clientPort);
			Disconnect();
			return reply;
		}

		public PeerQueryReply DoPeerQuery(string[] serverAddresses, int serverPort, string number)
		{
			if (!ConnectToAnyServer(serverAddresses, serverPort)) return null;
			PeerQueryReply reply = SendPeerQuery(number);
			Disconnect();
			return reply;
		}

		public PeerSearchReply DoPeerSearch(string[] serverAddresses, int serverPort, string name)
		{
			if (!ConnectToAnyServer(serverAddresses, serverPort)) return null;
			PeerSearchReply reply = SendPeerSearch(name);
			Disconnect();
			return reply;
		}

		private bool ConnectToAnyServer(string[] serverAddresses, int port)
		{
			foreach (string servAddr in serverAddresses)
			{
				if (!string.IsNullOrWhiteSpace(servAddr))
				{
					if (Connect(servAddr, port)) return true;
				}
			}
			return false;
		}

		private bool Connect(string address, int port)
		{
			try
			{
				_client = new TcpClientWithTimeout(address, port, TIMEOUT);
				_tcpClient = _client.Connect();
				_tcpClient.ReceiveTimeout = TIMEOUT;
				_stream = _tcpClient.GetStream();

				// check connection (work-around)
				PeerSearchReply reply = SendPeerSearch("abc");
				if (!reply.Valid)
				{
					string errStr = $"error in subscribe server communication {address}:{port}";
					Logging.Instance.Error(TAG, nameof(Connect), errStr);
					_stream?.Close();
					_tcpClient?.Close();
					return false;
				}
				return true;
			}
			catch(Exception ex)
			{
				string errStr = $"error connecting to subscribe server {address}:{port}";
				DebugManager.Instance.Write("{errStr}\r\n", DebugManager.Modes.Message);
				Logging.Instance.Error(TAG, nameof(Connect), errStr, ex);
				_stream?.Close();
				_tcpClient?.Close();
				_client = null;
				return false;
			}
		}

		private bool Disconnect()
		{
			_stream?.Close();
			_tcpClient?.Close();
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
		private PeerQueryReply SendPeerQuery(string number)
		{
			Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendPeerQuery), $"number='{number}'");

			PeerQueryReply reply = new PeerQueryReply();

			if (_client == null)
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
			if (!UInt32.TryParse(number, out uint num))
			{
				reply.Error = "invalid number";
				return reply;
			}

			byte[] sendData = new byte[2 + 5];
			sendData[0] = 0x03; // Peer_query
			sendData[1] = 0x05; // length
			byte[] numData = BitConverter.GetBytes(num);
			Buffer.BlockCopy(numData, 0, sendData, 2, 4);
			sendData[6] = 0x01; // version 1

			try
			{
				_stream.Write(sendData, 0, sendData.Length);
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error sending data to subscribe server", ex);
				reply.Error = "reply server error";
				return reply;
			}

			byte[] recvData = new byte[102];
			int recvLen;
			try
			{
				recvLen = _stream.Read(recvData, 0, recvData.Length);
			}
			catch (Exception ex)
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

			if (recvLen < 2 + 0x64)
			{
				Logging.Instance.Log(LogTypes.Error, TAG, nameof(SendPeerSearch), $"received data to short ({recvLen} bytes)");
				reply.Error = $"received data to short ({recvLen} bytes)";
				return reply;
			}

			if (recvData[1] != 0x64)
			{
				Logging.Instance.Log(LogTypes.Error, TAG, nameof(SendPeerSearch), $"invalid length value ({recvData[1]})");
				reply.Error = $"invalid length value ({recvData[1]})";
				return reply;
			}

			reply.Data = ByteArrayToPeerData(recvData, 2);

			reply.Valid = true;
			reply.Error = "ok";

			return reply;
		}

		/// <summary>
		/// Query for search string
		/// </summary>
		/// <param name="name"></param>
		/// <returns>search reply with list of peers</returns>
		private PeerSearchReply SendPeerSearch(string name)
		{
			Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendPeerSearch), $"name='{name}'");
			PeerSearchReply reply = new PeerSearchReply();

			if (_client == null)
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
				_stream.Write(sendData, 0, sendData.Length);
			}
			catch(Exception ex)
			{
				Message?.Invoke(LngText(LngKeys.Message_SubscribeServerError), false);
				DebugManager.Instance.Write("error sending data to subscribe server", DebugManager.Modes.Message);
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
					recvLen = _stream.Read(recvData, 0, recvData.Length);
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
					_stream.Write(ack, 0, ack.Length);
				}
				catch(Exception ex)
				{
					Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error sending data to subscribe server", ex);
					return null;
				}
			}

			list.Sort(new PeerQueryDataSorter(PeerQueryDataSorter.Sort.Number));
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

			PeerQueryData data = new PeerQueryData
			{
				Number = BitConverter.ToUInt32(bytes, offset).ToString(),
				LongName = Encoding.ASCII.GetString(bytes, offset + 4, 40).Trim(new char[] { '\x00' }),
				SpecialAttribute = BitConverter.ToUInt16(bytes, offset + 44),
				PeerType = bytes[offset + 46],
				HostName = Encoding.ASCII.GetString(bytes, offset + 47, 40).Trim(new char[] { '\x00' }),
				IpAddress = $"{bytes[offset + 87]}.{bytes[offset + 88]}.{bytes[offset + 89]}.{bytes[offset + 90]}",
				PortNumber = BitConverter.ToUInt16(bytes, offset + 91),
				ExtensionNumber = bytes[offset + 93],
				Pin = BitConverter.ToUInt16(bytes, offset + 94)
			};

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
		private ClientUpdateReply SendClientUpdate(int number, int pin, int port)
		{
			Logging.Instance.Log(LogTypes.Debug, TAG, nameof(SendClientUpdate), $"number='{number}'");
			ClientUpdateReply reply = new ClientUpdateReply();

			if (_client == null)
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
				_stream.Write(sendData, 0, sendData.Length);
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error sending data to subscribe server", ex);
				reply.Error = "reply server error (send)";
				return reply;
			}

			byte[] recvData = new byte[6];
			int recvLen = 0;
			try
			{
				recvLen = _stream.Read(recvData, 0, recvData.Length);
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SendPeerQuery), $"error receiving data from subscribe server", ex);
				reply.Error = "reply server error (recv)";
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

		public static string SelectIp4Addr(string host)
		{
			IPHostEntry hostEntry = null;
			if (IPAddress.TryParse(host, out _) == true) return host;

			try
			{
				hostEntry = Dns.GetHostEntry(host);
			}
			catch (Exception ex)
			{
				Logging.Instance.Warn(TAG, nameof(SelectIp4Addr), $"dns request failed, host={host} ex={ex}");
				return host;
			}

			if (hostEntry.AddressList == null || hostEntry.AddressList.Length == 0)
			{
				Logging.Instance.Warn(TAG, nameof(SelectIp4Addr), $"dns request failed, no hostEntry, host={host}");
				return host;
			}

			string ipv4Str = null;
			for (int i = 0; i < hostEntry.AddressList.Length; i++)
			{
				IPAddress ipAddr = hostEntry.AddressList[i];
				if (ipv4Str == null && ipAddr.AddressFamily.ToString() == ProtocolFamily.InterNetwork.ToString())
				{
					// ipv4 address
					ipv4Str = ipAddr.ToString();
				}
				Logging.Instance.Debug(TAG, nameof(SelectIp4Addr),
					$"{i + 1}: ipAddr={ipAddr} mapToIPv4={ipAddr.MapToIPv4()} addressFamily={ipAddr.AddressFamily}");
			}
			Logging.Instance.Debug(TAG, nameof(SelectIp4Addr), $"ipv4addr = {ipv4Str}");
			return ipv4Str;
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

	class PeerQueryDataSorter : IComparer<PeerQueryData>
	{
		public enum Sort { Number, Name }

		private readonly Sort _sort;

		public PeerQueryDataSorter(Sort sort)
		{
			_sort = sort;
		}

		public int Compare(PeerQueryData item1, PeerQueryData item2)
		{
			switch (_sort)
			{
				case Sort.Number:
					return item1.Number.CompareTo(item2.Number);
				case Sort.Name:
					return item1.LongName.CompareTo(item2.LongName);
			}
			return 0;
		}
	}
}
