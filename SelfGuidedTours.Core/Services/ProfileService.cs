using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Extensions;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.RequestDto;
using SelfGuidedTours.Core.Models.ResponseDto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;
using static SelfGuidedTours.Common.Constants.FormatConstants;
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
            user.ProfilePictureUrl = profilePictureUrl ?? user.ProfilePictureUrl;
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

            if (user.ProfilePictureUrl != null)
            {
                await blobSerivice.DeleteFileAsync(fileName, containerName);
            }

            var profilePictureUrl = await blobSerivice.UploadFileAsync(containerName, profilePicture, fileName, true);

            return profilePictureUrl;
        }

        public async Task<ApiResponse> GetMyToursAsync(string userId, int page = 1, int pageSize = 10)
        {
            var tours = _repository.AllReadOnly<Tour>()
                .Where(t => t.CreatorId == userId)
                .Select(t => new ProfileToursResponseDto
                 {
                     TourId = t.TourId,
                     CreatorId = t.CreatorId,
                     CreatedAt = t.CreatedAt.ToString("dd.MM.yyyy"),
                     ThumbnailImageUrl = t.ThumbnailImageUrl,
                     Destination = t.Destination,
                     Summary = t.Summary,
                     EstimatedDuration = t.EstimatedDuration,
                     Price = t.Price,
                     Status = t.Status == Status.UnderReview ? "Under Review" : t.Status.ToString(),
                     Title = t.Title,
                 });


            var response = await GetResponseAsync(tours, pageSize, page);
            
            return response;

        }
        public async Task<ApiResponse> GetBoughtToursAsync(string userId, int page = 1, int pageSize = 10)
        {
            var tours =  _repository.AllReadOnly<UserTours>()
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.User)
                .Include(ut => ut.Tour)
                .Select(userTour => new ProfileToursResponseDto
                {
                    TourId = userTour.Tour.TourId,
                    CreatorId = userTour.Tour.CreatorId,
                    CreatorName = string.IsNullOrEmpty(userTour.Tour.Creator.FirstName) ? userTour.Tour.Creator.Name : userTour.Tour.Creator.FirstName + " " + userTour.Tour.Creator.LastName,
                    CreatedAt = userTour.Tour.CreatedAt.ToString("dd.MM.yyyy"),
                    ThumbnailImageUrl = userTour.Tour.ThumbnailImageUrl,
                    Destination = userTour.Tour.Destination,
                    Summary = userTour.Tour.Summary,
                    EstimatedDuration = userTour.Tour.EstimatedDuration,
                    Price = userTour.Tour.Price,
                    Status = userTour.Tour.Status == Status.UnderReview ? "Under Review" : userTour.Tour.Status.ToString(),
                    Title = userTour.Tour.Title,
                });
                

            var response = await GetResponseAsync(tours, pageSize, page);


            return response;
        }

        public async Task<ApiResponse> GetUserTransactionsAsync(string userId, int page = 1, int pageSize = 10)
        {
            var transactions = _repository
                               .AllReadOnly<Payment>()
                               .Where(p => p.Status == PaymentStatus.Succeeded && p.UserId == userId)
                               .Select(tr => new UserTransactionsResponseDto
                               {
                                   TourTitle = tr.Tour.Title,
                                   Date = tr.PaymentDate.ToString(TransactionDateFormat),
                                   Price = tr.Amount,
                               });

            var response = await GetResponseAsync(transactions, pageSize, page);

            return response;

        }
        private int CalculatePages(int totalItems, int pageSize)
        {
            return (int)Math.Ceiling(totalItems / (double)pageSize);
        }

        private async Task<ApiResponse> GetResponseAsync(IQueryable<object> results, int pageSize, int page)
        {
            var totalItems = await results.CountAsync();
            var totalPages = CalculatePages(totalItems, pageSize);

            var response = new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = new
                {
                    Data = await (results.Paginate(page, pageSize).ToListAsync()),
                    TotalPages = totalPages
                }
            };

            return response;
        }

        
    }
}
