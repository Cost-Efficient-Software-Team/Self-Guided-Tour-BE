using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;
using static SelfGuidedTours.Common.MessageConstants.LoggerMessages;
using static SelfGuidedTours.Common.Constants.FormatConstants;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using System.Net;
namespace SelfGuidedTours.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository repository;
        private readonly ApiResponse response;
        private readonly ILogger<AdminService> logger;
        private readonly ITourService tourService;

        public AdminService(IRepository repository, ILogger<AdminService> logger, ITourService tourService)
        {
            this.repository = repository;
            this.logger = logger;
            this.tourService = tourService;
            response = new ApiResponse();
        }

        public async Task<ApiResponse> ApproveTourAsync(int id)
        {
            var tour = await repository.GetByIdAsync<Tour>(id)
                ?? throw new KeyNotFoundException(TourNotFoundErrorMessage);

            if (tour.Status == Status.Approved) throw new InvalidOperationException(TourAlreadyApprovedErrorMessage);


            tour.Status = Status.Approved;
            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            response.Result = tourService.MapTourToTourResponseDto(tour);

            return response;
        }

        public async Task<IEnumerable<AllToursToAdminDTO>> GetAllToursAsync(Status status = Status.UnderReview)
        {
            var tours = await repository.AllReadOnly<Tour>()
                .Where(t => t.Status == status)
                .Select(tm => new AllToursToAdminDTO()
                {
                    Id = tm.TourId,
                    Title = tm.Title,
                    Status = status == Status.UnderReview ? "Under Review" : status.ToString(),
                    CreatorName = tm.Creator.Name,
                    CreatedAt = tm.CreatedAt.ToString(DateFormat),
                }).ToListAsync();

            if (tours.Count == 0)
            {
                logger.LogWarning(WarningMessageForNotFoundedToursForAdmin);
                throw new KeyNotFoundException(WarningMessageForNotFoundedToursForAdmin);
            }

            logger.LogInformation(InformationMessageForAllSuccessfullyReturnedToursForAdmin, status);

            return tours;
        }

        public async Task<ApiResponse> RejectTourAsync(int id)
        {
            var tour = await repository.GetByIdAsync<Tour>(id)
                ?? throw new KeyNotFoundException(TourNotFoundErrorMessage);

            if (tour.Status == Status.Declined) throw new InvalidOperationException(TourAlreadyRejectedErrorMessage);

            tour.Status = Status.Declined;
            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            response.Result = tourService.MapTourToTourResponseDto(tour);

            return response;
        }
    }
}
