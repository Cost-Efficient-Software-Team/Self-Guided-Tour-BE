using SelfGuidedTours.Core.Models;
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
        Task<List<TourResponseDto>> GetMyToursAsync(string userId);
        Task<List<TourResponseDto>> GetBoughtToursAsync(string userId);
        
       
    }
}