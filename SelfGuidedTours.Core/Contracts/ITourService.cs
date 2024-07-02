using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ITourService
    {
        Task<ApiResponse> AddAsync(TourCreateDTO model, string creatorId);

        Task<List<Tour>> GetFilteredTours(string title, string location, decimal? minPrice, decimal? maxPrice,
            int? minEstimatedDuration, int? maxEstimatedDuration);
    }
}
