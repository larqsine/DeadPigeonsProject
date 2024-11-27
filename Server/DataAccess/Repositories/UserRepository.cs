using Microsoft.AspNetCore.Identity;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class UserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Create user
        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        // Assign role to user
        public async Task<IdentityResult> AssignRoleToUserAsync(User user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            return result;
        }

        // Find user by email
        public async Task<User?> FindUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        // Get roles for a user
        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }
    }
}