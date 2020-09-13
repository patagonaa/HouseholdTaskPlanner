using HouseholdTaskPlanner.Common.Db.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Repositories
{
    public interface IUserRemoteRepository
    {
        Task<IList<User>> GetAll();
    }
}