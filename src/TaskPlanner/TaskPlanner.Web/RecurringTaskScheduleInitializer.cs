using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace TaskPlanner.Web
{
    public class RecurringTaskScheduleInitializer : IHostedService
    {
        private readonly ScheduledTaskService _scheduledTaskService;

        public RecurringTaskScheduleInitializer(ScheduledTaskService scheduledTaskService)
        {
            _scheduledTaskService = scheduledTaskService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _scheduledTaskService.InitializeRecurringTasks();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
