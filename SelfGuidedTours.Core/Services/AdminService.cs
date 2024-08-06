using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;
using static SelfGuidedTours.Common.MessageConstants.LoggerMessages;

namespace SelfGuidedTours.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository repository;
        private readonly ApiResponse response;
        private readonly ILogger<AdminService> logger;
        public AdminService(IRepository repository, ILogger<AdminService> logger)
        {
            this.repository = repository;
            this.logger = logger;
            response = new ApiResponse();
        }
        public async Task<IEnumerable<AllToursToAdminDTO>> GetAllToursAsync(Status status = Status.UnderReview)
        {
            var tours = await repository.AllReadOnly<Tour>()
                .Where(t => t.Status == status)
                .Select(tm => new AllToursToAdminDTO()
                {
                    Id = tm.TourId,
                    Title = tm.Title,
                    Status = status.ToString(),
                    CreatorName = tm.Creator.Name
                }).ToListAsync();

            if (!tours.Any())
            {
                logger.LogWarning(WarningMessageForNotFoundedToursForAdmin);
                throw new KeyNotFoundException(WarningMessageForNotFoundedToursForAdmin);
            }

            logger.LogInformation(InformationMessageForAllSucessfullyReturnedToursForAdmin, status);

            return tours;
        }
    }
}
