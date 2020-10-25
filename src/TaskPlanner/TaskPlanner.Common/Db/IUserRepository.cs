using TaskPlanner.Common.Db.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskPlanner.Common.Db
{
    public interface IUserRepository
    {
        Task<IList<User>> GetAll();
    }
}
