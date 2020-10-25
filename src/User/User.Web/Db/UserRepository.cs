using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseholdPlanner.Common;
using User.Common.Models;

namespace User.Web.Db
{
    public class UserRepository : SqlServerRepository, IUserRepository
    {
        public UserRepository(IOptions<UserDbConfiguration> options) : base(options.Value)
        {
        }

        public async Task<IList<UserModel>> GetAll()
        {
            using (var connection = GetConnection())
            {
                var results = await connection.QueryAsync<UserModel>("SELECT * FROM [dbo].[User]");
                return results.ToList();
            }
        }
    }
}
