﻿using Barwy.Data.Data.Repositories.Interfaces;
using Barwy.Data.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Barwy.Data.Data.ViewModels.Account;

namespace Barwy.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;

        public JwtService(IConfiguration configuration, TokenValidationParameters tokenValidationParameters, IAccountRepository accountRepository, IUserRepository userRepository)
        {
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
        }

        public async Task<Tokens> GenerateJwtTokenAsync(AppUser user)
        {
            var roles = await _userRepository.GetRolesAsync(user);
            var role = roles.Count > 0 ? roles[0] : "unknown";

            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Key"]);
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("id", user.Id),
                new Claim("username", user.UserName),
                new Claim("name", user.Name),
                new Claim("surname", user.Surname),
                new Claim("email", user.Email),
                new Claim("phoneNumber", user.PhoneNumber == null ? "" : user.PhoneNumber),
                new Claim("emailConfirmed", user.EmailConfirmed.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtConfig:TokenValidityInMinutes"])),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken(user, token);

            await _accountRepository.SaveRefreshTokenAsync(refreshToken);

            var tokens = new Tokens();
            tokens.AccessToken = jwtToken;
            tokens.RefreshToken = refreshToken;

            return tokens;
        }

        private RefreshToken GenerateRefreshToken(AppUser user, SecurityToken token)
        {
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtConfig:RefreshTokenValidityInDays"])),
                IsRevoked = false,
                Token = RandomString(25) + Guid.NewGuid()
            };
            return refreshToken;
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<ServiceResponse> VerifyTokenAsync(TokenRequestVM tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return new ServiceResponse()
                        {
                            Message = "Token not valid",
                            IsSuccess = false
                        };
                    }
                }

                // Will get the time stamp in unix time
                var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                // we convert the expiry date from seconds to the date
                var expDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expDate > DateTime.UtcNow)
                {
                    return new ServiceResponse()
                    {
                        Message = "We cannot refresh this since the token has not expired",
                        IsSuccess = false
                    };
                }

                // Check the token we got if its saved in the db
                var storedRefreshToken = await _accountRepository.CheckRefreshTokenAsync(tokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new ServiceResponse()
                    {
                        Message = "Refresh token doesnt exist",
                        IsSuccess = false
                    };
                }

                // Check the date of the saved token if it has expired
                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new ServiceResponse()
                    {
                        Message = "Token has expired",
                        IsSuccess = false
                    };
                }

                // check if the refresh token has been used
                if (storedRefreshToken.IsUsed)
                {
                    return new ServiceResponse()
                    {
                        Message = "Token has been used",
                        IsSuccess = false
                    };
                }

                // Check if the token is revoked
                if (storedRefreshToken.IsRevoked)
                {
                    return new ServiceResponse()
                    {
                        Message = "Token has been revoked",
                        IsSuccess = false
                    };
                }

                // we are getting here the jwt token id
                var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                // check the id that the recieved token has against the id saved in the db
                if (storedRefreshToken.JwtId != jti)
                {
                    return new ServiceResponse()
                    {
                        Message = "Token doenst mateched the saved token",
                        IsSuccess = false
                    };
                }

                storedRefreshToken.IsUsed = true;
                await _accountRepository.UpdateRefreshTokenAsync(storedRefreshToken);

                var dbUser = await _userRepository.GetUserByIdAsync(storedRefreshToken.UserId);

                var tokens = new Tokens();
                tokens = await GenerateJwtTokenAsync(dbUser);
                return
                    new ServiceResponse()
                    {
                        IsSuccess = true,
                        Message = "Token successfully updated.",
                        Payload = tokens
                    };
            }
            catch (Exception ex)
            {
                return
                   new ServiceResponse()
                   {
                       Message = ex.Message,
                       IsSuccess = false
                   };
            }
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }
    }

    public class Tokens
    {
        public string AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
