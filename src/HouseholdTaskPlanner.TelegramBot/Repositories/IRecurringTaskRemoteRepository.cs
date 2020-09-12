using HouseholdTaskPlanner.Common.Db.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Repositories
{
    public interface IRecurringTaskRemoteRepository
    {
        Task<IList<RecurringTask>> GetAll();

        Task Insert(RecurringTask task);

        Task<bool> Update(RecurringTask task);
    }
}