using System.Collections.Generic;
using System.Threading.Tasks;
using HouseholdPlanner.Common;
using Microsoft.Extensions.Options;
using User.Common.Api;
using TaskPlanner.Common.Models;

namespace TaskPlanner.Common.Api
{
    public class RecurringTaskRemoteRepository : ApiRemoteRepository<IRecurringTaskRestApi>, IRecurringTaskRemoteRepository
    {
        public RecurringTaskRemoteRepository(IOptions<TaskApiConfiguration> options)
            : base(options.Value)
        {
        }

        public Task<IList<RecurringTask>> GetAll()
            => Api.GetAll();

        public Task Insert(RecurringTaskAddModel task)
            => Api.Insert(task);

        public Task<bool> Update(RecurringTask task)
            => Api.Update(task);

        public Task Delete(int id)
            => Api.Delete(id);
    }
}