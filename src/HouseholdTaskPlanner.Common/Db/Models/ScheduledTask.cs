using System;

namespace HouseholdTaskPlanner.Common.Db.Models
{
    public class ScheduledTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int? RecurringTaskId { get; set; }
        public ScheduledTaskState State { get; set; }
        public int? AssignedUserId { get; set; }
    }
}
