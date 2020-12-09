using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Common.Models;

namespace TaskPlanner.Common.Api
{
    public interface IRecurringTaskRemoteRepository
    {
        Task<IList<RecurringTask>> GetAll();

        Task Insert(RecurringTaskAddModel task);

        Task<bool> Update(RecurringTask task);

        Task Delete(int id);
    }
}