using AutoMapper;
using Barwy.Data.Data.Repositories.Interfaces;
using Barwy.Data.Data.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;
using Barwy.Data.Data.ViewModels.Account;

namespace Barwy.Services
{
    public class AccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly EmailService _emailService;

        public AccountService(IConfiguration configuration, JwtService jwtService, IMapper mapper, IUserRepository userRepository, IAccountRepository accountRepository, EmailService emailService)
        {
            _configuration = configuration;
            _jwtService = jwtService;
            _mapper = mapper;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _emailService = emailService;
        }

        public async Task<ServiceResponse> SignInAsync(SignInVM model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Login);

            if (user == null)
            {
                user = await _userRepository.GetUserByUserNameAsync(model.Login);

                if (user == null)
                {
                    return new ServiceResponse
                    {
                        Message = "Login or password incorrect.",
                        IsSuccess = false
                    };
                }
            }

            var loginResult = await _accountRepository.SignInAsync(user, model.Password, model.RememberMe);

            if (loginResult.Succeeded)
            {
                var tokens = await _jwtService.GenerateJwtTokenAsync(user);

                return new ServiceResponse
                {
                    Message = "Login success",
                    IsSuccess = true,
                    Payload = tokens
                };
            }
            else if (loginResult.IsNotAllowed)
            {
                return new ServiceResponse
                {
                    Message = "Email not confirmed",
                    IsSuccess = false,
                };
            }
            else if (loginResult.IsLockedOut)
            {
                return new ServiceResponse
                {
                    Message = "More than 5 attempts",
                    IsSuccess = false,
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Message = "Login or password incorrect.",
                    IsSuccess = false
                };
            }
        }

        public async Task<ServiceResponse> SignUpAsync(SignUpVM model)
        {
            var newUser = _mapper.Map<SignUpVM, AppUser>(model);

            var result = await _accountRepository.SignUpAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                await _userRepository.AddToRoleAsync(newUser, "User");

                var emailConfirmationToken = await _accountRepository.GenerateEmailConfirmationTokenAsync(newUser);
                var encodedEmailToken = Encoding.UTF8.GetBytes(emailConfirmationToken);
                var emailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                string url = $"{_configuration["HostSettings:URL"]}/api/Account/confirmEmail?userid={newUser.Id}&token={emailToken}";

                //string emailBody = $"<h1>Confirm your email</h1> <a href='{url}'>Confirm now</a>";
                string emailBody =
                    $"<div style=\"font-size:16px;text-align:left\">" +
                    $"<div style=\"line-height:150%\">" +
                    $"<div style=\"font-size:20px\">Hi {newUser.UserName},</div>" +
                    $"<div style=\"margin:15px 0\">" +
                    $"Thank you for creating an account at Barwy shop. Please verify your email address by clicking the button below." +
                    $"</div>" +
                    $"</div>" +
                    $"<div>" +
                    $"<a href=\"{url}\" style=\"background:#007bff;padding:9px;width:200px;color:#fff;text-decoration:none;display:inline-block;font-weight:bold;text-align:center;letter-spacing:0.5px;border-radius:4px\">Verify email address" +
                    $"</a>" +
                    $"</div>" +
                    $"<div style=\"line-height:150%\">" +
                    $"<div style=\"color:#828282;margin:15px 0 75px\">" +
                    $" - Barwy shop team" +
                    $"</div>" +
                    $"</div>" +
                    $"</div>";

                await _emailService.SendEmailAsync(newUser.Email, "Confirm your email address", emailBody);

                return new ServiceResponse
                {
                    Message = "Successfully registered",
                    IsSuccess = true
                };
            }

            return new ServiceResponse
            {
                Message = "Not successfully registered",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<ServiceResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return new ServiceResponse
                {
                    Message = "User not found",
                    IsSuccess = false
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _accountRepository.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new ServiceResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true,
                };

            return new ServiceResponse
            {
                Message = "Email did not confirm",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<ServiceResponse> RefreshTokenAsync(TokenRequestVM model)
        {
            var result = await _jwtService.VerifyTokenAsync(model);
            return result;
        }
    }
}
