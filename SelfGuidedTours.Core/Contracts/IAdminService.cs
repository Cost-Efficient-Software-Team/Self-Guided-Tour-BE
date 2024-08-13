using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Enums;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IAdminService
    {
        Task<IEnumerable<AllToursToAdminDTO>> GetAllToursAsync(Status status = Status.UnderReview);
        Task<ApiResponse> ApproveTourAsync(int id);

        Task<ApiResponse> RejectTourAsync(int id);
    }
}
