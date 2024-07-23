using SelfGuidedTours.Core.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IPaymentService
    {
        Task<ApiResponse> MakePaymentAsync(string userId, PaymentRequest paymentRequest);
    }
}