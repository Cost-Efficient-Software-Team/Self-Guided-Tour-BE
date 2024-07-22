using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ILandmarkService
    {
        Task<ICollection<Landmark>> CreateLandmarskForTourAsync(ICollection<LandmarkCreateTourDTO> landmarksDto, Tour tour);
    }
}
