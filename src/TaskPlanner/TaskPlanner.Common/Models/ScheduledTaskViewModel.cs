using System;

namespace TaskPlanner.Common.Models
{
    public class ScheduledTaskViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ScheduledTaskState State { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AssignedUser { get; set; }
    }
}
