using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ILandmarkResourceService
    {
        Task CreateLandmarkResourcesAsync(List<IFormFile> resources, Landmark landmark);
        Task UpdateLandmarkResourcesAsync(List<IFormFile> resources, List<int> resourcesToDelete, Landmark landmark);
    }
}
