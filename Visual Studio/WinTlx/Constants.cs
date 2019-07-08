namespace WinTlx
{
	static class Constants
	{
		public const string PROGRAM_NAME = "WinTlx";

		public const string DEBUG_LOG = "debug.log";

		public const string DEFAULT_LANGUAGE = "EN";

		public const string DEFAULT_ANSWERBACK = "12345 wintlx";
		public const int DEFAULT_IDLE_TIMEOUT = 120; // disconnect after 2 minutes without communication
		public const int DEFAULT_INCOMING_PORT = 134;

		public const int WAIT_BEFORE_SEND_MSEC = 100; // 0.1 seconds
		public const int WAIT_BEFORE_SEND_ACK = 16; // 16 characters

		public const int SCHEDULER_MAX_RETRIES = 3; // 3 retries
		public const int SCHEDULER_RETRY_DELAY = 30000; // wait 30 secondes before retry
	}
}
