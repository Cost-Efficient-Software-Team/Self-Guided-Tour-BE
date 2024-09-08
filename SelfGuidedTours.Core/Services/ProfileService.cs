using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.RequestDto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBlobService blobSerivice;

        public ProfileService(IRepository repository, UserManager<ApplicationUser> userManager, IBlobService blobSerivice)
        {
            _repository = repository;
            _userManager = userManager;
            this.blobSerivice = blobSerivice;
        }

        public async Task<UserProfileDto?> GetProfileAsync(string userId)
        {
          var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found");

            var userProfile = new UserProfileDto
            {
                UserId = Guid.Parse(user.Id),
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                PhoneNumber = user.PhoneNumber,
                AboutYourself = user.Bio,
                Email = user.Email!,
            };
                
            return userProfile;

        }

        public async Task<UserProfileDto?> UpdateProfileAsync(string userId, UpdateProfileRequestDto profile)
        {
          var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found");
            //If there is a profile picture, upload it to blob storage and get the URL
            string? profilePictureUrl = await HandleProfilePictureAsync(profile?.ProfilePicture);
            //Update the user's profile
            user.FirstName = profile?.FirstName;
            user.LastName = profile?.LastName;
            user.ProfilePictureUrl =profilePictureUrl;
            user.PhoneNumber = profile?.PhoneNumber;
            user.Bio = profile?.AboutYourself;
            user.Email = profile?.Email;

            await _repository.UpdateAsync(user);
            await _repository.SaveChangesAsync();
            //Return the updated profile
            var updatedProfile = new UserProfileDto
            {
                UserId = Guid.Parse(user.Id),
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                PhoneNumber = user.PhoneNumber,
                AboutYourself = user.Bio,
                Email = user.Email!,
            };

            return updatedProfile;
        }

        public async Task CreateProfileAsync(UserProfile userProfile)
        {
            await _repository.AddAsync(userProfile);
            await _repository.SaveChangesAsync();
        }

        protected async Task<string> HandleProfilePictureAsync(IFormFile? profilePicture)
        {
            if (profilePicture is null)
                return string.Empty;
            //TODO: take this from env variables when we decide if its going to be in a different container
            string containerName = "profile-pictures";
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";

            var profilePictureUrl = await blobSerivice.UploadFileAsync(containerName,profilePicture, fileName, true);

            return profilePictureUrl ?? string.Empty;
        }
    }
}
