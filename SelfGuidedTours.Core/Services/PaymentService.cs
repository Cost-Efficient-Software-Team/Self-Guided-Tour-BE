using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using Stripe;
using System.Net;
using static SelfGuidedTours.Common.MessageConstants.LoggerMessages;
using static SelfGuidedTours.Common.Constants.PaymentConstants;
namespace SelfGuidedTours.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository _repository;
        private readonly IStripeClient stripeClient;
        private readonly ILogger<PaymentService> logger;
        private readonly ApiResponse _response;

        public PaymentService(IRepository repository, IStripeClient stripeClient, ILogger<PaymentService> logger)
        {
            _repository = repository;
            this.stripeClient = stripeClient;
            this.logger = logger;
            _response = new ApiResponse();
        }
        /// <summary>
        /// Creates or finds an already existing  stripe customer and asssociated with the current user
        /// </summary>
        /// <param name="userId">Id of the current user</param>
        /// <returns>Id of the Stripe Customer </returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string> CreateOrGetCustomerAsync(string userId)
        {
            var user = _repository.All<ApplicationUser>().FirstOrDefault(u => u.Id == userId)
                ?? throw new ArgumentException(UserNotFoundMessage);

            //Check if we already have a stripe customer accout associated with the user, return it
            if (!string.IsNullOrEmpty(user.StripeCustomerId))
            {
                return user.StripeCustomerId;
            }

            //If the user does not have a stripe customer id, create one
            var customerOptions = new CustomerCreateOptions
            {
                Email = user.Email,
                Name = user.UserName,
                Description = CustomerDescription,
            };

            var customerService = new CustomerService(stripeClient);

            var customer = await customerService.CreateAsync(customerOptions);

            logger.LogInformation(StripeCustomerCreatedSuccessfully,customer.Id);

            user.StripeCustomerId = customer.Id;

            await _repository.UpdateAsync(user);
            await _repository.SaveChangesAsync();

            return customer.Id;
        }

        public async Task<ApiResponse> FinalizePaymentAsync(string userId, FinalizePaymentRequest paymentRequest)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(paymentRequest.PaymentIntentId))
                throw new ArgumentNullException(InvalidPaymentRequestMessage);

            var user = _repository.All<ApplicationUser>().FirstOrDefault(u => u.Id == userId)
                ?? throw new ArgumentException(UserNotFoundMessage);


            // Check if the tour is already purchased by the user
            var existingPayment = await _repository.All<UserTours>()
                 .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TourId == paymentRequest.TourId);

            if (existingPayment != null)
                throw new InvalidOperationException(TourAlreadyPurchasedMessage);

            var tour = await _repository.All<Tour>().FirstOrDefaultAsync(t => t.TourId == paymentRequest.TourId)
                    ?? throw new InvalidOperationException(TourNotFoundMessage);

            if (!tour.Price.HasValue)
                throw new InvalidOperationException(TourPriceNotSetMessage);

            var payment = new Payment
            {
                UserId = userId,
                TourId = paymentRequest.TourId,
                PaymentIntentId = paymentRequest.PaymentIntentId,
                Amount = tour.Price.Value,
                PaymentDate = DateTime.Now
            };

            logger.LogInformation(PaymentCreatetSuccessfully, payment.PaymentId);

            var userTours = new UserTours
            {
                UserId = userId,
                TourId = paymentRequest.TourId,
                PurchaseDate = DateTime.Now,
            };

            logger.LogInformation(UserTourCreatedSuccessfully, userTours.UserTourId);

            await _repository.AddAsync(userTours);
            await _repository.AddAsync(payment);
            await _repository.SaveChangesAsync();

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = new { Message = SuccesfullPaymentMessage };

            return _response;
        }


        public async Task<ApiResponse> MakePaymentAsync(string userId, int tourId)
        {

            if (string.IsNullOrEmpty(userId) || tourId <= 0)
                throw new ArgumentNullException(InvalidPaymentRequestMessage);

            // Check if the user exists
            var userExists = await _repository.All<ApplicationUser>().AnyAsync(u => u.Id == userId);

            if (!userExists)
                throw new InvalidOperationException(UserNotFoundMessage);


            // Check if Tour Exists
            var tour = await _repository.All<Tour>()
                .FirstOrDefaultAsync(t => t.TourId == tourId)
                        ?? throw new InvalidOperationException(TourNotFoundMessage);

            // Check if the tour price is set
            if (!tour.Price.HasValue)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(TourPriceNotSetMessage);
                return _response;
            }

            string customerId = await CreateOrGetCustomerAsync(userId);

            #region Create Payment Intent

            PaymentIntentCreateOptions options = new()
            {
                Amount = (int)(tour.Price.Value * 100), // Amount in cents
                Currency = "usd",
                PaymentMethodTypes =
                [
                    "card",
                ],
                Customer = customerId,
            };

            PaymentIntentService service = new(stripeClient);


            PaymentIntent stripeResponse = await service.CreateAsync(options);

            logger.LogInformation(PaymentIntentCreatedSuccessfully,stripeResponse.Id);

            #endregion

            var paymentResult = new PaymentResult
            {
                PaymentIntentId = stripeResponse.Id,
                ClientSecret = stripeResponse.ClientSecret,
                Amount = tour.Price.Value

            };

            _response.Result = paymentResult;
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return _response;
        }
    }
}
