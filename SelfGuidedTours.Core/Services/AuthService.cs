using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SelfGuidedTours.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository repository;
        private readonly IConfiguration configuration;

        public AuthService(IRepository repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        private async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            //In that case if the current user is not registered with the provided email yet, the method will return null.

            return await repository.AllReadOnly<ApplicationUser>()
                .FirstOrDefaultAsync(au => au.Email == email);
        }

        private string GenerateJwtToken(string email, TimeSpan expiration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Environment.GetEnvironmentVariable("JWT_KEY") ??
                throw new ApplicationException("JWT key is not configured.");
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, email)
                }),
                Expires = DateTime.UtcNow.Add(expiration),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
           
            return tokenHandler.WriteToken(token);
        }
        public async Task<string> RegisterAsync(RegisterInputModel model)
        {
            if (await GetByEmailAsync(model.Email) != null)
            {
                throw new ArgumentException("User already exists!");
            }

            if (model.Password != model.RepeatPassword)
            {
                throw new ArgumentException("Passwords do not match!");
            }

            var hasher = new PasswordHasher<ApplicationUser>();

            var user = new ApplicationUser
            {
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                Name = model.Name,
                PasswordHash = hasher.HashPassword(null!, model.Password) // Hash the password
            };

            await repository.AddAsync(user);
            await repository.SaveChangesAsync();

            return "User registered successfully!";
        }

        public async Task<LoginResponse> LoginAsync(LoginInputModel model)
        {
            LoginResponse response = new LoginResponse();

            var user = await GetByEmailAsync(model.Email);

            if(user == null)
            {
                response.ResponseMessage = "Email or password is incorrect!";
                return response;
            }

            var isPassCorrect = new PasswordHasher<ApplicationUser>().VerifyHashedPassword(user, user.PasswordHash!, model.Password);

            if(isPassCorrect != PasswordVerificationResult.Success)
            {
                response.ResponseMessage = "Email or password is incorrect!";
                return response;
            }

            var accessToken = GenerateJwtToken(user.Email!, TimeSpan.FromSeconds(15));
            var refreshToken = GenerateJwtToken(user.Email!, TimeSpan.FromDays(60));

            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            response.ResponseMessage = "Successfully logged in!";

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(60);

            await repository.UpdateAsync(user);
            await repository.SaveChangesAsync();

            return response;
        }
    }
}
