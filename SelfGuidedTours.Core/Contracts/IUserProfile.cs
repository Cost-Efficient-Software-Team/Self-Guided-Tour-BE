using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IProfileService
    {
        Task<UserProfileDto?> GetProfileAsync(Guid userId);
        Task<UserProfile?> UpdateProfileAsync(Guid userId, UserProfile profile);
        Task CreateProfileAsync(UserProfile userProfile);
    }
}