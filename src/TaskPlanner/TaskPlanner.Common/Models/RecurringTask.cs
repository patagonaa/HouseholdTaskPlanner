namespace TaskPlanner.Common.Models
{
    public class RecurringTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IntervalDays { get; set; }
        public string Tags { get; set; }
    }
}
