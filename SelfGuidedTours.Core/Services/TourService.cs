using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;

namespace SelfGuidedTours.Core.Services
{
    public class TourService : ITourService
    {
        private readonly IRepository repository;
        private readonly IBlobService blobService;
        private readonly ILandmarkService landmarkService;
        private readonly ApiResponse response;

        public TourService(IRepository repository,
                            IBlobService blobService,
                            ILandmarkService landmarkService)
        {
            this.repository = repository;
            this.blobService = blobService;
            this.landmarkService = landmarkService;
            response = new ApiResponse();
        }

        //public async Task<ApiResponse> AddAsync(TourCreateDTO model, string creatorId)
        //{
        //    var tourToAdd = new Tour()
        //    {
        //        Title = model.Title,
        //        Description = model.Description,
        //        Price = model.Price,
        //        Location = model.Location,
        //        ThumbnailImageUrl = model.ThumbnailImageUrl,
        //        EstimatedDuration = model.EstimatedDuration,
        //        CreatorId = creatorId,
        //    };

        //    await repository.AddAsync(tourToAdd);
        //    await repository.SaveChangesAsync();

        //    response.Result = tourToAdd;
        //    response.StatusCode = HttpStatusCode.Created;

        //    return response;
        //}

        public async Task<Tour> CreateTourAsync(TourCreateDTO model, string creatorId)
        {
            if (model == null) throw new ArgumentException();

            var containerName = Environment.GetEnvironmentVariable("THUMBNAIL_CONTAINER");

            if (containerName == null) throw new Exception("Thumbnail Container is not configured");

            var thumbnailUrl = await blobService.UploadFileAsync(containerName, model.ThumbnailImage, model.ThumbnailImage.FileName, true);

            if (thumbnailUrl == null) throw new Exception("Error uploading image");

            var tourToAdd = new Tour
            {
                CreatorId = creatorId,
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Location = model.Location,
                ThumbnailImageUrl = thumbnailUrl,
                EstimatedDuration = model.EstimatedDuration
            };


            await repository.AddAsync(tourToAdd);

            await landmarkService.CreateLandmarskForTourAsync(model.Landmarks, tourToAdd);

            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.Created;

            return tourToAdd;
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

        //public async Task<List<Tour>> GetAllTours()
        //{
        //    return await _context.Tours
        //        .Include(t => t.Landmarks)
        //        .Include(t => t.Payments)
        //        .Include(t => t.Reviews)
        //        .Include(t => t.UserTours)
        //        .ToListAsync();
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
