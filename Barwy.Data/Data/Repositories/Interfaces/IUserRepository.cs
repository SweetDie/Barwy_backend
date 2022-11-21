using Barwy.Data.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Barwy.Data.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // Users
        Task<AppUser> GetUserByIdAsync(string id);
        Task<AppUser> GetUserByEmailAsync(string email);
        Task<AppUser> GetUserByUserNameAsync(string userName);
        Task<List<AppUser>> GetAllUsersAsync();

        // Roles
        Task<IList<string>> GetRolesAsync(AppUser model);
        Task<IdentityResult> AddToRoleAsync(AppUser model, string role);
    }
}
