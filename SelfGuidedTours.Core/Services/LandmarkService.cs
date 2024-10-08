﻿using Microsoft.EntityFrameworkCore;
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
            var landmarksToAdd = new List<Landmark>();
            foreach (var landmarkDto in landmarksDto)
            {
                var coordinate = new Coordinate
                {
                    Latitude = landmarkDto.Latitude,
                    Longitude = landmarkDto.Longitude,
                    City = string.IsNullOrWhiteSpace(landmarkDto.City) ? null : landmarkDto.City
                };
                await repository.AddAsync(coordinate);

                var landmark = new Landmark
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

            return landmarksToAdd;
        }

        public async Task<ICollection<Landmark>> UpdateLandmarksForTourAsync(ICollection<LandmarkUpdateTourDTO> landmarksDto, Tour tour)
        {
            if (landmarksDto.Count == 0)
                throw new ArgumentException(TourWithNoLandmarksErrorMessage);

            var existingLandmarks = await repository.All<Landmark>()
                .Include(l => l.Coordinate)
                .Include(l => l.Resources)
                .Where(l => l.TourId == tour.TourId)
                .ToListAsync();

            // Delete landmarks that are no longer in the updated list
            var landmarksToDelete = existingLandmarks
                .Where(el => !landmarksDto.Any(ldto => ldto.LandmarkId == el.LandmarkId))
                .ToList();

            foreach (var landmarkToDelete in landmarksToDelete)
            {
                // Delete associated resources
                await resourceService.DeleteLandmarkResourcesAsync(landmarkToDelete.Resources);

                // Delete coordinate
                repository.Delete(landmarkToDelete.Coordinate);

                // Delete landmark
                repository.Delete(landmarkToDelete);
            }

            var landmarksToUpdate = new List<Landmark>();

            foreach (var landmarkDto in landmarksDto)
            {
                Landmark? existingLandmark = null;
                if (landmarkDto.LandmarkId.HasValue)
                {
                    existingLandmark = existingLandmarks.FirstOrDefault(l => l.LandmarkId == landmarkDto.LandmarkId.Value);
                }

                if (existingLandmark != null)
                {
                    existingLandmark.LocationName = landmarkDto.LocationName;
                    existingLandmark.Description = landmarkDto.Description;
                    existingLandmark.StopOrder = landmarkDto.StopOrder;
                    existingLandmark.Coordinate.Latitude = landmarkDto.Latitude;
                    existingLandmark.Coordinate.Longitude = landmarkDto.Longitude;
                    existingLandmark.Coordinate.City = landmarkDto.City;
                    existingLandmark.PlaceId = landmarkDto.PlaceId;
                    existingLandmark.UpdatedAt = DateTime.Now;

                    await resourceService.UpdateLandmarkResourcesAsync(landmarkDto.Resources, existingLandmark);
                    landmarksToUpdate.Add(existingLandmark);
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
                    var newLandmark = new Landmark
                    {
                        LocationName = landmarkDto.LocationName,
                        Description = landmarkDto.Description,
                        Coordinate = coordinate,
                        StopOrder = landmarkDto.StopOrder,
                        Tour = tour,
                        PlaceId = landmarkDto.PlaceId,
                        CreatedAt = DateTime.Now
                    };

                    await repository.AddAsync(newLandmark);
                    await resourceService.CreateLandmarkResourcesFromUpdateDtoAsync(landmarkDto.Resources, newLandmark);
                    landmarksToUpdate.Add(newLandmark);
                }
            }
            await repository.SaveChangesAsync();
            return landmarksToUpdate;
        }


    }
}
