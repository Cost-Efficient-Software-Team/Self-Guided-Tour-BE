using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ILandmarkResourceService
    {
        Task CreateLandmarkResourcesAsync(List<LandmarkResourceUpdateDTO> resourcesDto, Landmark landmark);
        Task UpdateLandmarkResourcesAsync(List<LandmarkResourceUpdateDTO> resourcesDto, Landmark landmark);
    }
}
