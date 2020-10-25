using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Common.Models;

namespace TaskPlanner.Web.Db
{
    public interface IRecurringTaskRepository
    {
        Task Insert(RecurringTask task);
        Task<bool> Update(RecurringTask task);
        Task<IList<RecurringTask>> GetAll();
        Task<RecurringTask> Get(int id);
        Task<IList<RecurringTask>> GetUnscheduled();
        Task<bool> Delete(int id);
    }
}