using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Debugging;
using WinTlx.TextEditor;

namespace WinTlx
{
	class BufferManager
	{
		private const string TAG = nameof(BufferManager);

		private readonly ItelexProtocol _itelex;

		private readonly SubscriberServer _subscriberServer;

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
		private readonly ConcurrentQueue<char> _sendBuffer;
		private object _sendBufferLock = new object();

		private bool _clearLocalBufferFlag = false;

		/// <summary>
		/// Timer for screen output buffer
		/// </summary>
		private readonly System.Timers.Timer _localOutputTimer;
		private bool _localOutputTimerActive;
		private readonly ConcurrentQueue<ScreenChar> _localOutputBuffer;
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
			_itelex.Dropped += Itelex_DroppedHandler;
			//private void MessageHandler(string asciiText)
			//{
			//	ShowLocalMessage(asciiText);
			//}

			_subscriberServer = new SubscriberServer();
			_subscriberServer.Message += Itelex_MessageHandler;

			_configData = ConfigManager.Instance.Config;

			//_textEditorManager = TextEditorManager.Instance;
			//_textEditorManager.Send += TextEditor_Send;
			//_textEditorManager.ShowMsg += TextEditor_ShowMsg;

			_lastLocalOutputChars = "\r\n";

			_sendBuffer = new ConcurrentQueue<char>();
			_sendTimerActive = false;
			_sendTimer = new System.Timers.Timer(1);
			_sendTimer.Elapsed += SendTimer_Elapsed;
			_sendTimer.Start();

