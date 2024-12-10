namespace WinTlx
{
	static class Constants
	{
		public const string PROGRAM_NAME = "WinTlx";

		public const bool BETA = false;

		public const string APP_CODE = "wt";

		public const string CONSOLE_LOG = "wintlx_console.log";
		public const string DEBUG_LOG = "wintlx_debug.log";
		public const string BINARY_LOG = "wintlx_console.bin";
		public const string SEARCH_HISTORY = "wintlx_search.txt";
		public const int SEARCH_HISTORY_LENGTH = 20;

		public const string DEFAULT_LANGUAGE = "EN";

		public const int CENTRALEX_PORT = 49491;

		public const int TCPCLIENT_TIMEOUT = 5000;

		public const string DEFAULT_ANSWERBACK = "wintlx";
		public const int DEFAULT_IDLE_TIMEOUT = 120; // disconnect after 2 minutes without communication
		public const int DEFAULT_REMOTE_BUFFER_SIZE = 16; // byte
		public const int DEFAULT_OUTPUT_SPEED = 50; // baud
		public const int DEFAULT_INCOMING_PORT = 134;

		//public const int ITELIX_REMOTEBUFFER_SIZE = 16; // 16 characters
		public const int ITELIX_SENDBUFFER_SIZE = 16; // 16 characters

		public const int WAIT_BEFORE_SEND_MSEC = 100; // 0.1 seconds
		public const int WAIT_BEFORE_SEND_ACK = 16; // 16 characters

		public const int SCHEDULER_MAX_RETRIES = 3; // 3 retries
		public const int SCHEDULER_RETRY_DELAY = 30000; // wait 30 secondes before retry
	}
}
