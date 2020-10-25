using TaskPlanner.Common.Db.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskPlanner.TelegramBot.Repositories
{
    public interface IRecurringTaskRemoteRepository
    {
        Task<IList<RecurringTask>> GetAll();

        Task Insert(RecurringTask task);

        Task<bool> Update(RecurringTask task);

        Task Delete(int id);
    }
}