﻿using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class LandmarkService : ILandmarkService
    {
        private readonly IRepository repository;
        private readonly IResourceService resourceService;

        public LandmarkService(IRepository repository, IResourceService resourceService)
        {
            this.repository = repository;
            this.resourceService = resourceService;
        }
        public async Task<ICollection<Landmark>> CreateLandmarskForTourAsync(ICollection<LandmarkCreateTourDTO> landmarksDto, Tour tour)
        {
            if (landmarksDto.Count == 0) throw new ArgumentException("A tour must have at least one landmark");
            var ladnmarksToAdd = new List<Landmark>();
            foreach (var landmarkDto in landmarksDto)
            {
                var cordinate = new Coordinate
                {
                    Latitude = landmarkDto.Latitude,
                    Longitude = landmarkDto.Longitude
                };

                await repository.AddAsync(cordinate);
               // await repository.SaveChangesAsync();

                var landmark = new Landmark
                {
                    Name = landmarkDto.Name,
                    Description = landmarkDto.Description,
                   // CoordinateId = cordinate.CoordinateId,
                    //TourId = tourId,
                    Coordinate = cordinate,
                    Tour = tour
                };

                await repository.AddAsync(landmark);
                //await repository.SaveChangesAsync();

                await resourceService.CreateLandmarkResoursecAsync(landmarkDto.Resources, landmark);

            }

            return ladnmarksToAdd;
        }
    }
}