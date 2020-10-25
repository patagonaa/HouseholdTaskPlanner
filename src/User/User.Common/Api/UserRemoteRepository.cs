using HouseholdPlanner.Common;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using User.Common.Models;

namespace User.Common.Api
{
    public class UserRemoteRepository : ApiRemoteRepository<IUserRestApi>, IUserRemoteRepository
    {
        public UserRemoteRepository(IOptions<UserApiConfiguration> options)
            : base(options.Value)
        {
        }

        public Task<IList<UserModel>> GetAll() => Api.GetAll();
    }
}