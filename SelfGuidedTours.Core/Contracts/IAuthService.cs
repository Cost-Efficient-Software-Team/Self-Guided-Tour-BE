using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Data.Models;
﻿using SelfGuidedTours.Core.Models.Auth;


namespace SelfGuidedTours.Core.Contracts
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterInputModel model);
        Task<LoginResponse> LoginAsync(LoginInputModel model);
        Task LogoutAsync(string userId);
        Task<LoginResponse> RefreshAsync(RefreshRequestModel model);
    }
}
