using System.Collections.Generic;
using System.Threading.Tasks;
using User.Common.Models;

namespace User.Web.Db
{
    public interface IUserRepository
    {
        Task<IList<UserModel>> GetAll();
    }
}
