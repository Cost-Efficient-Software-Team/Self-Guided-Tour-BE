using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ILandmarkService
    {
        Task<ICollection<Landmark>> CreateLandmarksForTourAsync(ICollection<LandmarkCreateTourDTO> landmarksDto, Tour tour);
        Task<ICollection<Landmark>> UpdateLandmarksForTourAsync(ICollection<LandmarkUpdateTourDTO> landmarksDto, Tour tour);
    }
}
