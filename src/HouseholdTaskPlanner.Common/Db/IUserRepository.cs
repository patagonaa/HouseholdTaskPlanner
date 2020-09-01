using HouseholdTaskPlanner.Common.Db.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.Common.Db
{
    public interface IUserRepository
    {
        Task<IList<User>> GetAll();
    }
}
