using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HouseholdTaskPlanner.TelegramBot
{
    internal class CronlikeTimer
    {
        private readonly Action _callback;
        private readonly Cronos.CronExpression _cronExpression;
        private readonly Timer _timer;

        public CronlikeTimer(string pattern, Action callback)
        {
            _callback = callback;
            _cronExpression = Cronos.CronExpression.Parse(pattern);
            _timer = new Timer(new TimerCallback(Callback));
        }

        public void Start()
        {
            var nextOccurance = _cronExpression.GetNextOccurrence(DateTime.UtcNow);
            if (nextOccurance.HasValue)
            {
                _timer.Change(nextOccurance.Value-DateTime.UtcNow, TimeSpan.FromMilliseconds(-1));
            }
        }

        private void Callback(object o)
        {
            try { _callback?.Invoke(); }
            catch { }
            Start();
        }
    }
}