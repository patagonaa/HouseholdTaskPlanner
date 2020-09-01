﻿using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace HouseholdTaskPlanner.Common.Db
{
    public abstract class SqlServerRepository
    {
        private readonly DbConfiguration _config;

        protected SqlServerRepository(IOptions<DbConfiguration> options)
        {
            _config = options.Value;
        }

        protected SqlConnection GetConnection()
        {
            SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString);
            sqlConnection.Open();
            return sqlConnection;
        }
    }
}
