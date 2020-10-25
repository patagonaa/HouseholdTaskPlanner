using TaskPlanner.Common.Db.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskPlanner.Common.Db
{
    public interface IScheduledTaskRepository
    {
        Task<ScheduledTask> Get(int id);
        Task<IList<ScheduledTaskViewModel>> GetAll();
        Task Insert(ScheduledTask scheduledTask);
        Task SetState(int id, ScheduledTaskState state);
        Task SetAssignedUser(int id, int? userId);
        Task DeleteForRecurringTask(int recurringTaskId);
        Task<bool> Delete(int id);
    }
}
