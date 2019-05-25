using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTelex
{
	static class Constants
	{
		public const string PROGRAM_NAME = "WinTelex";

		public const int INCOMING_PORT = 134;

		public const int EXPIRE_DAYS = 30;

		public const string SUBSCRIBE_SERVER = "sonnibs.no-ip.org";
		public const int SUBSCRIBE_SERVER_PORT = 11811;

		public const string DEFAULT_KENNUNG = "12345 wintelex";

		public const int WAIT_BEFORE_SEND_MSEC = 500; // 0.5 seconds
		public const int WAIT_BEFORE_SEND_ACK = 16; // 16 characters

		public const int TIMEOUT_SEC = 120; // disconnect after 2 minutes without communication
	}
}
