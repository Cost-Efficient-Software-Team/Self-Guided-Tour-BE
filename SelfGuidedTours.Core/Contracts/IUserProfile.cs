using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IProfileService
    {
        Task<UserProfile?> GetProfileAsync(Guid userId);
        Task<UserProfile?> UpdateProfileAsync(Guid userId, UserProfile profile);
        Task CreateProfileAsync(UserProfile userProfile);
    }
}