using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Services
{
    public class TourService
    {
        private readonly SelfGuidedToursDbContext _context;

        public TourService(SelfGuidedToursDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> CreateTour(TourCreateDTO tourCreateDTO, string creatorId)
        {
            var response = new ApiResponse();

            Tour tourToCreate = new Tour
            {
                Title = tourCreateDTO.Title,
                Description = tourCreateDTO.Description,
                Price = tourCreateDTO.Price,
                Location = tourCreateDTO.Location,
                CreatedAt = DateTime.Now,
                ThumbnailImageUrl = tourCreateDTO.ThumbnailImageUrl,
                EstimatedDuration = tourCreateDTO.EstimatedDuration,
                CreatorId = creatorId,
                Status = Status.Pending,
                UpdatedAt = DateTime.Now
            };

            _context.Tours.Add(tourToCreate);
            await _context.SaveChangesAsync();
            response.Result = tourToCreate;
            response.StatusCode = HttpStatusCode.Created;

            return response;
        }

        public async Task<Tour> GetTourById(int id)
        {
            return await _context.Tours
                .Include(t => t.Landmarks)
                .Include(t => t.Payments)
                .Include(t => t.Reviews)
                .Include(t => t.UserTours)
                .FirstOrDefaultAsync(t => t.TourId == id);
        }

        public async Task<List<Tour>> GetAllTours()
        {
            return await _context.Tours
                .Include(t => t.Landmarks)
                .Include(t => t.Payments)
                .Include(t => t.Reviews)
                .Include(t => t.UserTours)
                .ToListAsync();
        }

        public async Task<ApiResponse> UpdateTour(int id, TourUpdateDTO tourUpdateDTO)
        {
            var response = new ApiResponse();

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                return response;
            }

            tour.Title = tourUpdateDTO.Title;
            tour.Description = tourUpdateDTO.Description;
            tour.Price = tourUpdateDTO.Price;
            tour.Location = tourUpdateDTO.Location;
            tour.ThumbnailImageUrl = tourUpdateDTO.ThumbnailImageUrl;
            tour.EstimatedDuration = tourUpdateDTO.EstimatedDuration;
            tour.UpdatedAt = DateTime.Now;
            tour.Status = Status.Pending;

            _context.Entry(tour).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.NoContent;
            return response;
        }

        public async Task<ApiResponse> DeleteTour(int id)
        {
            var response = new ApiResponse();

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                return response;
            }

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.NoContent;
            return response;
        }

        public bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.TourId == id);
        }
    }
}
