using System;

namespace Timer
{
	public class TimerData
	{
		public DateTime Start { get; private set; }

		public TimeSpan Duration { get; }

		public TimeSpan Remain
		{
			get
			{
				var remain = (Start + Duration) - DateTime.Now;
				return remain.TotalSeconds > 0 ? remain : TimeSpan.Zero;
			}
		}

		public TimerData(DateTime start, TimeSpan duration)
		{
			Start = start;
			Duration = duration;
		}

		public void Reset()
		{
			Start = DateTime.Now;
		}
	}
}
