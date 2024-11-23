using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // POST api/user/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            var user = new User { UserName = model.UserName, Email = model.Email };
            var result = await _userRepository.CreateUserAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Assign the role
            await _userRepository.AssignRoleToUserAsync(user, model.Role);
            return Ok("User created successfully with role " + model.Role);
        }

        // POST api/user/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDto model)
        {
            var user = await _userRepository.FindUserByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // You can add more login logic here, like password verification.
            var roles = await _userRepository.GetUserRolesAsync(user);

            return Ok(new { UserName = user.UserName, Roles = roles });
        }
    }

    // DTOs for creating and logging in users
    public class CreateUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Admin or Player
    }

    public class LoginUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
