using HouseholdTaskPlanner.Common.Db;
using HouseholdTaskPlanner.Common.Db.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholeTaskPlanner.Web.Controllers
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
    }
}
