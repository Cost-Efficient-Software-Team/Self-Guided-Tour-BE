using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ITourService
    {
        Task<ApiResponse> AddAsync(TourCreateDTO model, string creatorId);
    }
}
