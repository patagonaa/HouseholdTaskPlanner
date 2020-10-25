using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using User.Common.Models;

namespace User.Common.Api
{
    public interface IUserRestApi
    {
        [Get("/user")]
        Task<IList<UserModel>> GetAll();
    }
}