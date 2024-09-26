using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.RequestDto;
using SelfGuidedTours.Core.Models.ResponseDto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;
using static SelfGuidedTours.Common.Constants.FormatConstants;
namespace SelfGuidedTours.Core.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBlobService blobSerivice;
        private readonly ITourService tourService;

        public ProfileService(IRepository repository, UserManager<ApplicationUser> userManager, IBlobService blobSerivice, ITourService tourService)
        {
            _repository = repository;
            _userManager = userManager;
            this.blobSerivice = blobSerivice;
            this.tourService = tourService;
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
                About = user.Bio,
                Email = user.Email!,
                HasPassword = user.HasPassword,
            };
                
            return userProfile;

        }

        public async Task<UserProfileDto?> UpdateProfileAsync(string userId, UpdateProfileRequestDto profile)
        {
          var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found");
            //If there is a profile picture, upload it to blob storage and get the URL
            string? profilePictureUrl = await HandleProfilePictureAsync(profile?.ProfilePicture, user);
            //Update the user's profile
            user.FirstName = profile?.FirstName ?? user.FirstName;
            user.LastName = profile?.LastName ?? user.LastName;
            user.ProfilePictureUrl =profilePictureUrl ?? user.ProfilePictureUrl;
            user.PhoneNumber = profile?.PhoneNumber ?? user.PhoneNumber;
            user.Bio = profile?.About ?? user.Bio;
            user.Email = profile?.Email ?? user.Email;
            user.UserName = profile?.Email ?? user.Email;
            user.NormalizedEmail = profile?.Email?.ToUpper() ?? user.Email!.ToUpper();
            user.NormalizedUserName = profile?.Email?.ToUpper() ?? user.Email!.ToUpper();

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
                About = user.Bio,
                Email = user.Email!,
            };

            return updatedProfile;
        }

        public async Task CreateProfileAsync(UserProfile userProfile)
        {
            await _repository.AddAsync(userProfile);
            await _repository.SaveChangesAsync();
        }

        protected async Task<string?> HandleProfilePictureAsync(IFormFile? profilePicture, ApplicationUser user)
        {
            //If there is no profile picture, return null.
            //If the user already has a profile picture but doesent want changes the FileName is undefined
            if (profilePicture is null || profilePicture.FileName == "undefined")
                return null;
            //TODO: take this from env variables when we decide if its going to be in a different container
            string containerName = "profile-pictures";

            
            var fileName = $"{user.Id}-{profilePicture.Name}{Path.GetExtension(profilePicture.FileName)}";

            if(user.ProfilePictureUrl != null)
            {
                await blobSerivice.DeleteFileAsync(fileName, containerName);
            }

            var profilePictureUrl = await blobSerivice.UploadFileAsync(containerName,profilePicture, fileName, true);

            return profilePictureUrl;
        }

        public async Task<List<TourResponseDto>> GetMyToursAsync(string userId)
        {
            var tours = await _repository.AllReadOnly<Tour>()
                .Where(t=>t.CreatorId == userId)
                .ToListAsync();

            var tourResponse = tours
                .Select(t => tourService.MapTourToTourResponseDto(t))
                .ToList();

            return tourResponse;
        } 
        public async Task<List<TourResponseDto>> GetBoughtToursAsync(string userId)
        {
            var tours = await _repository.AllReadOnly<UserTours>()
                .Where(ut=>ut.UserId == userId)
                .Select(t=> tourService.MapTourToTourResponseDto(t.Tour))
                .ToListAsync();

            return tours;
        }

        public async Task<List<UserTransactionsResponseDto>> GetUserTransactionsAsync(string userId)
        {
            var transactions = await _repository
                                        .AllReadOnly<Payment>()
                                        .Where(p => p.Status == PaymentStatus.Succeeded && p.UserId == userId)
                                        .Select(tr => new UserTransactionsResponseDto
                                        {
                                            TourTitle = tr.Tour.Title,
                                            Date = tr.PaymentDate.ToString(TransactionDateFormat),
                                            Price = tr.Amount,
                                        })
                                        .ToListAsync();
             return transactions;

        }
    }
}
