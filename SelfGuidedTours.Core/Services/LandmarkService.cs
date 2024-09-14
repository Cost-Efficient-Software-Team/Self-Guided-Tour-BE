using Microsoft.EntityFrameworkCore;
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
                    Tour = tour,
                    PlaceId = landmarkDto.PlaceId
                };

                await repository.AddAsync(landmark);

                await resourceService.CreateLandmarkResourcesAsync(landmarkDto.Resources!, landmark);
            }

            return ladnmarksToAdd;
        }

        public async Task<ICollection<Landmark>> UpdateLandmarksForTourAsync(ICollection<LandmarkUpdateTourDTO> landmarksDto, Tour tour)
        {
            if (landmarksDto.Count == 0)
                throw new ArgumentException(TourWithNoLandmarksErrorMessage);

            var landmarksToUpdate = new List<Landmark>();

            foreach (var landmarkDto in landmarksDto)
            {
                var existingLandmark = await repository.All<Landmark>()
                    .Include(l => l.Coordinate)
                    .FirstOrDefaultAsync(l => l.PlaceId == landmarkDto.PlaceId && l.TourId == tour.TourId);

                Landmark landmark;

                if (existingLandmark != null)
                {
                    existingLandmark.LocationName = landmarkDto.LocationName;
                    existingLandmark.Description = landmarkDto.Description;
                    existingLandmark.StopOrder = landmarkDto.StopOrder;

                    existingLandmark.Coordinate.Latitude = landmarkDto.Latitude;
                    existingLandmark.Coordinate.Longitude = landmarkDto.Longitude;
                    existingLandmark.Coordinate.City = landmarkDto.City;

                    await resourceService.UpdateLandmarkResourcesAsync(landmarkDto.Resources, existingLandmark);

                    landmark = existingLandmark;
                }
                else
                {
                    var coordinate = new Coordinate
                    {
                        Latitude = landmarkDto.Latitude,
                        Longitude = landmarkDto.Longitude,
                        City = landmarkDto.City
                    };

                    await repository.AddAsync(coordinate);

                    landmark = new Landmark
                    {
                        LocationName = landmarkDto.LocationName,
                        Description = landmarkDto.Description,
                        Coordinate = coordinate,
                        StopOrder = landmarkDto.StopOrder,
                        Tour = tour,
                        PlaceId = landmarkDto.PlaceId
                    };

                    await repository.AddAsync(landmark);
                    await resourceService.CreateLandmarkResourcesAsync(landmarkDto.Resources, landmark);
                }
                landmarksToUpdate.Add(landmark);
            }

            await repository.SaveChangesAsync();

            return landmarksToUpdate;
        }


    }
}
