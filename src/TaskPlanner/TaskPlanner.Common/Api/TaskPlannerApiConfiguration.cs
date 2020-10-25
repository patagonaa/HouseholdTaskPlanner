using HouseholdPlanner.Common;

namespace User.Common.Api
{
    public class TaskApiConfiguration : IApiConfiguration
    {
        public string BackendLocation { get; set; }

        public string BasicAuth { get; set; }
    }
}
