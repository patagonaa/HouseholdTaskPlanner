using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Common.Models;

namespace TaskPlanner.Common.Api
{
    public interface IScheduledTaskRemoteRepository
    {
        Task<ScheduledTaskViewModel> Get(int id);

        Task<IList<ScheduledTaskViewModel>> GetTodoList();
        Task SetAssignedUser(int id, int? userId);
        Task SetDone(int id);
        Task Delete(int id);

        Task Insert(ScheduledTaskViewModel model);
    }
}