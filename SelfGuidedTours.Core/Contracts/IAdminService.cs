using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Data.Enums;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IAdminService
    {
        Task<ApiResponse> GetAllToursAsync(Status status);
    }
}
