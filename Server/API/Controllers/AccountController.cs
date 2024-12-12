using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Service.DTOs.PlayerDto;
using Service.DTOs.UserDto;
using Service.Security;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IJwtService _jwtService;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        //[Authorize(Policy = "AdminPolicy")] 
        public async Task<IActionResult> Register([FromBody] CreateUserDto model)
        {
            if (string.IsNullOrEmpty(model.Role))
            {
                return BadRequest(new { message = "Role is required." });
            }

            User user;

            // Determine whether to create an Admin or Player
            if (model.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                user = new Admin
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    FullName = model.FullName,
                    CreatedAt = DateTime.UtcNow,
                    PasswordChangeRequired = false
                };
            }
            else if (model.Role.Equals("Player", StringComparison.OrdinalIgnoreCase))
            {
                user = new Player
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    FullName = model.FullName,
                    CreatedAt = DateTime.UtcNow,
                    Balance = 0, 
                    AnnualFeePaid = true,
                    PasswordChangeRequired = true
                };
            }
            else
            {
                return BadRequest(new { message = "Invalid role. Must be 'Admin' or 'Player'." });
            }

            // Create the user
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Registration failed.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            // Assign role to user
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(model.Role));
            }
            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { message = "Registration successful!", userId = user.Id });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password." });

            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT token using the JwtService
            var token = _jwtService.GenerateJwtToken(user.Id.ToString(), user.UserName, roles.ToList());

            return Ok(new LoginResultDto()
            {
                Message = "Login successful!",
                UserName = user.UserName,
                Roles = roles,
                Token = token,
                PasswordChangeRequired = user.PasswordChangeRequired 
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout successful!" });
        }
        
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated. Please log in." });
            }

            // Get the user ID from the token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub); 

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token. User ID not found." });
            }

            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            // Check if the current password provided matches the user's actual current password
            var result = await _signInManager.PasswordSignInAsync(user, model.CurrentPassword, false, false);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Current password is incorrect." });
            }

            // Change the password
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return BadRequest(new { message = "Password change failed.", errors = changePasswordResult.Errors.Select(e => e.Description) });
            }

            // Update the user's PasswordChangeRequired flag
            user.PasswordChangeRequired = false;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Password changed successfully!", user.UserName });
        }


    }
}
