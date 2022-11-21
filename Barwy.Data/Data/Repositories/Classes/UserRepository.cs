using Barwy.Data.Data.Models;
using Barwy.Data.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Barwy.Data.Data.Repositories.Classes
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Users
        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            var result = await _userManager.FindByIdAsync(id);
            return result;
        }
        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            return result; ;
        }

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
            var result = await _userManager.FindByNameAsync(userName);
            return result; ;
        }

        public async Task<List<AppUser>> GetAllUsersAsync()
        {
            var result = await _userManager.Users.ToListAsync();
            return result;
        }

        // Roles
        public async Task<IList<string>> GetRolesAsync(AppUser model)
        {
            var result = await _userManager.GetRolesAsync(model);
            return result;
        }

        public async Task<IdentityResult> AddToRoleAsync(AppUser model, string role)
        {
            var result = await _userManager.AddToRoleAsync(model, role);
            return result;
        }
    }
}
