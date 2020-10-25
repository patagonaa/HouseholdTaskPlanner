using System.Collections.Generic;
using System.Threading.Tasks;
using User.Common.Models;

namespace User.Common.Api
{
    public interface IUserRemoteRepository
    {
        Task<IList<UserModel>> GetAll();
    }
}