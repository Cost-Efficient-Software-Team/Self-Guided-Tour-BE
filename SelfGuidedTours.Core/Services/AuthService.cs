using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Services.TokenGenerators;
using SelfGuidedTours.Core.Services.TokenValidators;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository repository;
        private readonly AccessTokenGenerator accessTokenGenerator;
        private readonly RefreshTokenGenerator refreshTokenGenerator;
        private readonly RefreshTokenValidator refreshTokenValidator;
        private readonly IRefreshTokenService refreshTokenService;

        public AuthService(IRepository repository,
            AccessTokenGenerator accessTokenGenerator,
            RefreshTokenGenerator refreshTokenGenerator,
            RefreshTokenValidator refreshTokenValidator,
            IRefreshTokenService refreshTokenService)
        {
            this.repository = repository;
            this.accessTokenGenerator = accessTokenGenerator;
            this.refreshTokenGenerator = refreshTokenGenerator;
            this.refreshTokenValidator = refreshTokenValidator;
            this.refreshTokenService = refreshTokenService;
        }

        private async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            //In that case if the current user is not registered with the provided email yet, the method will return null.

            return await repository.AllReadOnly<ApplicationUser>()
                .FirstOrDefaultAsync(au => au.Email == email);
        }

        private async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await repository.AllReadOnly<ApplicationUser>()
                .FirstOrDefaultAsync(au => au.Id == userId);
        }

        private async Task<LoginResponse> AuthenticateAsync(ApplicationUser user, string responesMessage)
        {
            var accessToken = accessTokenGenerator.GenerateToken(user);
            var refreshToken = refreshTokenGenerator.GenerateToken();

            RefreshToken refreshTokenDTO = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };

            await refreshTokenService.CreateAsync(refreshTokenDTO);
            
            return new LoginResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ResponseMessage = responesMessage
            };
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
            var user = await GetByEmailAsync(model.Email);

            if (user == null)
            {
                throw new ArgumentException("Email or password is incorrect!");
            }

            var isPassCorrect = new PasswordHasher<ApplicationUser>().VerifyHashedPassword(user, user.PasswordHash!, model.Password);

            if (isPassCorrect != PasswordVerificationResult.Success)
            {
                throw new ArgumentException("Email or password is incorrect!");
            }

            return await AuthenticateAsync(user, "Successfully logged in!");
        }

        public async Task LogoutAsync(string userId)
        {
            await refreshTokenService.DeleteAllAsync(userId);
        }

        public async Task<LoginResponse> RefreshAsync(RefreshRequestModel model)
        {
            var isValidRefreshToken = refreshTokenValidator.Validate(model.RefreshToken);

            if (!isValidRefreshToken)
            {
                throw new ArgumentException("Invalid refresh token!");
            }

            var refreshTokenDTO = await refreshTokenService.GetByTokenAsync(model.RefreshToken);

            if(refreshTokenDTO == null)
            {
                throw new ArgumentException("Refresh token was not found!");
            }

            await refreshTokenService.DeleteAsync(refreshTokenDTO.Id);

            var user = await GetByIdAsync(refreshTokenDTO.UserId);

            if(user == null)
            {
                throw new ArgumentException("User was not found!");
            }

            return await AuthenticateAsync(user, "Successfully got new tokens!");
        }

    }
}
