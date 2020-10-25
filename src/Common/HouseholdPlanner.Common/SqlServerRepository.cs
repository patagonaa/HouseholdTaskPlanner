using System.Data.SqlClient;

namespace HouseholdPlanner.Common
{
    public abstract class SqlServerRepository
    {
        private readonly IDbConfiguration _config;

        protected SqlServerRepository(IDbConfiguration config)
        {
            _config = config;
        }

        protected SqlConnection GetConnection()
        {
            SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString);
            sqlConnection.Open();
            return sqlConnection;
        }
    }
}
