namespace WinTlx.TapePunch
{
	class PunchLine
	{
		public byte Code { get; set; }

		public string Text { get; set; }

		public PunchLine(byte code, string text)
		{
			Code = code;
			Text = text;
		}
	}
}
