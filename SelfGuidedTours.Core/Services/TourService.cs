using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.ResponseDto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Enums;
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

        public async Task<Tour> CreateTourAsync(TourCreateDTO model, string creatorId)
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
                EstimatedDuration = model.EstimatedDuration,
                TypeTour = model.TypeTour
            };

            await repository.AddAsync(tourToAdd);
            await landmarkService.CreateLandmarksForTourAsync(model.Landmarks, tourToAdd);
            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.Created;
            return tourToAdd;
        }

        public async Task<ApiResponse> UpdateTourAsync(int id, TourUpdateDTO model)
        {
            var tour = await repository.GetByIdAsync<Tour>(id)
                       ?? throw new KeyNotFoundException(TourNotFoundErrorMessage);

            tour.Title = model.Title;
            tour.Summary = model.Summary;
            tour.Price = model.Price;
            tour.Destination = model.Destination;
            tour.EstimatedDuration = model.EstimatedDuration;
            tour.TypeTour = model.TypeTour;
            tour.UpdatedAt = DateTime.Now;
            tour.Status = Status.UnderReview;
            if (model.ThumbnailImage != null)
            {
                var containerName = Environment.GetEnvironmentVariable("CONTAINER_NAME")
                                    ?? throw new ApplicationException(ContainerNameErrorMessage);

                await blobService.DeleteFileAsync(tour.ThumbnailImageUrl, containerName);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ThumbnailImage.FileName)}";
                var thumbnailUrl = await blobService.UploadFileAsync(containerName, model.ThumbnailImage, fileName, true);

                tour.ThumbnailImageUrl = thumbnailUrl;
            }

            await landmarkService.UpdateLandmarksForTourAsync(model.Landmarks, tour);
            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            response.Result = MapTourToTourResponseDto(tour);
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

            if (tour.Status != Status.Approved)
            {
                throw new UnauthorizedAccessException("This tour is under review and cannot be accessed.");
            }
            return tour;
        }

        public async Task<(List<Tour> Tours, int TotalPages)> GetFilteredTours(string searchTerm, string sortBy, int pageNumber = 1, int pageSize = 1000)
        {
            //var query = repository.All<Tour>().AsQueryable(); //Turn On if you want to test faster  // Use this to not confirm each tour during development  //remove during production
            var query = repository.All<Tour>()
                .Where(t => t.Status == Status.Approved)
                .AsQueryable();//Turn Off is the above code is On!

            //filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.Destination.Contains(searchTerm)
                                         || t.Title.Contains(searchTerm)
                                         || t.Summary.Contains(searchTerm));
            }

            //sorting
            switch (sortBy)
            {
                case "newest":
                    query = query.OrderByDescending(t => t.CreatedAt);
                    break;
                case "averageRating":
                    query = query.OrderByDescending(t => t.AverageRating);
                    break;
                case "mostBought":
                    query = query.OrderByDescending(t => t.Payments.Count);
                    break;
                case "minPrice":
                    query = query.OrderBy(t => t.Price);
                    break;
                case "maxPrice":
                    query = query.OrderByDescending(t => t.Price);
                    break;
                default:
                    query = query.OrderBy(t => t.Destination)
                        .ThenBy(t => t.Title)
                        .ThenBy(t => t.Summary);
                    break;
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var skip = (pageNumber - 1) * pageSize;
            var tours = await query.Skip(skip).Take(pageSize)
                .Include(t => t.Reviews)
                .Include(t => t.Landmarks)
                .ThenInclude(l => l.Resources)
                .Include(t => t.Landmarks)
                .ThenInclude(l => l.Coordinate)
                .ToListAsync();

            return (tours, totalPages);
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

        public TourResponseDto MapTourToTourResponseDto(Tour tour)
        {
            var tourResponse = new TourResponseDto
            {
                TourId = tour.TourId,
                CreatorId = tour.CreatorId,
                CreatedAt = tour.CreatedAt.ToString("dd.MM.yyyy"),
                ThumbnailImageUrl = tour.ThumbnailImageUrl,
                Destination = tour.Destination,
                Summary = tour.Summary,
                EstimatedDuration = tour.EstimatedDuration,
                Price = tour.Price,
                Status = tour.Status == Status.UnderReview ? "Under Review" : tour.Status.ToString(),
                Title = tour.Title,
                TourType = tour.TypeTour.ToString(),
                AverageRating = tour.AverageRating,
                Landmarks = tour.Landmarks.Select(l => new LandmarkResponseDto
                {
                    LandmarkId = l.LandmarkId,
                    LocationName = l.LocationName,
                    Description = l.Description,
                    StopOrder = l.StopOrder,
                    City = l.Coordinate.City,
                    Latitude = l.Coordinate.Latitude,
                    Longitude = l.Coordinate.Longitude,
                    PlaceId = l.PlaceId,
                    Resources = l.Resources.Select(r => new LandmarkResourceResponseDto
                    {
                        ResourceId = r.LandmarkResourceId,
                        ResourceUrl = r.Url,
                        ResourceType = r.Type.ToString()
                    }).ToList()
                }).ToList()
            };
            return tourResponse;
        }
    }
}
