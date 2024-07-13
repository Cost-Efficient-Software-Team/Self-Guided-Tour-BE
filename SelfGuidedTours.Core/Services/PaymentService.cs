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
        private readonly IRepository repository;
        private readonly ApiResponse response;
        private readonly IConfiguration configuration;

        public PaymentService(IRepository repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
            response = new ApiResponse();

            // Зареждане на Stripe API ключа от конфигурацията
            StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];
        }

        public async Task<ApiResponse> MakePaymentAsync(string userId, PaymentRequest paymentRequest)
        {
            if (paymentRequest == null || string.IsNullOrEmpty(userId) || paymentRequest.TourId <= 0)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Invalid payment request.");
                return response;
            }

            // Проверка дали турът вече е закупен от потребителя
            var existingPayment = await repository.All<Payment>()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.TourId == paymentRequest.TourId);
            if (existingPayment != null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Tour already purchased.");
                return response;
            }

            // Получаване на информация за тура
            var tour = await repository.All<Tour>()
                .FirstOrDefaultAsync(t => t.TourId == paymentRequest.TourId);
            if (tour == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Tour not found.");
                return response;
            }

            // Проверка дали цената на тура е налична
            if (!tour.Price.HasValue)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.IsSuccess = false;
                response.ErrorMessages.Add("Tour price is not set.");
                return response;
            }

            #region Create Payment Intent

            PaymentIntentCreateOptions options = new()
            {
                Amount = (int)(tour.Price.Value * 100), // Amount in cents
                Currency = "usd",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
            };

            PaymentIntentService service = new();
            PaymentIntent stripeResponse = await service.CreateAsync(options);

            var paymentResult = new PaymentResult
            {
                PaymentIntentId = stripeResponse.Id,
                ClientSecret = stripeResponse.ClientSecret,
                Amount = tour.Price.Value
            };

            // Записване на информацията за плащането в базата данни
            var payment = new Payment
            {
                UserId = userId,
                TourId = paymentRequest.TourId,
                PaymentIntentId = stripeResponse.Id,
                Amount = tour.Price.Value,
                PaymentDate = DateTime.Now
            };

            await repository.AddAsync(payment);
            await repository.SaveChangesAsync();

            #endregion

            response.Result = paymentResult;
            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            return response;
        }
    }
}
