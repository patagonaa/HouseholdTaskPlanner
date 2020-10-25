using Dapper;
using TaskPlanner.Common.Db.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskPlanner.Common.Db
{
    public class UserRepository : SqlServerRepository, IUserRepository
    {
        public UserRepository(IOptions<DbConfiguration> options) : base(options)
        {
        }

        public async Task<IList<User>> GetAll()
        {
            using (var connection = GetConnection())
            {
                var results = await connection.QueryAsync<User>("SELECT * FROM [dbo].[User]");
                return results.ToList();
            }
        }
    }
}
