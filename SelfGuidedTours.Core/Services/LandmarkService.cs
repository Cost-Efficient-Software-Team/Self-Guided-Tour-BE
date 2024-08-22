using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
namespace SelfGuidedTours.Core.Services
{
    public class LandmarkService : ILandmarkService
    {
        private readonly IRepository repository;
        private readonly ILandmarkResourceService resourceService;

        public LandmarkService(IRepository repository, ILandmarkResourceService resourceService)
        {
            this.repository = repository;
            this.resourceService = resourceService;
        }
        public async Task<ICollection<Landmark>> CreateLandmarksForTourAsync(ICollection<LandmarkCreateTourDTO> landmarksDto, Tour tour)
        {
            if (landmarksDto.Count == 0) throw new ArgumentException(TourWithNoLandmarksErrorMessage);
            var ladnmarksToAdd = new List<Landmark>();
            foreach (var landmarkDto in landmarksDto)
            {
                var cordinate = new Coordinate
                {
                    Latitude = landmarkDto.Latitude,
                    Longitude = landmarkDto.Longitude,
                    City = landmarkDto.City
                };

                await repository.AddAsync(cordinate);

                var landmark = new Landmark
                {
                    LocationName = landmarkDto.LocationName,
                    Description = landmarkDto.Description,
                    Coordinate = cordinate,
                    StopOrder = landmarkDto.StopOrder,
                    Tour = tour
                };

                await repository.AddAsync(landmark);

                await resourceService.CreateLandmarkResourcesAsync(landmarkDto.Resources!, landmark);
            }

            return ladnmarksToAdd;
        }

        public async Task<ICollection<Landmark>> UpdateLandmarksForTourAsync(ICollection<LandmarkUpdateTourDTO> landmarksDto, Tour tour)
        {
            if (landmarksDto.Count == 0) throw new ArgumentException(TourWithNoLandmarksErrorMessage);
            var ladnmarksToAdd = new List<Landmark>();
            foreach (var landmarkDto in landmarksDto)
            {
                var cordinate = new Coordinate
                {
                    Latitude = landmarkDto.Latitude,
                    Longitude = landmarkDto.Longitude,
                    City = landmarkDto.City
                };

                await repository.AddAsync(cordinate);

                var landmark = new Landmark
                {
                    LocationName = landmarkDto.LocationName,
                    Description = landmarkDto.Description,
                    Coordinate = cordinate,
                    StopOrder = landmarkDto.StopOrder,
                    Tour = tour
                };

                await repository.AddAsync(landmark);

                await resourceService.UpdateLandmarkResourcesAsync(landmarkDto.Resources!, landmark);
            }

            return ladnmarksToAdd;
        }
    }
}
