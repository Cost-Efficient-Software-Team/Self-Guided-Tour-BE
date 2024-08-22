using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.ResponseDto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ITourService
    {
        Task<List<Tour>> GetFilteredTours(string searchTerm, string sortBy, int pageNumber = 1, int pageSize = 1000);

        Task<Tour> CreateAsync(TourCreateDTO model, string creatorId);

        Task<ApiResponse> DeleteTourAsync(int id);

        Task<Tour?> GetTourByIdAsync(int id);

        TourResponseDto MapTourToTourResponseDto(Tour tour);

        Task<ApiResponse> UpdateTourAsync(int id, TourUpdateDTO model);

        Task<int> GetTotalPagesNumberAsync(string searchTerm, string sortBy, int pageSize);
    }
}
