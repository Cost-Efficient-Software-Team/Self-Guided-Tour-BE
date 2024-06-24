using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Data.Enums;
using System.Net;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class TourService : ITourService
    {
        private readonly IRepository repository;
        private readonly ApiResponse response;

        public TourService(IRepository repository)
        {
             this.repository = repository;
             response = new ApiResponse();
        }

        public async Task<ApiResponse> AddAsync(TourCreateDTO model, string creatorId)
        {
            var tourToAdd = new Tour()
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Location = model.Location,
                CreatedAt = DateTime.Now,
                ThumbnailImageUrl = model.ThumbnailImageUrl,
                EstimatedDuration = model.EstimatedDuration,
                CreatorId = creatorId,
                Status = Status.Pending,
                UpdatedAt = DateTime.Now
            };

            await repository.AddAsync(tourToAdd);
            await repository.SaveChangesAsync();

            response.Result = tourToAdd;
            response.StatusCode = HttpStatusCode.Created;

            return response;
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
