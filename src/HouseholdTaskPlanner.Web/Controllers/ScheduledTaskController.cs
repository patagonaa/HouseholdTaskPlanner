﻿using HouseholdTaskPlanner.Common.Db;
using HouseholdTaskPlanner.Common.Db.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.Web.Controllers
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

        [HttpGet]
        public async Task<IList<ScheduledTaskViewModel>> GetList()
        {
            return await _scheduledTaskRepository.GetList();
        }

        [HttpPost()]
        public Task Create([FromBody] ScheduledTaskViewModel model)
            => _scheduledTaskRepository.Insert(new ScheduledTask
            {
                Date = model.Date,
                Name = model.Name,
                State = ScheduledTaskState.Todo,
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
        public async Task<ActionResult> Delete([FromBody] int id)
        {
            if (!await _scheduledTaskRepository.Delete(id))
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
