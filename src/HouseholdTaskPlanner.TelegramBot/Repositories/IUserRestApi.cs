using HouseholdTaskPlanner.Common.Db.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Repositories
{
    public interface IUserRestApi
    {
        [Get("/user")]
        Task<IList<User>> GetAll();
    }
}