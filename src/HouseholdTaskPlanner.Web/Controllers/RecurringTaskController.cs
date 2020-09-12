﻿using HouseholdTaskPlanner.Common.Db;
using HouseholdTaskPlanner.Common.Db.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurringTaskController : ControllerBase
    {
        private readonly IRecurringTaskRepository _recurringTaskRepository;
        private readonly ScheduledTaskService _scheduledTaskService;

        public RecurringTaskController(IRecurringTaskRepository recurringTaskRepository, ScheduledTaskService scheduledTaskService)
        {
            _recurringTaskRepository = recurringTaskRepository;
            _scheduledTaskService = scheduledTaskService;
        }

        [HttpGet]
        public async Task<IList<RecurringTask>> GetList()
        {
            return await _recurringTaskRepository.GetAll();
        }

        [HttpPost]
        public async Task Post([FromBody] RecurringTaskAddModel addModel)
        {
            var newTask = new RecurringTask
            {
                Name = addModel.Name,
                Description = addModel.Description,
                IntervalDays = addModel.IntervalDays
            };

            await _recurringTaskRepository.Insert(newTask);
            await _scheduledTaskService.InitializeRecurringTasks();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] RecurringTask updateModel)
        {
            if (!await _recurringTaskRepository.Update(updateModel))
            {
                return NotFound();
            }
            await _scheduledTaskService.RescheduleRecurringTask(updateModel);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            if (!await _recurringTaskRepository.Delete(id))
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
