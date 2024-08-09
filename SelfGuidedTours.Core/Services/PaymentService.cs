using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using Stripe;
using System.Net;

namespace SelfGuidedTours.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository _repository;
        private readonly ApiResponse _response;

        public PaymentService(IRepository repository)
        {
            _repository = repository;
            _response = new ApiResponse();
        }


        public async Task<ApiResponse> MakePaymentAsync(string userId, int tourId)
        {
            
            if (string.IsNullOrEmpty(userId) || tourId <= 0)
                            throw new ArgumentNullException("Invalid payment request!");

            // Check if the user exists
            var userExists = await _repository.All<ApplicationUser>().AnyAsync(u => u.Id == userId);
            if (!userExists)
                throw new ArgumentException("User not found!");

            // Check if the tour is already purchased by the user
            //var existingPayment = await _repository.All<Payment>()
            //    .FirstOrDefaultAsync(p => p.UserId == userId && p.TourId == tourId);
            //if (existingPayment != null)
            //    throw new InvalidOperationException("Tour already purchased!");

            // Check if Tour Exists
            var tour = await _repository.All<Tour>()
                .FirstOrDefaultAsync(t => t.TourId == tourId)
                        ?? throw new InvalidOperationException("Tour not found!");

            // Check if the tour price is set
            if (!tour.Price.HasValue)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Tour price is not set.");
                return _response;
            }

            #region Create Payment Intent

            PaymentIntentCreateOptions options = new()
            {
                Amount = (int)(tour.Price.Value * 100), // Amount in cents
                Currency = "usd",
                PaymentMethodTypes =
                [
                    "card",
                ],
            };

            PaymentIntentService service = new();
            PaymentIntent stripeResponse = await service.CreateAsync(options);

            var paymentResult = new PaymentResult
            {
                PaymentIntentId = stripeResponse.Id,
                ClientSecret = stripeResponse.ClientSecret,
                Amount = tour.Price.Value
            };

            // Save payment information in the database
            var payment = new Payment
            {
                UserId = userId,
                TourId = tourId,
                PaymentIntentId = stripeResponse.Id,
                Amount = tour.Price.Value,
                PaymentDate = DateTime.Now
            };

            await _repository.AddAsync(payment);
            await _repository.SaveChangesAsync();

            #endregion

            _response.Result = paymentResult;
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return _response;
        }
    }
}
