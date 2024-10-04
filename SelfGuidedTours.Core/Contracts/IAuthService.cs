using Microsoft.AspNetCore.Identity;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.ExternalLogin;
using SelfGuidedTours.Core.Models.RequestDto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IAuthService
    {
        Task<AuthenticateResponse> RegisterAsync(RegisterInputModel model);
        Task<AuthenticateResponse> LoginAsync(LoginInputModel model);
        Task LogoutAsync(string userId);
        Task<AuthenticateResponse> RefreshAsync(RefreshRequestModel model);
        Task<AuthenticateResponse> GoogleSignInAsync(GoogleUserDto googleUser);
        Task<ApiResponse> ChangePasswordAsync(ChangePasswordModel model);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<ApiResponse> CreatePasswordAsync(string userId, string password);
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<bool> VerifyPasswordResetTokenAsync(ApplicationUser user, string token);
        Task<IdentityResult> ResetPasswordAsync(string token, string newPassword);

    }
}