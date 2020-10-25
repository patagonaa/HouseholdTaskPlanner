using TaskPlanner.Common.Db.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskPlanner.TelegramBot.Repositories
{
    public interface IRecurringTaskRestApi
    {
        [Get("/recurringTask")]
        Task<IList<RecurringTask>> GetAll();

        [Post("/recurringTask")]
        Task Insert(RecurringTask task);

        [Put("/recurringTask")]
        Task<bool> Update(RecurringTask task);

        [Delete("/recurringTask/{id}")]
        Task Delete(int id);
    }
}
