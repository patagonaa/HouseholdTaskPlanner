namespace HouseholdTaskPlanner.Web.Controllers
{
    public class RecurringTaskAddModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int IntervalDays { get; set; }
    }
}