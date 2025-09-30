using Microsoft.AspNetCore.Mvc;
using TransactionApp.BUSINESS.Abstract;
using TransactionApp.ENTITIES.Dto.TransactionDto;
using TransactionApp.ENTITIES.Dto.UserDto;

namespace TransactionApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserListDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(result.Data);
        }


        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User information</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return Ok(result.Data);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userCreateDto"></param>
        /// <returns>Created user</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserCreateDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddUser([FromBody] UserCreateDto userCreateDto)
        {
            var result = await _userService.AddUserAsync(userCreateDto);
            return CreatedAtAction(nameof(GetUserById), new { id = result.Data }, userCreateDto);
        }

    }
}
