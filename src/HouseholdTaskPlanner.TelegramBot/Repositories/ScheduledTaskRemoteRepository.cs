using HouseholdTaskPlanner.Common.Db.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Repositories
{
    public class ScheduledTaskRemoteRepository : IScheduledTaskRemoteRepository
    {
        private readonly IScheduledTaskRestApi _api;
        public ScheduledTaskRemoteRepository(TaskplannerApiHttpClientFactory httpClientFactory)
        {
            _api = RestService.For<IScheduledTaskRestApi>(httpClientFactory.Get());
        }

        public Task<ScheduledTaskViewModel> Get(int id)
            => _api.Get(id);

        public Task<IList<ScheduledTaskViewModel>> GetTodoList()
            => _api.GetTodoList();

        public Task Insert(ScheduledTaskViewModel model)
            => _api.Insert(model);

        public Task Delete(int id)
            => _api.Delete(id);

        public Task SetAssignedUser(int id, int? userId)
            => _api.SetAssignedUser(id, userId);

        public Task SetDone(int id)
            => _api.SetDone(id);
    }
}