using Cronos;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace TaskPlanner.TelegramBot
{
    internal class CronlikeTimer
    {
        private readonly Func<Task> _callback;
        private readonly ILogger<CronlikeTimer> _logger;
        private readonly CronExpression _cronExpression;
        private readonly Timer _timer;

        public CronlikeTimer(string pattern, Func<Task> callback, ILogger<CronlikeTimer> logger)
        {
            _callback = callback;
            _logger = logger;
            _cronExpression = CronExpression.Parse(pattern);
            _timer = new Timer(new TimerCallback(Callback));
        }

        public void Start()
        {
            var nextOccurance = _cronExpression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
            if (nextOccurance.HasValue)
            {
                TimeSpan timePeriod = nextOccurance.Value - DateTimeOffset.Now;
                _logger.LogInformation("Scheduled Task in {TimePeriodHours} hours", timePeriod.TotalHours.ToString("F2", CultureInfo.InvariantCulture));
                _timer.Change(timePeriod, TimeSpan.FromMilliseconds(-1));
            }
        }

        private async void Callback(object o)
        {
            _logger.LogInformation("Running scheduled Task!");
            try
            {
                if(_callback != null)
                {
                    await _callback();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while running Task");
            }
            Start();
        }
    }
}