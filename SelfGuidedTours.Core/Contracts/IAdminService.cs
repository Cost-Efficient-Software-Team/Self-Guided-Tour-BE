using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Enums;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IAdminService
    {
        Task<IEnumerable<AllToursToAdminDTO>> GetAllToursAsync(Status status);
    }
}
