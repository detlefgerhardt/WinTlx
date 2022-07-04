using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.TextEditor;

namespace WinTlx
{
	class BufferManager
	{
		private const string TAG = nameof(BufferManager);

		private readonly ItelexProtocol _itelex;

		private readonly ConfigData _configData;

		public delegate void UpdateSendEventHandler();
		public event UpdateSendEventHandler UpdateSend;

		public delegate void OutputEventHandler(ScreenChar screenChar);
		public event OutputEventHandler Output;

		public delegate void SendSpeedTriggerEventHandler();
		public event SendSpeedTriggerEventHandler SendCharTrigger;

		/// <summary>
		/// Timer for send buffer
		/// </summary>
		private bool _sendTimerActive;
		private readonly System.Timers.Timer _sendTimer;
		private readonly Queue<char> _sendBuffer;
		private object _sendBufferLock = new object();

		/// <summary>
		/// Timer for screen output buffer
		/// </summary>
		private readonly System.Timers.Timer _localOutputTimer;
		private bool _localOutputTimerActive;
		private readonly Queue<ScreenChar> _localOutputBuffer;
		private object _localOutputBufferLock = new object();
		private string _lastLocalOutputChars;

		public int LocalOutputBufferCount => _localOutputBuffer.Count;

		private int _localOutputSpeed;
		public int LocalOutputSpeed => _localOutputSpeed;

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static BufferManager instance;

		public static BufferManager Instance => instance ?? (instance = new BufferManager());

		private BufferManager()
		{
			_itelex = ItelexProtocol.Instance;
			_itelex.Message += Itelex_MessageHandler;
			_itelex.Received += Itelex_ReceivedHandler;
			//private void MessageHandler(string asciiText)
			//{
			//	ShowLocalMessage(asciiText);
			//}


			_configData = ConfigManager.Instance.Config;

			//_textEditorManager = TextEditorManager.Instance;
			//_textEditorManager.Send += TextEditor_Send;
			//_textEditorManager.ShowMsg += TextEditor_ShowMsg;

			_lastLocalOutputChars = "\r\n";

			_sendBuffer = new Queue<char>();
			_sendTimerActive = false;
			_sendTimer = new System.Timers.Timer(1);
			_sendTimer.Elapsed += SendTimer_Elapsed;
			_sendTimer.Start();

			_localOutputTimerActive = false;
			_localOutputBuffer = new Queue<ScreenChar>();
			_localOutputTimer = new System.Timers.Timer(10);
			_localOutputTimer.Elapsed += LocalOutputTimer_Elapsed;
		}

		/*
		private void TextEditor_Send(string asciiText)
		{
			LocalOutputSend(asciiText);
		}

		private void TextEditor_ShowMsg(string asciiText)
		{
			LocalOutputMsg(asciiText);
		}
		*/

		public void SendBufferEnqueueChr(char asciiChr)
		{
			SendBufferEnqueueString(asciiChr.ToString());
		}

		public void SendBufferEnqueueString(string asciiStr)
		{
			asciiStr = CodeManager.AsciiStringToTelex(asciiStr, _configData.CodeSet);
			foreach (char chr in asciiStr)
			{
				lock (_sendBufferLock)
				{
					_sendBuffer.Enqueue(chr);
				}
			}
		}

		//public void UpdateSendCount(int count)
		//{
		//	_itelex.AddPrintedCharCount(count);
		//}

		public int SendBufferCount => _sendBuffer.Count;

		public void WaitSendBufferEmpty()
		{
			while (true)
			{
				Thread.Sleep(100);
				if (_itelex.IdleTimerMs > 30 * 1000)
				{
					//Debug.WriteLine($"WaitSendBufferEmpty timeout {_itelex.IdleTimerMs}");
					return;
				}
				if (SendBufferCount == 0 && _itelex.SendBufferCount == 0 && _itelex.GetRemoteBufferCount() == 0) return;
			}
		}

		public async Task WaitSendBufferEmptyAsync()
		{
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			while (true)
			{
				await Task.Delay(100);
				if (_itelex.IdleTimerMs > 30 * 1000)
				{
					//Debug.WriteLine($"WaitSendBufferEmpty timeout {_itelex.IdleTimerMs}");
					return;
				}
				if (SendBufferCount == 0 && _itelex.SendBufferCount == 0 && _itelex.GetRemoteBufferCount() == 0) return;
			}
		}

		private void SendTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_sendBuffer.Count == 0 || _sendTimerActive) return;

