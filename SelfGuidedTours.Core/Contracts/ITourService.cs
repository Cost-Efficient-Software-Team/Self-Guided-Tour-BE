using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ITourService
    {
        Task<ApiResponse> AddAsync(TourCreateDTO model, string creatorId);
        Task<Tour> GetTourById(int id);
    }
}
