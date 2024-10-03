using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.RequestDto;
using SelfGuidedTours.Core.Models.ResponseDto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IProfileService
    {
        Task<UserProfileDto?> GetProfileAsync(string userId);
        Task<UserProfileDto?> UpdateProfileAsync(string userId, UpdateProfileRequestDto profile);
        Task CreateProfileAsync(UserProfile userProfile);
        Task<ApiResponse> GetMyToursAsync(string userId, int page, int pageSize);
        Task<ApiResponse> GetBoughtToursAsync(string userId, int page, int pageSize);
        Task<ApiResponse> GetUserTransactionsAsync(string userId, int page, int pageSize);
       // Task<ApiResponse> ChangePasswordAsync(string userId, CreateOrChangePasswordRequestDto changePasswordRequest);

    }
}