using HouseholdTaskPlanner.Common.Db.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.TelegramBot.Repositories
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
