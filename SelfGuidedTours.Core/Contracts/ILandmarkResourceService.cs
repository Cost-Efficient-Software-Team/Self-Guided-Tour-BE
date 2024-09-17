using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ILandmarkResourceService
    {
        Task CreateLandmarkResourcesAsync(ICollection<IFormFile> resources, Landmark landmark);

        Task UpdateLandmarkResourcesAsync(List<LandmarkResourceUpdateDTO> resourcesDto, Landmark landmark);
    }
}
