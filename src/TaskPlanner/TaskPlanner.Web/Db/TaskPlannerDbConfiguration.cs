using HouseholdPlanner.Common;

namespace TaskPlanner.Web.Db
{
    public class TaskPlannerDbConfiguration : IDbConfiguration
    {
        public string ConnectionString { get; set; }
    }
}
