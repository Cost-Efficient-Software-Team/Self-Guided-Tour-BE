using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IRepository _repository;

        public ProfileService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserProfile?> GetProfileAsync(Guid userId)
        {
            return await _repository.GetProfileAsync(userId);
        }

        public async Task<UserProfile?> UpdateProfileAsync(Guid userId, UserProfile profile)
        {
            return await _repository.UpdateProfileAsync(userId, profile);
        }

        public async Task CreateProfileAsync(UserProfile userProfile)
        {
            await _repository.AddAsync(userProfile);
            await _repository.SaveChangesAsync();
        }
    }
}
