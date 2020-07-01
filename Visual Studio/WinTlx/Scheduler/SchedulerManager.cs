using System;
using System.Collections.Generic;
using System.IO;

namespace WinTlx.Scheduler
{
	class SchedulerManager
	{
		private const string TAG = nameof(SchedulerManager);

		private const string SCHEDULE_NAME = Constants.PROGRAM_NAME + ".sch";

		public ScheduleData ScheduleData { get; set; }

		public List<SchedulerItem> SchedulerList => ScheduleData?.SchedulerList;

		public delegate void ScheduleEventHandler(ScheduleEventArgs scheduleEventArgs);
		public event ScheduleEventHandler Schedule;

		public delegate void ChangedEventHandler();
		public event ChangedEventHandler Changed;

		private readonly System.Timers.Timer _scheduleTimer;

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static SchedulerManager instance;

		public static SchedulerManager Instance => instance ?? (instance = new SchedulerManager());

		public int SchedulerActive { get; private set; }

		private SchedulerManager()
		{
			SchedulerActive = -1;
			_scheduleTimer = new System.Timers.Timer(500);
			_scheduleTimer.Elapsed += ScheduleTimer_Elapsed;
			_scheduleTimer.Start();
		}

		private bool _schedulerActive = false;

		private void ScheduleTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (_schedulerActive || ScheduleData?.SchedulerList==null)
			{
				return;
			}

			_schedulerActive = true;

			DateTime now = DateTime.Now;
			//bool changed = false;
			for(int i=0; i< ScheduleData.SchedulerList.Count; i++)
			{
				SchedulerItem item = ScheduleData.SchedulerList[i];
				if (!item.Active || item.Success || item.Error)
				{
					continue;
				}

				if (!item.Executable)
				{
					item.Error = true;
					//changed = true;
					continue;
				}

				if (item.Timestamp <= now)
				{
					if (item.Retries > 0 && item.LastRetry != null)
					{
						// retry after 10 seconds
						if (Helper.MilliDiff(item.LastRetry.Value) < 10000)
							break;
					}

					SchedulerActive = i;
					Changed?.Invoke();

					DoSchedule(item);
					if (item.Success)
					{
						item.Success = true;
						item.Active = false;
						SaveScheduler();
						Changed?.Invoke();
					}
					else
					{
						item.LastRetry = DateTime.Now;
						item.Retries++;
						if (item.Retries >= Constants.SCHEDULER_MAX_RETRIES)
						{
							item.Error = true;
							item.Active = false;
							SaveScheduler();
							Changed?.Invoke();
						}
					}

					SchedulerActive = -1;
				}
			}
			_schedulerActive = false;
		}

		/*
		public void Test()
		{
			ScheduleData = new ScheduleData();
			ScheduleData.SchedulerList.Add(new SchedulerItem()
			{
				Active = true,
				Success = false,
				Timestamp = DateTime.Now.AddSeconds(30),
				Destination = "211230",
				Filename = @"d:\test.bat"
			});
		}
		*/

		public bool LoadScheduler()
		{
			try
			{
				if (!File.Exists(SCHEDULE_NAME))
				{
					ScheduleData = new ScheduleData();
					Logging.Instance.Info(TAG, nameof(LoadScheduler), "No schedular file found");
					return false;
				}

				string configXml = File.ReadAllText(SCHEDULE_NAME);
				ScheduleData = Helper.Deserialize<ScheduleData>(configXml);
				if (ScheduleData.SchedulerList==null)
				{
					ScheduleData.SchedulerList = new List<SchedulerItem>();
				}
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(LoadScheduler), "Error reading scheduler file", ex);
				ScheduleData = new ScheduleData();
				return false;
			}
		}

		public bool SaveScheduler()
		{
			try
			{
				string configXml = Helper.SerializeObject2<ScheduleData>(ScheduleData);
				File.WriteAllText(SCHEDULE_NAME, configXml);
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SaveScheduler), "Error writing scheduler file", ex);
				return false;
			}
		}

		public void SortScheduler()
		{
			ScheduleData.SchedulerList.Sort(new SchedulerItemComparer());
		}

		public void DoSchedule(SchedulerItem item)
		{
			ScheduleEventArgs args = new ScheduleEventArgs(item);
			Schedule?.Invoke(args);
		}
	}
}
