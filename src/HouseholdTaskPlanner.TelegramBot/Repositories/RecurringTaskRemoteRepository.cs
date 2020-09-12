using HouseholdTaskPlanner.Common.Db.Models;
using Refit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Repositories
{
    public class RecurringTaskRemoteRepository : IRecurringTaskRemoteRepository
    {
        private readonly IRecurringTaskRestApi _api;

        public RecurringTaskRemoteRepository(HttpClient client)
        {
            _api = RestService.For<IRecurringTaskRestApi>(client);
        }

        public Task<IList<RecurringTask>> GetAll()
            => _api.GetAll();

        public Task Insert(RecurringTask task)
            => _api.Insert(task);

        public Task<bool> Update(RecurringTask task)
            => _api.Update(task);

        public Task Delete(int id)
            => _api.Delete(id);
    }
}