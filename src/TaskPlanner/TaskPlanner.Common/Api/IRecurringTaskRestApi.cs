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

        [Put("/recurringTask")]
        Task Insert(RecurringTaskAddModel task);

        [Post("/recurringTask")]
        Task<bool> Update(RecurringTask task);

        [Delete("/recurringTask/{id}")]
        Task Delete(int id);
    }
}
