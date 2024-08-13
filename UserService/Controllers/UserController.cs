using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Interfaces;
using UserService.Models;
using UserService.Utils;
using static UserService.Models.UserRepository;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(AuthorizationResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> AddAdministrator([FromBody] UserDto user)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ControllerUtils.GetModelErrors(ModelState));

            return await _userRepository.AddAdministratorAsync(user).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(AuthorizationResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> AddUser([FromBody] UserDto user)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ControllerUtils.GetModelErrors(ModelState));

            return await _userRepository.AddUserAsync(user).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<UserRepository>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetUsers()
        {
            return await _userRepository.GetUserListAsync().ConfigureAwait(false);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("[action]/{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> RemoveUser(Guid id)
        {
            return await _userRepository.RemoveUserAsync(id).ConfigureAwait(false);
        }

        [HttpGet]
        [Authorize]
        [Route("[action]")]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetMyId()
        {
            var guid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new OkObjectResult(guid);
        }
    }
}
