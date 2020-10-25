using System.Collections.Generic;

namespace TaskPlanner.TelegramBot
{
    public class BotConfiguration
    {
        public string BotToken { get; set; }

        public long ScheduleChat { get; set; }
        public IList<long> AllowedChats { get; set; }
    }
}
