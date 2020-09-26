using Cronos;
using System;
using System.Threading;

namespace HouseholdTaskPlanner.TelegramBot
{
    internal class CronlikeTimer
    {
        private readonly Action _callback;
        private readonly CronExpression _cronExpression;
        private readonly Timer _timer;

        public CronlikeTimer(string pattern, Action callback)
        {
            _callback = callback;
            _cronExpression = CronExpression.Parse(pattern);
            _timer = new Timer(new TimerCallback(Callback));
        }

        public void Start()
        {
            var nextOccurance = _cronExpression.GetNextOccurrence(DateTime.UtcNow);
            if (nextOccurance.HasValue)
            {
                _timer.Change(nextOccurance.Value - DateTime.Now, TimeSpan.FromMilliseconds(-1));
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