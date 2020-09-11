using HouseholdTaskPlanner.Common.Db.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.Common.Db
{
    public interface IScheduledTaskRepository
    {
        Task<ScheduledTask> Get(int id);
        Task<IList<ScheduledTaskViewModel>> GetList();
        Task Insert(ScheduledTask scheduledTask);
        Task SetState(int id, ScheduledTaskState state);
        Task SetAssignedUser(int id, int? userId);
        Task DeleteForRecurringTask(int recurringTaskId);
        Task<bool> Delete(int id);
    }
}
