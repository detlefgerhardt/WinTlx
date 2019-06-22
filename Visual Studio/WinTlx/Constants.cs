using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx
{
	static class Constants
	{
		public const string PROGRAM_NAME = "WinTlx";

		public const string DEBUG_LOG = "debug.log";

		public const string DEFAULT_LANGUAGE = "EN";

		public const string DEFAULT_ANSWERBACK = "12345 wintlx";
		public const int DEFAULT_INACTIVITY_TIMEOUT = 120; // disconnect after 2 minutes without communication
		public const int DEFAULT_INCOMING_PORT = 134;

		public const int WAIT_BEFORE_SEND_MSEC = 100; // 0.1 seconds
		public const int WAIT_BEFORE_SEND_ACK = 16; // 16 characters

		//public const string MSG_CONNECTED = "connected";
		//public const string MSG_DISCONNECTED = "disconnected";
		//public const string MSG_REJECT = "reject";
		//public const string MSG_ACTIVITY_TIMEOUT = "activity timeout";
		//public const string MSG_INCOMING_CONNECTION = "incoming connection from";
		//public const string MSG_SUBSCRIBE_SERVER_ERROR = "subscribe server error";
		//public const string MSG_INVALID_SUBSCRIBE_SERVER_DATA = "invalid subscribe server address or port";
		//public const string MSG_QUERY_RESULT = "member(s) found";
		//public const string MSG_NO_ADDRESS = "no address";
		//public const string MSG_INVALID_PORT = "invalid port";
		//public const string MSG_INVALID_EXTENSION_NUMBER = "invalid extension";
		//public const string MSG_CONNECTION_ERROR = "connection error";
		//public const string MSG_EYEBALL_CHAR_ACTIVE = "eyeball char mode active - start tape punch";
	}
}
