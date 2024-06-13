using SelfGuidedTours.Core.Models.Auth;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IAuthService
    {
        Task<AuthenticateResponse> RegisterAsync(RegisterInputModel model);
        Task<AuthenticateResponse> LoginAsync(LoginInputModel model);
        Task LogoutAsync(string userId);
        Task<AuthenticateResponse> RefreshAsync(RefreshRequestModel model);
    }
}
