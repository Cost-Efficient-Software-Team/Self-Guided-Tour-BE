using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.ResponseDto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ITourService
    {
        Task<List<Tour>> GetFilteredTours(string title, string destination, decimal? minPrice, decimal? maxPrice,
            int? minEstimatedDuration, int? maxEstimatedDuration);

        Task<Tour> CreateAsync(TourCreateDTO model, string creatorId);

        Task<ApiResponse> DeleteTourAsync(int id);

        Task<Tour?> GetTourByIdAsync(int id);

        TourResponseDto MapTourToTourResponseDto(Tour tour);

    }
}
