using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto.Review;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;

namespace SelfGuidedTours.Core.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository repository;
        private readonly ApiResponse response;

        public ReviewService(IRepository repository)
        {
            this.repository = repository;
            response = new ApiResponse();
        }

        public async Task<Review> CreateAsync(ReviewCreateDTO model, string userId, int tourId)
        {
            if (model == null) throw new ArgumentException("Model cannot be null");

            var user = await repository.GetByIdAsync<ApplicationUser>(userId);
            if (user == null) throw new ArgumentException("Invalid userId");

            var tour = await repository.GetByIdAsync<Tour>(tourId);
            if (tour == null) throw new ArgumentException("Invalid tourId");

            var reviewToAdd = new Review
            {
                UserId = userId,
                TourId = tourId,
                Rating = model.Rating,
                Comment = model.Comment,
                ReviewDate = DateTime.Now
            };

            try
            {
                await repository.AddAsync(reviewToAdd);
                await repository.SaveChangesAsync();
                response.StatusCode = HttpStatusCode.Created;
                return reviewToAdd;
            }
            catch (DbUpdateException dbEx)
            {
                // Log the database update exception
                Console.WriteLine($"Database update error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Message = "A database error occurred while saving the review. Please try again later.";
                response.ErrorMessages.Add(dbEx.InnerException?.Message ?? dbEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Log the general exception
                Console.WriteLine($"Error saving review: {ex.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.Message = "An error occurred while saving the review. Please try again later.";
                response.ErrorMessages.Add(ex.Message);
                throw;
            }
        }

        public async Task<ApiResponse> DeleteReviewAsync(int id)
        {
            var review = await repository.GetByIdAsync<Review>(id);
            if (review == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.Message = "Review not found.";
                return response;
            }

            repository.Delete(review);
            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.NoContent;
            return response;
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            var review = await repository.GetByIdAsync<Review>(id);

            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }

            return review;
        }

        public async Task<List<Review>> GetReviewsByTourIdAsync(int tourId)
        {
            return await repository.All<Review>()
                .Where(r => r.TourId == tourId)
                .ToListAsync();
        }

        public async Task<ApiResponse> UpdateReviewAsync(int id, ReviewUpdateDTO model)
        {
            var review = await repository.GetByIdAsync<Review>(id);
            if (review == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.Message = "Review not found.";
                return response;
            }

            review.Rating = model.Rating;
            review.Comment = model.Comment;
            review.UpdatedAt = DateTime.Now;

            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.OK;
            response.Result = review;

            return response;
        }
    }
}
