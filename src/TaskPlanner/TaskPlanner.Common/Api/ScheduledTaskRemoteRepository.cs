using HouseholdPlanner.Common;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskPlanner.Common.Models;
using User.Common.Api;

namespace TaskPlanner.Common.Api
{
    public class ScheduledTaskRemoteRepository : ApiRemoteRepository<IScheduledTaskRestApi>, IScheduledTaskRemoteRepository
    {
        public ScheduledTaskRemoteRepository(IOptions<TaskApiConfiguration> options)
            : base(options.Value)
        {
        }

        public Task<ScheduledTaskViewModel> Get(int id)
            => Api.Get(id);

        public Task<IList<ScheduledTaskViewModel>> GetTodoList()
            => Api.GetTodoList();

        public Task Insert(ScheduledTaskViewModel model)
            => Api.Insert(model);

        public Task Delete(int id)
            => Api.Delete(id);

        public Task SetAssignedUser(int id, int? userId)
            => Api.SetAssignedUser(id, userId);

        public Task SetDone(int id)
            => Api.SetDone(id);
    }
}