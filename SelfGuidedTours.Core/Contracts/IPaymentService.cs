using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using Stripe;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IPaymentService
    {
        Task<ApiResponse> MakePaymentAsync(string userId, int tourId);

        Task<ApiResponse> FinalizePaymentAsync(string paymentIndentId);

        Task<string> CreateOrGetCustomerAsync(string userId);

        Task<ApiResponse> AddFreeTour(string userId, int tourId);
    }
}