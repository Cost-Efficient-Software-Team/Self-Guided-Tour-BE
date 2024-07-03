using Microsoft.AspNetCore.Identity;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.ExternalLogin;
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
        Task<IdentityResult> ResetPasswordAsync(string token, string newPassword);
        Task<ApplicationUser?> GetByEmailAsync(string email);
    }
}
