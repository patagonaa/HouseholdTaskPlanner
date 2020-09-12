using HouseholdTaskPlanner.Common.Db.Models;
using Refit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Repositories
{
    public class UserRemoteRepository : IUserRemoteRepository
    {
        private readonly IUserRestApi _api;

        public UserRemoteRepository(HttpClient httpClient)
        {
            _api = RestService.For<IUserRestApi>(httpClient);
        }

        public Task<IList<User>> GetAll() => _api.GetAll();
    }
}