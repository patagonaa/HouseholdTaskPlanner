using HouseholdTaskPlanner.Common.Db.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Repositories
{
    public interface IScheduledTaskRemoteRepository
    {
        Task<ScheduledTaskViewModel> Get(int id);

        Task<IList<ScheduledTaskViewModel>> GetList();
        Task SetAssignedUser(int id, int? userId);
        Task SetDone(int id);
        Task Delete(int id);

        Task Insert(ScheduledTaskViewModel model);
    }
}