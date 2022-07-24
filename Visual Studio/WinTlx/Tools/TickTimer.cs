using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Tools
{
	public class TickTimer
	{
		private long _msTicks;

		public bool IsStarted { get; private set; }

		public TickTimer()
		{
			Start();
		}

		public TickTimer(bool start)
		{
			IsStarted = start;
			if (start)
			{
				Start();
			}
		}

		public void Start()
		{
			_msTicks = GetTicksMs();
			IsStarted = true;
		}

		public void Stop()
		{
			IsStarted = false;
		}

		public bool IsElapsedSeconds(int seconds)
		{
			if (!IsStarted) return false;
			return IsElapsedMilliseconds(seconds * 1000);
		}

		public bool IsElapsedMinutes(int minutes)
		{
			if (!IsStarted) return false;
			return IsElapsedMilliseconds(minutes * 1000 * 60);
		}

		public bool IsElapsedHours(int hours)
		{
			if (!IsStarted) return false;
			return IsElapsedMilliseconds(hours * 1000 * 60 * 60);
		}

		public bool IsElapsedMilliseconds(int milliseconds)
		{
			if (!IsStarted) return false;
			return ElapsedMilliseconds > milliseconds;
		}

		public int ElapsedSeconds
		{
			get
			{
				if (!IsStarted) return 0;
				return (int)(ElapsedMilliseconds / 1000);
			}
		}

		public long ElapsedMilliseconds
		{
			get
			{
				if (!IsStarted) return 0;
				return GetTicksMs() - _msTicks;
			}
		}

		public long Milliseconds
		{
			get
			{
				if (!IsStarted) return 0;
				return _msTicks;
			}
		}

		/// <summary>
		/// get systems ticks in ms
		/// </summary>
		public static long GetTicksMs()
		{
			return DateTime.Now.Ticks / 10000;
		}

		//private long MilliTicks()
		//{
		//	return DateTime.Now.Ticks / 10000;
		//}
	}
}
