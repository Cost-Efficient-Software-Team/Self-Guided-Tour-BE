using Microsoft.AspNetCore.Identity;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<UserProfileDto?> GetProfileAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return null;
            }

            var profile = await _repository.GetProfileAsync(userId);
            if (profile == null)
            {
                return null;
            }

            return new UserProfileDto
            {
                UserId = Guid.Parse(user.Id),
                Name = user.Name,
                Email = user.Email
                // Добавете други полета, които искате да включите
            };
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
