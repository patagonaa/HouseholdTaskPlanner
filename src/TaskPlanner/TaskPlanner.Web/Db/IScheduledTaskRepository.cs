using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Common.Models;

namespace TaskPlanner.Web.Db
{
    public interface IScheduledTaskRepository
    {
        Task<ScheduledTask> Get(int id);
        Task<IList<ScheduledTaskViewModel>> GetAll();
        Task Insert(ScheduledTask scheduledTask);
        Task SetState(int id, ScheduledTaskState state);
        Task SetAssignedUser(int id, int? userId);
        Task DeleteScheduledForRecurringTask(int recurringTaskId);
        Task<bool> Delete(int id);
    }
}
