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

		public const string DEFAULT_KENNUNG = "12345 wintelex";
		public const int DEFAULT_INACTIVITY_TIMEOUT = 120; // disconnect after 2 minutes without communication
		public const int DEFAULT_INCOMING_PORT = 134;

		public const int WAIT_BEFORE_SEND_MSEC = 500; // 0.5 seconds
		public const int WAIT_BEFORE_SEND_ACK = 16; // 16 characters

	}
}
