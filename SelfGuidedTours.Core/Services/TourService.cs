using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.ResponseDto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;

namespace SelfGuidedTours.Core.Services
{
    public class TourService : ITourService
    {
        private readonly IRepository repository;
        private readonly ApiResponse response;
        private readonly IBlobService blobService;
        private readonly ILandmarkService landmarkService;

        public TourService(IRepository repository, IBlobService blobService, ILandmarkService landmarkService)
        {
            this.repository = repository;
            this.blobService = blobService;
            response = new ApiResponse();
            this.landmarkService = landmarkService;
        }

        public async Task<Tour> CreateAsync(TourCreateDTO model, string creatorId)
        {
            if (model == null) throw new ArgumentException();

            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");

            if (containerName == null) throw new Exception(ContainerNameErrorMessage);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ThumbnailImage.FileName)}";
            var thumbnailUrl = await blobService.UploadFileAsync(containerName, model.ThumbnailImage, fileName, true);

            if (thumbnailUrl == null) throw new Exception(BlobStorageErrorMessage);

            var tourToAdd = new Tour
            {
                CreatorId = creatorId,
                Title = model.Title,
                Summary = model.Summary,
                Price = model.Price,
                Destination = model.Destination,
                ThumbnailImageUrl = thumbnailUrl,
                EstimatedDuration = model.EstimatedDuration
            };


            await repository.AddAsync(tourToAdd);

            await landmarkService.CreateLandmarskForTourAsync(model.Landmarks, tourToAdd);

            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.Created;

            return tourToAdd;
        }

        public async Task<ApiResponse> DeleteTourAsync(int id)
        {
            var response = new ApiResponse();

            var tour = await repository.GetByIdAsync<Tour>(id);
            if (tour == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                return response;
            }

            var landmarks = await repository.All<Landmark>().Where(l => l.TourId == id).ToListAsync();

            var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME") ??
                         throw new ApplicationException(ContainerNameErrorMessage);

            await blobService.DeleteFileAsync(tour.ThumbnailImageUrl, containerName);

            foreach (var landmark in landmarks)
            {
                var resources = await repository.All<LandmarkResource>().Where(r => r.LandmarkId == landmark.LandmarkId).ToListAsync();
                foreach (var resource in resources)
                {
                    await blobService.DeleteFileAsync(resource.Url, containerName);
                    repository.Delete(resource);
                }

                var coordinates = await repository.All<Coordinate>().Where(r => r.CoordinateId == landmark.CoordinateId).ToListAsync();
                foreach (var coordinate in coordinates)
                {
                    repository.Delete(coordinate);
                }
            }

            await repository.DeleteAllAsync(landmarks);

            repository.Delete(tour);
            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.NoContent;
            return response;
        }
        public async Task<Tour?> GetTourByIdAsync(int id)
        {
            var tour = await repository.AllReadOnly<Tour>()
                .Include(t => t.Landmarks)
                .ThenInclude(l => l.Resources)
                .Include(t => t.Landmarks)
                .ThenInclude(l => l.Coordinate)
                .FirstOrDefaultAsync(t => t.TourId == id);

            if (tour == null)
            {
                throw new KeyNotFoundException(TourNotFoundErrorMessage);
            }

            return tour;
        }

        public TourResponseDto MapTourToTourResponseDto(Tour tour)
        {
            var tourResponse = new TourResponseDto
            {
                TourId = tour.TourId,
                ThumbnailImageUrl = tour.ThumbnailImageUrl,
                Destination = tour.Destination,
                Summary = tour.Summary,
                EstimatedDuration = tour.EstimatedDuration,
                Price = tour.Price,
                Status = tour.Status.ToString(),
                Title = tour.Title,
                Landmarks = tour.Landmarks.Select(l => new LandmarkResponseDto
                {
                    LandmarkId = l.LandmarkId,
                    LocationName = l.LocationName,
                    Description = l.Description,
                    StopOrder = l.StopOrder,
                    City = l.Coordinate.City,
                    Latitude = l.Coordinate.Latitude,
                    Longitude = l.Coordinate.Longitude,
                    Resources = l.Resources.Select(r => new ResourceResponseDto
                    {
                        ResourceId = r.LandmarkResourceId,
                        ResourceUrl = r.Url,
                        ResourceType = r.Type.ToString()
                    }).ToList()
                }).ToList()
            };
            return tourResponse;
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

        public async Task<List<Tour>> GetFilteredTours(string searchTerm)
        {
            var query = repository.All<Tour>().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.Destination.Contains(searchTerm)
                                         || t.Title.Contains(searchTerm)
                                         || t.Summary.Contains(searchTerm));
            }

            query = query.OrderBy(t => t.Destination)
                .ThenBy(t => t.Title)
                .ThenBy(t => t.Summary);

            return await query
                .Include(t => t.Landmarks)
                .Include(t => t.Payments)
                .Include(t => t.Reviews)
                .Include(t => t.UserTours)
                .ToListAsync();
        }


    }
}
