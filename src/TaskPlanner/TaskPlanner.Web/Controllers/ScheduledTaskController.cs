using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskPlanner.Common.Models;
using TaskPlanner.Web.Db;

namespace TaskPlanner.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduledTaskController : ControllerBase
    {
        private readonly IScheduledTaskRepository _scheduledTaskRepository;
        private readonly ScheduledTaskService _scheduledTaskService;

        public ScheduledTaskController(IScheduledTaskRepository scheduledTaskRepository, ScheduledTaskService scheduledTaskService)
        {
            _scheduledTaskRepository = scheduledTaskRepository;
            _scheduledTaskService = scheduledTaskService;
        }

        [HttpGet("Todo")]
        public async Task<IList<ScheduledTaskViewModel>> GetTodoList()
        {
            return (await _scheduledTaskRepository.GetAll())
                .Where(x => x.State == ScheduledTaskState.Todo)
                .ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduledTaskViewModel>> Get([FromRoute] int id)
        {
            var viewModel = (await _scheduledTaskRepository.GetAll())
                .SingleOrDefault(x => x.Id == id);

            if (viewModel == null)
            {
                return NotFound();
            }
            return viewModel;
        }

        [HttpPut()]
        public Task Create([FromBody] ScheduledTaskViewModel model)
            => _scheduledTaskRepository.Insert(new ScheduledTask
            {
                Date = model.Date,
                Name = model.Name,
                State = model.State == ScheduledTaskState.None ? ScheduledTaskState.Todo : model.State,
                RecurringTaskId = default
            });

        [HttpPost("{id}/SetDone")]
        public async Task SetDone([FromRoute] int id)
        {
            await _scheduledTaskService.HandleTaskDone(id);
        }

        [HttpPost("{id}/SetAssignedUser/{userId}")]
        public async Task SetAssigndeUser([FromRoute] int id, [FromRoute] int userId)
        {
            await _scheduledTaskRepository.SetAssignedUser(id, userId <= 0 ? null : (int?)userId);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            if (!await _scheduledTaskRepository.Delete(id))
            {
                return NotFound();
            }
            await _scheduledTaskService.InitializeRecurringTasks();
            return Ok();
        }
    }
}
