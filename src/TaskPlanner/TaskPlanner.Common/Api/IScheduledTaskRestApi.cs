using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Common.Models;

namespace TaskPlanner.Common.Api
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

        [Put("/ScheduledTask")]
        Task Insert(ScheduledTaskViewModel model);
    }
}