using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Common.Models;

namespace TaskPlanner.Common.Api
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
