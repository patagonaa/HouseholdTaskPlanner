using TaskPlanner.Common.Db.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskPlanner.TelegramBot.Repositories
{
    public interface IUserRemoteRepository
    {
        Task<IList<User>> GetAll();
    }
}