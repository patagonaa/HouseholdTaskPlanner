namespace HouseholdPlanner.Common
{
    public interface IApiConfiguration
    {
        string BackendLocation { get; }
        string BasicAuth { get; }
    }
}