			_localOutputTimerActive = false;
			_localOutputBuffer = new ConcurrentQueue<ScreenChar>();
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
				_sendBuffer.Enqueue(chr);
			}
		}

		public int SendBufferCount => _sendBuffer.Count;

		public void WaitSendBufferEmpty()
		{
			while (true)
			{
				Thread.Sleep(100);
				if (_itelex.IdleTimerMs > 30 * 1000) return;
				if (SendBufferCount == 0 && _itelex.SendBufferCount == 0 && _itelex.GetRemoteBufferCount() == 0) return;
			}
		}

		public async Task WaitSendBufferEmptyAsync()
		{
			while (true)
			{
				await Task.Delay(100);
				if (_itelex.IdleTimerMs > 30 * 1000) return;
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
				if (_itelex.GetSendBufferCount() < 5 && _sendBuffer.Count > 0)
				{
					if (_sendBuffer.TryDequeue(out char chr))
					{
						_itelex.SendAsciiChar(chr);
					}
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

			if (charSec <= 50 || charSec == 0)
			{
				_itelex.SetAckTimerInterval(0); // set to default default
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
		/*
		public void LocalOutputBufferClear_old(bool clearMessages)
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
		*/

		/**
		 * Clear local output buffer, keep Messages
		 * 
		 */
		public void LocalOutputBufferClear(bool clearMessages)
		{
			var timeout = TimeSpan.FromMilliseconds(1000);
			bool lockTaken = false;
			_clearLocalBufferFlag = true;

			try
			{
				//lock(_localOutputBufferLock)
				Monitor.TryEnter(_localOutputBufferLock, timeout, ref lockTaken);
				if (lockTaken)
				{
					if (clearMessages)
					{
						while (!_localOutputBuffer.IsEmpty)
						{
							_localOutputBuffer.TryDequeue(out _);
						}
						return;
					}

					List<ScreenChar> tmpBuf = new List<ScreenChar>();
					while (_localOutputBuffer.Count > 0)
					{
						if (_localOutputBuffer.TryDequeue(out ScreenChar scrChr))
						{
							if (scrChr.Attr == CharAttributes.Message || scrChr.Attr == CharAttributes.TechMessage) tmpBuf.Add(scrChr);
						}
					}

					while (!_localOutputBuffer.IsEmpty)
					{
						_localOutputBuffer.TryDequeue(out _);
					}

					foreach (ScreenChar scrChr in tmpBuf)
					{
						_localOutputBuffer.Enqueue(scrChr);
					}
				}
			}
			finally
			{
				if (lockTaken) Monitor.Exit(_localOutputBufferLock);
				_clearLocalBufferFlag = false;
			}
		}

		public void SendBufferClear()
		{
			while (_sendBuffer.TryDequeue(out _))
			{
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

		private void Itelex_MessageHandler(string msg, bool isTechMsg)
		{
			LocalOutputMessage(msg, isTechMsg);
		}

		private void Itelex_ReceivedHandler(string asciiText)
		{
			LocalOutputRecv(asciiText);
		}

		private void Itelex_DroppedHandler(string rejectReason)
		{
			SendBufferClear();
			LocalOutputBufferClear(false);
		}

		public void UpdateLastLocalOutputChars(char chr)
		{
			_lastLocalOutputChars += chr;
			_lastLocalOutputChars = _lastLocalOutputChars.Substring(_lastLocalOutputChars.Length - 2, 2);
		}

		public void LocalOutputMessage(string msg, string techMsg, bool showMsgAllways, bool showDebug)
		{
			if (!_configData.ShowTechnicalMessages || showMsgAllways)
			{
				if (!string.IsNullOrEmpty(msg))
				{
					if (!string.IsNullOrEmpty(msg)) LocalOutputMessage(msg, false);
				}
			}

			if (_configData.ShowTechnicalMessages)
			{
				if (!string.IsNullOrEmpty(techMsg))
				{
					LocalOutputMessage(techMsg, true);
					if (showDebug) DebugManager.Instance.Write($"{techMsg}\r\n", DebugManager.Modes.Message);
					if (!showMsgAllways) return;
				}
			}
		}

		public void LocalOutputMessage(string msg, bool isTechMsg)
		{
			//LocalOutputBufferClear(false);
			if (_clearLocalBufferFlag && !isTechMsg) return;

			if (_lastLocalOutputChars != "\r\n")
			{
				msg = "\r\n" + msg;
			}
			msg += "\r\n";

			foreach(char chr in msg)
			{
				LocalOutputEnqueue(char.ToUpper(chr), isTechMsg ? CharAttributes.TechMessage : CharAttributes.Message, 0);
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
			if (_itelex.Texting == ItelexProtocol.Textings.Ascii)
			{
				for (int i = 0; i < msg.Length; i++)
				{
					LocalOutputEnqueue(msg[i], CharAttributes.Recv, 0);
				}
				return;
			}

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
			if (attr == CharAttributes.Recv || attr == CharAttributes.Send)
			{
				if (_configData.UpperCaseChar) chr = Char.ToUpper(chr);
			}

			_localOutputBuffer.Enqueue(new ScreenChar(chr, attr, ackCount));
		}

		private void LocalOutputTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_localOutputTimerActive || _clearLocalBufferFlag) return;
			_localOutputTimerActive = true;

			var timeout = TimeSpan.FromMilliseconds(1000);
			bool lockTaken = false;

			try
			{
				if (_localOutputBuffer.Count > 0)
				{
					//Debug.WriteLine($"_localOutputBuffer.Count={_localOutputBuffer.Count}");
					Monitor.TryEnter(_localOutputBufferLock, timeout, ref lockTaken);
					if (lockTaken)
					{
						ScreenChar peek;
						do
						{
							if (_clearLocalBufferFlag) break;
							if (_localOutputBuffer.TryDequeue(out ScreenChar scrChr))
							{
								if (scrChr.Attr != CharAttributes.RecvEmpty) Output?.Invoke(scrChr);
								if (scrChr.AckCount > 0) _itelex.AddPrintedCharCount(scrChr.AckCount);
							}
							_localOutputBuffer.TryPeek(out peek);
						}
						while (_localOutputBuffer.Count > 0 &&
							(peek.Attr == CharAttributes.Message || peek.Attr == CharAttributes.TechMessage));
					}
				}
				if (_configData.OutputSpeed > 0)
				{
					SendCharTrigger?.Invoke();
				}
			}
			finally
			{
				if (lockTaken) Monitor.Exit(_localOutputBufferLock);
				_localOutputTimerActive = false;
			}
		}

	}
}
