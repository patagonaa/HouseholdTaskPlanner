using TaskPlanner.Common.Db.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskPlanner.TelegramBot.Repositories
{
    public interface IScheduledTaskRestApi
    {
        [Get("/ScheduledTask/{id}")]
        Task<ScheduledTaskViewModel> Get(int id);

        [Get("/ScheduledTask/Todo")]
        Task<IList<ScheduledTaskViewModel>> GetTodoList();

        [Post("/ScheduledTask/{id}/SetAssignedUser/{userId}")]
        Task SetAssignedUser(int id, int? userId);

        [Post("/ScheduledTask/{id}/SetDone")]
        Task SetDone(int id);

        [Delete("/ScheduledTask/{id}")]
        Task Delete(int id);

        [Post("/ScheduledTask")]
        Task Insert(ScheduledTaskViewModel model);
    }
}