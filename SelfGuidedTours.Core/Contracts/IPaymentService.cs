using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using Stripe;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IPaymentService
    {
        Task<ApiResponse> MakePaymentAsync(string userId, int tourId);

        Task<ApiResponse> FinalizePaymentAsync(string userId, FinalizePaymentRequest paymentRequest);

        Task<string> CreateOrGetCustomerAsync(string userId);
    }
}