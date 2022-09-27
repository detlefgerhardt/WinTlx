using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace WinTlx.Scheduler
{
	[DataContract(Namespace = "")]
	class SchedulerItem
	{
		[DataMember]
		public DateTime Timestamp { get; set; }

		[DataMember]
		public string Destination
		{
			get { return _destination; }
			set
			{
				_destination = value;
				SetDestination(value);
			}
		}
		private string _destination;

		[DataMember]
		public string Filename { get; set; }

		[DataMember]
		public bool Active { get; set; }

		[DataMember]
		public bool Success { get; set; }

		[DataMember]
		public bool Error { get; set; }

		public int DestNumber { get; private set; }
		/// <summary>
		/// Destination host name or address
		/// </summary>
		public string DestAddress { get; set; }
		public int DestPort { get; set; }
		public int DestExtension { get; set; }
		public int Retries { get; set; }
		public DateTime? LastRetry { get; set; }

		public bool Executable => !string.IsNullOrWhiteSpace(Destination) && !string.IsNullOrWhiteSpace(Filename);

		public SchedulerItem()
		{
			Retries = 0;
			Success = false;
		}

		private void SetDestination(string dest)
		{
			if (string.IsNullOrWhiteSpace(dest))
			{
				_destination = null;
				return;
			}

			string[] list = dest.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (list.Length == 1)
			{
				if (uint.TryParse(list[0], out uint number))
				{
					DestNumber = (int)number;
				}
			}
			else if (list.Length == 3)
			{
				DestAddress = list[0];
				if (uint.TryParse(list[0], out uint port))
				{
					DestNumber = (int)port;
				}
				if (uint.TryParse(list[0], out uint extension))
				{
					DestNumber = (int)extension;
				}
				else
				{
					DestNumber = 0;
				}
			}
		}

		public override string ToString()
		{
			return $"{Timestamp:dd.MM.yyyy HH:mm:ss} {Destination} active={Active} success={Success} error={Error} retries={Retries}";
		}

	}
}
