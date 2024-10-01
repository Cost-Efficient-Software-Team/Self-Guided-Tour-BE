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

        public async Task<Review> CreateReviewAsync(ReviewCreateDTO model, string userId, int tourId)
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

                var averageRating = await repository.All<Review>()
                    .Where(r => r.TourId == tourId)
                    .AverageAsync(r => (decimal)r.Rating);

                tour.AverageRating = Math.Round(averageRating, 1);
                await repository.SaveChangesAsync();

                response.StatusCode = HttpStatusCode.Created;
                return reviewToAdd;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database update error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages.Add("A database error occurred while saving the review. Please try again later.");
                response.ErrorMessages.Add(dbEx.InnerException?.Message ?? dbEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving review: {ex.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages.Add("An error occurred while saving the review. Please try again later.");
                response.ErrorMessages.Add(ex.Message);
                throw;
            }
        }

        public async Task<ApiResponse> UpdateReviewAsync(int id, ReviewUpdateDTO model)
        {
            var review = await repository.GetByIdAsync<Review>(id);
            if (review == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Review not found.");
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

        public async Task<ReviewDTO?> GetReviewByIdAsync(int id)
        {
            var review = await repository.All<Review>()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }

            var reviewDTO = new ReviewDTO
            {
                ReviewId = review.ReviewId,
                UserId = review.UserId,
                UserName = review.User.Name,
                UserImg = review.User.ProfilePictureUrl,
                TourId = review.TourId,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate,
                UpdatedAt = review.UpdatedAt
            };

            return reviewDTO;
        }

        public async Task<List<ReviewDTO>> GetReviewsByTourIdAsync(int tourId)
        {
            var reviews = await repository.All<Review>()
                .Where(r => r.TourId == tourId)
                .Include(r => r.User)
                .ToListAsync();

            var reviewDTOs = reviews.Select(review => new ReviewDTO
            {
                ReviewId = review.ReviewId,
                UserId = review.UserId,
                UserName = review.User.Name,
                UserImg = review.User.ProfilePictureUrl,
                TourId = review.TourId,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate,
                UpdatedAt = review.UpdatedAt
            }).ToList();

            return reviewDTOs;
        }


        public async Task<ApiResponse> DeleteReviewAsync(int id)
        {
            var review = await repository.GetByIdAsync<Review>(id);
            if (review == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Review not found.");
                return response;
            }

            repository.Delete(review);
            await repository.SaveChangesAsync();

            response.StatusCode = HttpStatusCode.NoContent;
            return response;
        }
    }
}
