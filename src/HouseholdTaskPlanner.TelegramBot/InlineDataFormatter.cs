using System;
using System.Linq;

namespace HouseholdTaskPlanner.TelegramBot
{
    public static class InlineDataFormatter
    {
        public static class Prefixes
        {
            public const string Recurring = "rec";
            public const string Scheduled = "sch";
        }

        public static string GetPrefix(string input)
            => input.Split('-').FirstOrDefault();

        public static string FormatRecurring(RecurringAction action, string id)
            => string.Join("-", Prefixes.Recurring, Enum.GetName(typeof(RecurringAction), action), id);

        public static (RecurringAction action, int? taskId) ParseRecurring(string input)
        {
            string[] data = input.Split('-');

            if (!data[0].Equals(Prefixes.Recurring))
            {
                throw new ArgumentException($"Broken Callbackdata {input}, expected Prefix to be {Prefixes.Recurring}");
            }

            int? taskId = default;
            if (int.TryParse(data[2], out int parsedTaskId))
            {
                taskId = parsedTaskId;
            }

            return ((RecurringAction)Enum.Parse(typeof(RecurringAction), data[1], true), taskId);
        }

        public static string FormatScheduled(ScheduledAction action, string taskId)
           => string.Join("-", Prefixes.Scheduled, Enum.GetName(typeof(ScheduledAction), action), taskId);

        public static (ScheduledAction action, int? taskId) ParseScheduled(string input)
        {
            string[] data = input.Split('-');

            if (!data[0].Equals(Prefixes.Scheduled))
            {
                throw new ArgumentException($"Broken Callbackdata {input}, expected Prefix to be {Prefixes.Scheduled}");
            }

            int? taskId = default;
            if (int.TryParse(data[2], out int parsedTaskId))
            {
                taskId = parsedTaskId;
            }

            return ((ScheduledAction)Enum.Parse(typeof(ScheduledAction), data[1], true), taskId);
        }

    }
}