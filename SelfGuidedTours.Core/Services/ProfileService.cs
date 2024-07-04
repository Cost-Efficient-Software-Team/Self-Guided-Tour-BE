using SelfGuidedTours.Core.Contracts;
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
            var existingProfile = await _repository.GetProfileAsync(userId);
            if (existingProfile == null)
            {
                return null;
            }

            existingProfile.Name = profile.Name;
            existingProfile.Email = profile.Email;

            await _repository.UpdateAsync(existingProfile);
            await _repository.SaveChangesAsync();
            return existingProfile;
        }

        public async Task CreateProfileAsync(UserProfile userProfile)
        {
            await _repository.AddAsync(userProfile);
            await _repository.SaveChangesAsync();
        }
    }
}