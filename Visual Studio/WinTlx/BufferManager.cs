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

		private readonly ConfigData _configManager;

		public delegate void UpdateSendEventHandler();
		public event UpdateSendEventHandler UpdateSend;

		public delegate void OutputEventHandler(ScreenChar screenChar);
		public event OutputEventHandler Output;

		/// <summary>
		/// Timer for send buffer
		/// </summary>
		private bool _sendTimerActive;
		private readonly System.Timers.Timer _sendTimer;
		private readonly Queue<char> _sendBuffer;

		/// <summary>
		/// Timer for screen output buffer
		/// </summary>
		private readonly System.Timers.Timer _localOutputTimer;
		private readonly Queue<ScreenChar> _localOutputBuffer;

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

			_configManager = ConfigManager.Instance.Config;

			//_textEditorManager = TextEditorManager.Instance;
			//_textEditorManager.Send += TextEditor_Send;
			//_textEditorManager.ShowMsg += TextEditor_ShowMsg;

			_sendBuffer = new Queue<char>();
			_sendTimerActive = false;
			_sendTimer = new System.Timers.Timer(100);
			_sendTimer.Elapsed += SendTimer_Elapsed;
			_sendTimer.Start();

			_localOutputBuffer = new Queue<ScreenChar>();
			_localOutputTimer = new System.Timers.Timer();
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
			asciiStr = CodeManager.AsciiStringToTelex(asciiStr, _configManager.CodeSet);
			foreach (char chr in asciiStr)
			{
				_sendBuffer.Enqueue(chr);
			}
		}

		public int SendBufferCount
		{
			get
			{
				return _sendBuffer.Count;
			}
		}

		public async Task WaitSendBufferEmpty()
		{
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			while (true)
			{
				await Task.Delay(100);
				if (_itelex.IdleTimerMs > 30 * 1000)
				{
					Debug.WriteLine($"WaitSendBufferEmpty timeout {_itelex.IdleTimerMs}");
					return;
				}
				if (SendBufferCount == 0 && _itelex.SendBufferCount == 0 && _itelex.RemoteBufferCount == 0) return;
			}
		}

		private void SendTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_sendTimerActive)
			{
				return;
			}

			try
			{
				_sendTimerActive = true;
				if (_sendBuffer.Count == 0)
				{
					return;
				}
				//Debug.WriteLine(_itelex.GetSendBufferCount());
				while (_itelex.GetSendBufferCount() < 5 && _sendBuffer.Count > 0)
				{
					_itelex.SendAsciiChar(_sendBuffer.Dequeue());
				}
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
				_localOutputTimer.Interval = 10;
			}
			_localOutputTimer.Start();
		}

		public void LocalOutputBufferClear()
		{
			_localOutputBuffer.Clear();
		}

		public async Task WaitLocalOutpuBufferEmpty()
		{
			while(true)
			{
				await Task.Delay(100);
				if (LocalOutputBufferCount == 0) return;
			}
		}

		public void LocalOutputMsg(string msg)
		{
			foreach(char chr in msg)
			{
				ScreenChar screenChr = new ScreenChar(char.ToUpper(chr), CharAttributes.Message);
				//OutputBufferEnqueue(screenChr);
				Output?.Invoke(screenChr);
			}
		}

		public void LocalOutputSend(string msg)
		{
			foreach (char chr in msg)
			{
				LocalOutputEnqueue(new ScreenChar(chr, CharAttributes.Send));
			}
		}

		public void LocalOutputRecv(string msg)
		{
			foreach (char chr in msg)
			{
				LocalOutputEnqueue(new ScreenChar(chr, CharAttributes.Recv));
			}
		}

		public void LocalOutputEnqueue(ScreenChar screenChar)
		{
			_localOutputBuffer.Enqueue(screenChar);
		}

		public int LocalOutputBufferCount
		{
			get
			{
				return _localOutputBuffer.Count;
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
			if (_localOutputBuffer.Count > 0)
			{
				Output?.Invoke(_localOutputBuffer.Dequeue());
			}
		}

	}
}
