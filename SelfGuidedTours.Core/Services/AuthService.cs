using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.ExternalLogin;
using SelfGuidedTours.Core.Services.TokenGenerators;
using SelfGuidedTours.Core.Services.TokenValidators;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Identity;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Infrastructure.Data.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository repository;
        private readonly AccessTokenGenerator accessTokenGenerator;
        private readonly RefreshTokenGenerator refreshTokenGenerator;
        private readonly RefreshTokenValidator refreshTokenValidator;
        private readonly IRefreshTokenService refreshTokenService;
        private readonly IProfileService profileService;  

        public AuthService(
            IRepository repository,
            AccessTokenGenerator accessTokenGenerator,
            RefreshTokenGenerator refreshTokenGenerator,
            RefreshTokenValidator refreshTokenValidator,
            IRefreshTokenService refreshTokenService,
            IProfileService profileService  
        )
        {
            this.repository = repository;
            this.accessTokenGenerator = accessTokenGenerator;
            this.refreshTokenGenerator = refreshTokenGenerator;
            this.refreshTokenValidator = refreshTokenValidator;
            this.refreshTokenService = refreshTokenService;
            this.profileService = profileService;  
        }

        private async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await repository.AllReadOnly<ApplicationUser>()
                .FirstOrDefaultAsync(au => au.Email == email);
        }

        private async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await repository.AllReadOnly<ApplicationUser>()
                .FirstOrDefaultAsync(au => au.Id == userId);
        }

        private long GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);

            return ticks;
        }

        private async Task<AuthenticateResponse> AuthenticateAsync(ApplicationUser user, string responseMessage)
        {
            var accessToken = accessTokenGenerator.GenerateToken(user);
            var refreshToken = refreshTokenGenerator.GenerateToken();

            RefreshToken refreshTokenDTO = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };

            await refreshTokenService.CreateAsync(refreshTokenDTO);

            return new AuthenticateResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ResponseMessage = responseMessage,
                AccessTokenExpiration = GetTokenExpirationTime(accessToken)
            };
        }

        public async Task<AuthenticateResponse> RegisterAsync(RegisterInputModel model)
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
                UserName = model.Email, // needed for the reset pass
                NormalizedUserName = model.Email.ToUpper(), // needed for the reset pass
                Name = model.Name,
                PasswordHash = hasher.HashPassword(null!, model.Password)
            };

            var userRole = AssignUserRole(user.Id);

            await repository.AddAsync(user);
            await repository.AddAsync(userRole);
            await repository.SaveChangesAsync();

            var userProfile = new UserProfile
            {
                UserId = Guid.Parse(user.Id),
                Name = model.Name,
                Email = model.Email
            };

            await profileService.CreateProfileAsync(userProfile);

            return await AuthenticateAsync(user, "User registered successfully!");
        }

        public async Task<AuthenticateResponse> LoginAsync(LoginInputModel model)
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

        public async Task<AuthenticateResponse> GoogleSignInAsync(GoogleUserDto googleUser)
        {
            if (googleUser == null)
            {
                throw new ArgumentException("Invalid Google Id Token");
            }

            var user = await GetByEmailAsync(googleUser.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = googleUser.Email,
                    UserName = googleUser.Email,
                    Name = googleUser.Name,
                };
                var userRole = AssignUserRole(user.Id);

                await repository.AddAsync(userRole);
                await repository.AddAsync(user);
                await repository.SaveChangesAsync();
            }

            return await AuthenticateAsync(user, "Successfully logged in!");
        }

        public async Task LogoutAsync(string userId)
        {
            await refreshTokenService.DeleteAllAsync(userId);
        }

        public async Task<AuthenticateResponse> RefreshAsync(RefreshRequestModel model)
        {
            var isValidRefreshToken = refreshTokenValidator.Validate(model.RefreshToken);

            if (!isValidRefreshToken)
            {
                throw new ArgumentException("Invalid refresh token!");
            }

            var refreshTokenDTO = await refreshTokenService.GetByTokenAsync(model.RefreshToken);

            if (refreshTokenDTO == null)
            {
                throw new ArgumentException("Refresh token was not found!");
            }

            await refreshTokenService.DeleteAsync(refreshTokenDTO.Id);

            var user = await GetByIdAsync(refreshTokenDTO.UserId);

            if (user == null)
            {
                throw new ArgumentException("User was not found!");
            }

            return await AuthenticateAsync(user, "Successfully got new tokens!");
        }

        private IdentityUserRole<string> AssignUserRole(string userId)
        {
            return new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = "4f8554d2-cfaa-44b5-90ce-e883c804ae90" //User Role Id
            };
        }

        public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordModel model)
        {
            if (model.CurrentPassword == model.NewPassword)
            {
                throw new ArgumentException("New password can't be the same as the current one!");
            }

            var user = await GetByIdAsync(model.UserId);

            if (user is null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            if (user.PasswordHash is null)
            {
                throw new UnauthorizedAccessException("User has no assigned password!");
            }

            var hasher = new PasswordHasher<ApplicationUser>();

            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);

            if (result != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid password");
            }

            user.PasswordHash = hasher.HashPassword(user, model.NewPassword);

            await repository.UpdateAsync(user);
            await repository.SaveChangesAsync();

            var response = new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = "Password changed successfully!"
            };
            return response;
        }
    }
}
