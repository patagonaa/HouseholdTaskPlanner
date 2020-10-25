using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using User.Common.Api;
using User.Common.Models;

namespace HouseholdPlanner.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRemoteRepository _userRepository;

        public UserController(IUserRemoteRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IList<UserModel>> GetList()
        {
            return await _userRepository.GetAll();
        }
    }
}
