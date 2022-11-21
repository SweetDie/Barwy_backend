using Barwy.Data.Data.Validation;
using Barwy.Data.Data.ViewModels.Account;
using Barwy.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Barwy.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("signIn")]
        public async Task<IActionResult> SignInAsync(SignInVM model)
        {
            var validator = new SignInValidation();
            var validateResult = validator.Validate(model);

            if(validateResult.IsValid)
            {
                var result = await _accountService.SignInAsync(model);
                return Ok(result);
            }

            return BadRequest(validateResult.Errors);
        }

        [AllowAnonymous]
        [HttpPost("signUp")]
        public async Task<IActionResult> SignUpAsync(SignUpVM model)
        {
            var validator = new SignUpValidation();
            var validateResult = validator.Validate(model);

            if (validateResult.IsValid)
            {
                var result = await _accountService.SignUpAsync(model);
                return Ok(result);
            }

            return BadRequest(validateResult.Errors);
        }

        [AllowAnonymous]
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _accountService.ConfirmEmailAsync(userId, token);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshTokenAsync(TokenRequestVM model)
        {
            var validator = new TokenRequestValidation();
            var validationResult = await validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
                var result = await _accountService.RefreshTokenAsync(model);
                return Ok(result);
            }
            else
            {
                return BadRequest(validationResult.Errors);
            }

        }
    }
}
