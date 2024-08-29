using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ILandmarkResourceService
    {
        Task CreateLandmarkResourcesAsync(ICollection<IFormFile> resources, Landmark landmark);

        Task UpdateLandmarkResourcesAsync(ICollection<IFormFile> resources, Landmark landmark);
    }
}
