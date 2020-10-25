using System;
using System.Threading.Tasks;
using TaskPlanner.Common.Models;
using TaskPlanner.Web.Db;

namespace TaskPlanner.Web
{
    public class ScheduledTaskService
    {
        private readonly IScheduledTaskRepository _scheduledTaskRepository;
        private readonly IRecurringTaskRepository _recurringTaskRepository;

        public ScheduledTaskService(IScheduledTaskRepository scheduledTaskRepository, IRecurringTaskRepository recurringTaskRepository)
        {
            _scheduledTaskRepository = scheduledTaskRepository;
            _recurringTaskRepository = recurringTaskRepository;
        }

        public async Task RescheduleRecurringTask(RecurringTask recurringTask)
        {
            await _scheduledTaskRepository.DeleteForRecurringTask(recurringTask.Id);
            await ScheduleRecurringTask(recurringTask);
        }

        public async Task InitializeRecurringTasks()
        {
            var recurringTasks = await _recurringTaskRepository.GetUnscheduled();
            foreach (var recurringTask in recurringTasks)
            {
                await ScheduleRecurringTask(recurringTask);
            }
        }

        private async Task ScheduleRecurringTask(RecurringTask recurringTask)
        {
            await _scheduledTaskRepository.Insert(new ScheduledTask
            {
                Date = DateTime.UtcNow.Date + TimeSpan.FromDays(recurringTask.IntervalDays), // TODO: this should use local time
                RecurringTaskId = recurringTask.Id,
                State = ScheduledTaskState.Todo
            });
        }

        public async Task HandleTaskDone(int id)
        {
            var finishedTask = await _scheduledTaskRepository.Get(id);
            if (finishedTask.RecurringTaskId.HasValue)
            {
                await _scheduledTaskRepository.Insert(await GetNext(finishedTask));
            }

            await _scheduledTaskRepository.SetState(id, ScheduledTaskState.Done);
        }

        private async Task<ScheduledTask> GetNext(ScheduledTask finishedTask)
        {
            var recurringTask = await _recurringTaskRepository.Get(finishedTask.RecurringTaskId.Value);
            if (recurringTask == null)
                throw new InvalidOperationException("This should not happen because of FK_ScheduledTask_RecurringTask");

            return new ScheduledTask
            {
                Date = DateTime.UtcNow.Date + TimeSpan.FromDays(recurringTask.IntervalDays), // TODO: this should use local time
                RecurringTaskId = recurringTask.Id,
                State = ScheduledTaskState.Todo
            };
        }
    }
}
