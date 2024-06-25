using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.ExternalLogin;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IAuthService
    {
        Task<AuthenticateResponse> RegisterAsync(RegisterInputModel model);
        Task<AuthenticateResponse> LoginAsync(LoginInputModel model);
        Task LogoutAsync(string userId);
        Task<AuthenticateResponse> RefreshAsync(RefreshRequestModel model);
        Task<AuthenticateResponse> GoogleSignInAsync(GoogleUserDto googleUser);
        Task<ApiResponse> ChanghePasswordAsync(ChangePasswordModel model);
        
    }
}
