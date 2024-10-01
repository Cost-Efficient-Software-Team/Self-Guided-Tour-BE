using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto.Review;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IReviewService
    {
        Task<Review> CreateReviewAsync(ReviewCreateDTO model, string userId, int tourId);
        Task<ReviewDTO?> GetReviewByIdAsync(int id);
        Task<List<ReviewDTO>> GetReviewsByTourIdAsync(int tourId);
        Task<ApiResponse> UpdateReviewAsync(int id, ReviewUpdateDTO model);
        Task<ApiResponse> DeleteReviewAsync(int id);
    }
}