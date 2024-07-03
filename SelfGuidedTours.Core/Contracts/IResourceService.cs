using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IResourceService
    {
        Task CreateLandmarkResoursecAsync(ICollection<IFormFile> resources, Landmark landmark);
    }
}
