using TaskPlanner.Common.Db.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskPlanner.TelegramBot.Repositories
{
    public class UserRemoteRepository : IUserRemoteRepository
    {
        private readonly IUserRestApi _api;

        public UserRemoteRepository(TaskplannerApiHttpClientFactory httpClientFactory)
        {
            _api = RestService.For<IUserRestApi>(httpClientFactory.Get());
        }

        public Task<IList<User>> GetAll() => _api.GetAll();
    }
}