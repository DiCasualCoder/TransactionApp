using Microsoft.AspNetCore.Mvc;
using TransactionApp.BUSINESS.Abstract;
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
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }


        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User information</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userCreateDto"></param>
        /// <returns>Created user</returns>
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserCreateDto userCreateDto)
        {
            var result = await _userService.AddUserAsync(userCreateDto);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetUserById), new { id = result.Data }, userCreateDto);
            }
            return BadRequest(result);
        }   

    }
}
