using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ILandmarkResourceService
    {
        Task CreateLandmarkResourcesAsync(List<IFormFile> resources, Landmark landmark);
        Task UpdateLandmarkResourcesAsync(List<LandmarkResourceUpdateDTO> resources, Landmark landmark);
        Task CreateLandmarkResourcesFromUpdateDtoAsync(List<LandmarkResourceUpdateDTO> resourcesDto, Landmark landmark);
        Task DeleteLandmarkResourcesAsync(ICollection<LandmarkResource> resources);

    }
}