			try
			{
				_sendTimerActive = true;
				// send one character
				char? chr = null;
				lock (_sendBufferLock)
				{
					if (_itelex.GetSendBufferCount() < 5 && _sendBuffer.Count > 0)
					{
						chr = _sendBuffer.Dequeue();
					}
				}
				if (chr.HasValue)
				{
					_itelex.SendAsciiChar(chr.Value);
				}


				/*
				if (_configData.OutputSpeed == 0)
				{   // max. speed
					while (_itelex.GetSendBufferCount() < 5 && _sendBuffer.Count > 0)
					{
						lock (_sendBufferLock)
						{
							_itelex.SendAsciiChar(_sendBuffer.Dequeue());
						}
					}
				}
				else
				{
					// send one character
					if (_itelex.GetSendBufferCount() < 5 && _sendBuffer.Count > 0)
					{
						_itelex.SendAsciiChar(_sendBuffer.Dequeue());
					}
				}
				*/

				UpdateSend?.Invoke();
			}
			finally
			{
				_sendTimerActive = false;
			}
		}

		public void SetLocalOutputSpeed(int charSec)
		{
			_localOutputSpeed = charSec;
			_localOutputTimer.Stop();
			if (charSec > 0)
			{
				// 5 data bits + 1 start bit + 1.5 stop bits = 7.5
				_localOutputTimer.Interval = 1000D / charSec * 7.5D;
			}
			else
			{
				// default speed: 10 ms
				_localOutputTimer.Interval = 10; // default output speed = 10 ms
			}
			_localOutputTimer.Start();

			if (charSec<= 50 || charSec == 0)
			{
				_itelex.SetAckTimerInterval(0); // default
			}
			else
			{
				// increase ack send interval to prevent stuttering
				_itelex.SetAckTimerInterval(1000); // 1000 ms
			}

			/*
			_sendTimer.Stop();
			if (charSec > 0)
			{
				// 5 data bits + 1 start bit + 1.5 stop bits = 7.5
				_sendTimer.Interval = 1000D / charSec * 7.5D;
			}
			else
			{
				// default speed: 10 ms
				_sendTimer.Interval = 100;
			}
			_sendTimer.Start();
			*/
		}

		/**
		 * Clear local output buffer, keep Messages
		 * 
		 */
		public void LocalOutputBufferClear(bool clearMessages)
		{
			if (clearMessages)
			{
				_localOutputBuffer.Clear();
				return;
			}

			List<ScreenChar> tmpBuf = new List<ScreenChar>();
			while (_localOutputBuffer.Count > 0)
			{
				//lock (_localOutputBufferLock)
				{
					ScreenChar scrChr = _localOutputBuffer.Dequeue();
					if (scrChr.Attr == CharAttributes.Message) tmpBuf.Add(scrChr);
				}
			}
			_localOutputBuffer.Clear();
			foreach (ScreenChar scrChr in tmpBuf)
			{
				_localOutputBuffer.Enqueue(scrChr);
			}
		}

		public async Task WaitLocalOutpuBufferEmpty()
		{
			while(true)
			{
				await Task.Delay(100);
				if (_localOutputBuffer.Count == 0) return;
			}
		}

		private void Itelex_MessageHandler(string msg)
		{
			LocalOutputMsg(msg);
		}

		private void Itelex_ReceivedHandler(string asciiText)
		{
			LocalOutputRecv(asciiText);
			/*
			//Debug.WriteLine(asciiText);
			for (int i = 0; i < asciiText.Length; i++)
			{
				switch (asciiText[i])
				{
					case CodeManager.ASC_BEL:
						SystemSounds.Beep.Play();
						LocalOutputRecv(asciiText.ToString());
						return;
					case CodeManager.ASC_WRU:
						//SendHereIs();
						_bufferManager.LocalOutputRecv(asciiText.ToString());
						return;
				}
				_bufferManager.LocalOutputRecv(asciiText[i].ToString());
			}
			*/
		}

		public void UpdateLastLocalOutputChars(char chr)
		{
			_lastLocalOutputChars += chr;
			_lastLocalOutputChars = _lastLocalOutputChars.Substring(_lastLocalOutputChars.Length - 2, 2);
		}

		public void LocalOutputMsg(string msg)
		{
			LocalOutputBufferClear(false);

			if (_lastLocalOutputChars != "\r\n")
			{
				msg = "\r\n" + msg;
			}
			msg += "\r\n";

			foreach(char chr in msg)
			{
				LocalOutputEnqueue(char.ToUpper(chr), CharAttributes.Message, 0);
				//ScreenChar screenChr = new ScreenChar(char.ToUpper(chr), CharAttributes.Message, 0);
				//Output?.Invoke(screenChr);
			}
		}

		public void LocalOutputSend(string msg)
		{
			foreach (char chr in msg)
			{
				LocalOutputEnqueue(chr, CharAttributes.Send, 0);
			}
		}

		public void LocalOutputRecv(string msg)
		{
			if (string.IsNullOrEmpty(msg))
			{
				// empty char with ackCount = 1
				LocalOutputEnqueue((char)0, CharAttributes.RecvEmpty, 1);
				return;
			}

			for (int i=0; i<msg.Length-2; i++)
			{
				LocalOutputEnqueue(msg[i], CharAttributes.Recv, 0);
			}

			// if a baudot char was converted into multi ascii chars, put the count of ack chars into the last ascii chars
			LocalOutputEnqueue(msg[msg.Length-1], CharAttributes.Recv, 1);
		}

		public void LocalOutputEnqueue(char chr, CharAttributes attr, int ackCount)
		{
			lock (_localOutputBufferLock)
			{ 
				_localOutputBuffer.Enqueue(new ScreenChar(chr, attr, ackCount));
				if (attr == CharAttributes.Recv)
				{
					Debug.WriteLine($"ScreenChar: {chr} {ackCount}");
				}
			}
		}

		//public int LocalOutputBufferFree
		//{
		//	get
		//	{
		//		return _localOutputBuffer.Count;
		//	}
		//}

		private void LocalOutputTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_localOutputTimerActive) return;
			_localOutputTimerActive = true;

			try
			{
				//Debug.WriteLine($"_localOutputBuffer.Count={_localOutputBuffer.Count}");
				if (_localOutputBuffer.Count > 0)
				{
					lock (_localOutputBufferLock)
					{
						do
						{
							ScreenChar scrChr = _localOutputBuffer.Dequeue();
							if (scrChr.Attr != CharAttributes.RecvEmpty) Output?.Invoke(scrChr);
							if (scrChr.AckCount > 0) _itelex.AddPrintedCharCount(scrChr.AckCount);
						}
						while (_localOutputBuffer.Count > 0 && _localOutputBuffer.Peek().Attr == CharAttributes.Message);
					}
				}
				if (_configData.OutputSpeed > 0)
				{
					SendCharTrigger?.Invoke();
				}
			}
			finally
			{
				_localOutputTimerActive = false;
			}
		}

	}
}
