using Barwy.Data.Data.Context;
using Barwy.Data.Data.Models;
using Barwy.Data.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Barwy.Data.Data.Repositories.Classes
{
    public class AccountRepository : IAccountRepository
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountRepository(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Account
        public async Task<SignInResult> SignInAsync(AppUser model, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(model, password, rememberMe, true);
            return result;
        }

        public async Task<IdentityResult> SignUpAsync(AppUser model, string password)
        {
            var result = await _userManager.CreateAsync(model, password);
            return result;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(AppUser model, string token)
        {
            var result = await _userManager.ConfirmEmailAsync(model, token);
            return result;
        }

        // Tokens
        public async Task<string> GenerateEmailConfirmationTokenAsync(AppUser appUser)
        {
            var result = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            return result;
        }

        public async Task<RefreshToken> CheckRefreshTokenAsync(string refreshToken)
        {
            using (var _context = new AppDbContext())
            {
                var result = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
                return result;
            }
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            using (var _context = new AppDbContext())
            {
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            using (var _context = new AppDbContext())
            {
                _context.RefreshTokens.Update(refreshToken);
                await _context.SaveChangesAsync();
            }
        }


    }
}
