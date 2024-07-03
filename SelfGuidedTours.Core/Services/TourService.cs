using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SelfGuidedTours.Infrastructure.Data.Enums;

namespace SelfGuidedTours.Core.Services
{
    public class TourService : ITourService
    {
        private readonly IRepository repository;
        private readonly ApiResponse response;
        private readonly IBlobService blobService;

        public TourService(IRepository repository, IBlobService blobService)
        {
            this.repository = repository;
            this.blobService = blobService;
            response = new ApiResponse();
        }

        public async Task<ApiResponse> AddAsync(TourCreateDTO model, string creatorId)
        {
            var blobNameThumnailImage = $"{Guid.NewGuid()}_{model.ThumbnailImageUrl.FileName}";
            await blobService.UploadFileAsync(model.ThumbnailImageUrl, blobNameThumnailImage);

            //Tour
            var tourToAdd = new Tour()
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Location = model.Location,
                ThumbnailImageUrl = blobService.GetFileUrl(blobNameThumnailImage),
                EstimatedDuration = model.EstimatedDuration,
                CreatorId = creatorId
            };

            await repository.AddAsync(tourToAdd);
            await repository.SaveChangesAsync();

            //Coordinate
            foreach (var landmark in model.Landmarks)
            {
                var coordinate = landmark.Coordinates;
                var coordinateToAdd = new Coordinate()
                {
                    Latitude = coordinate.Latitude,
                    Longitude = coordinate.Longitude,
                    City = coordinate.City,
                    Country = coordinate.Country
                };

                await repository.AddAsync(coordinateToAdd);
                await repository.SaveChangesAsync();

                var coordinateFromDb = await repository.AllReadOnly<Coordinate>()
                    .Where(c => c.Latitude == coordinate.Latitude && c.Longitude == coordinate.Longitude)
                    .FirstAsync();

                var tourFromDb = await repository.AllReadOnly<Tour>()
                    .Where(t => t.Title == tourToAdd.Title && tourToAdd.Description == tourToAdd.Description)
                    .FirstAsync();

                var landmarkToAdd = new Landmark()
                {
                    CoordinateId = coordinateFromDb.CoordinateId,
                    Name = landmark.Name,
                    Description = landmark.Description,
                    TourId = tourFromDb.TourId
                };

                tourToAdd.Landmarks.Add(landmarkToAdd);

                await repository.AddAsync(landmarkToAdd);
                await repository.SaveChangesAsync();

                var landmarkFromDb = await repository.AllReadOnly<Landmark>()
                    .Where(l => l.CoordinateId == coordinateFromDb.CoordinateId && l.Name == landmarkToAdd.Name && landmarkToAdd.Description == landmarkToAdd.Description)
                    .FirstAsync();

                var landmarkResources = landmark.Resources;
                foreach (var resource in landmarkResources!)
                {
                    if (resource.Length > 0)
                    {
                        var blobName = $"{Guid.NewGuid()}_{resource.FileName}";
                        await blobService.UploadFileAsync(resource, blobName);

                        var landmarkResourceToAdd = new LandmarkResource()
                        {
                            LandmarkId = landmarkFromDb.LandmarkId,
                            Type = GetResourceType(resource.ContentType),
                            Url = blobService.GetFileUrl(blobName),
                        };
                        await repository.AddAsync(landmarkResourceToAdd);
                    }
                }

                await repository.SaveChangesAsync();
            }

            response.Result = tourToAdd;
            response.StatusCode = HttpStatusCode.Created;

            return response;
        }
        private ResourceType GetResourceType(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => ResourceType.Image,
                "image/png" => ResourceType.Image,
                "video/mp4" => ResourceType.Video,
                "audio/mpeg" => ResourceType.Audio,
                "text/plain" => ResourceType.Text,
                "application/pdf" => ResourceType.Text,
                _ => ResourceType.Unknown
            };
        }

        public async Task<List<Tour>> GetAllTours()
        {
            return await repository.All<Tour>()
                .Include(t => t.Landmarks)
                .Include(t => t.Payments)
                .Include(t => t.Reviews)
                .Include(t => t.UserTours)
                .ToListAsync();
        }

        public async Task<List<Tour>> GetFilteredTours(string title, string location, decimal? minPrice, decimal? maxPrice, int? minEstimatedDuration, int? maxEstimatedDuration)
        {
            var query = repository.All<Tour>().AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(t => t.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(t => t.Location.Contains(location));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(t => t.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(t => t.Price <= maxPrice);
            }

            if (minEstimatedDuration.HasValue)
            {
                query = query.Where(t => t.EstimatedDuration >= minEstimatedDuration);
            }

            if (maxEstimatedDuration.HasValue)
            {
                query = query.Where(t => t.EstimatedDuration <= maxEstimatedDuration);
            }

            return await query
                .Include(t => t.Landmarks)
                .Include(t => t.Payments)
                .Include(t => t.Reviews)
                .Include(t => t.UserTours)
                .ToListAsync();
        }

        //public async Task<Tour> GetTourById(int id)
        //{
        //    return await _context.Tours
        //        .Include(t => t.Landmarks)
        //        .Include(t => t.Payments)
        //        .Include(t => t.Reviews)
        //        .Include(t => t.UserTours)
        //        .FirstOrDefaultAsync(t => t.TourId == id);
        //}

        //public async Task<ApiResponse> UpdateTour(int id, TourUpdateDTO tourUpdateDTO)
        //{
        //    var response = new ApiResponse();

        //    var tour = await _context.Tours.FindAsync(id);
        //    if (tour == null)
        //    {
        //        response.StatusCode = HttpStatusCode.NotFound;
        //        response.IsSuccess = false;
        //        return response;
        //    }

        //    tour.Title = tourUpdateDTO.Title;
        //    tour.Description = tourUpdateDTO.Description;
        //    tour.Price = tourUpdateDTO.Price;
        //    tour.Location = tourUpdateDTO.Location;
        //    tour.ThumbnailImageUrl = tourUpdateDTO.ThumbnailImageUrl;
        //    tour.EstimatedDuration = tourUpdateDTO.EstimatedDuration;
        //    tour.UpdatedAt = DateTime.Now;
        //    tour.Status = Status.Pending;

        //    _context.Entry(tour).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();

        //    response.StatusCode = HttpStatusCode.NoContent;
        //    return response;
        //}

        //public async Task<ApiResponse> DeleteTour(int id)
        //{
        //    var response = new ApiResponse();

        //    var tour = await _context.Tours.FindAsync(id);
        //    if (tour == null)
        //    {
        //        response.StatusCode = HttpStatusCode.BadRequest;
        //        response.IsSuccess = false;
        //        return response;
        //    }

        //    _context.Tours.Remove(tour);
        //    await _context.SaveChangesAsync();

        //    response.StatusCode = HttpStatusCode.NoContent;
        //    return response;
        //}

        //public bool TourExists(int id)
        //{
        //    return _context.Tours.Any(e => e.TourId == id);
        //}

    }
}
