using TaskPlanner.Common.Db.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskPlanner.TelegramBot.Repositories
{
    public interface IUserRestApi
    {
        [Get("/user")]
        Task<IList<User>> GetAll();
    }
}