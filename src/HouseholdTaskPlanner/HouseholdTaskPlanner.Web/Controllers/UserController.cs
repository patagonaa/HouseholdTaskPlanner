using HouseholdTaskPlanner.Common.Db;
using HouseholdTaskPlanner.Common.Db.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HouseholdTaskPlanner.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IList<User>> GetList()
        {
            return await _userRepository.GetAll();
        }
    }
}
