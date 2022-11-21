using Barwy.Data.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Barwy.Data.Data.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        // Account
        Task<SignInResult> SignInAsync(AppUser model, string password, bool rememberMe);
        Task<IdentityResult> SignUpAsync(AppUser model, string password);
        Task<IdentityResult> ConfirmEmailAsync(AppUser model, string token);

        // Tokens

        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> CheckRefreshTokenAsync(string refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task<string> GenerateEmailConfirmationTokenAsync(AppUser appUser);
    }
}
