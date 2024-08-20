using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;
using Stripe;
using System.Net;
using static SelfGuidedTours.Common.Constants.PaymentConstants;
using static SelfGuidedTours.Common.MessageConstants.LoggerMessages;
namespace SelfGuidedTours.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository _repository;
        private readonly IStripeClient stripeClient;
        private readonly ILogger<PaymentService> logger;
        private readonly CustomerService customerService;
        private readonly PaymentIntentService paymentIntentService;
        private readonly ApiResponse _response;

        public PaymentService(IRepository repository, IStripeClient stripeClient, ILogger<PaymentService> logger,
            CustomerService customerService, PaymentIntentService paymentIntentService)
        {
            _repository = repository;
            this.stripeClient = stripeClient;
            this.logger = logger;
            this.customerService = customerService;
            this.paymentIntentService = paymentIntentService;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> AddFreeTour(string userId, int tourId)
        {
            var tour = _repository.All<Tour>().FirstOrDefault(t => t.TourId == tourId)
                       ?? throw new InvalidOperationException(TourNotFoundMessage);

            // Check if the tour is actualy free
            if (tour.Price != 0)
                throw new InvalidOperationException(TourNotFreeMessage);

            var existingUserTour = _repository.All<UserTours>()
                .FirstOrDefault(ut => ut.UserId == userId && ut.TourId == tourId);

            if (existingUserTour != null)
                throw new InvalidOperationException(TourAlreadyPurchasedMessage);

            var userTours = new UserTours
            {
                UserId = userId,
                TourId = tourId,
                PurchaseDate = DateTime.Now,
            };

            logger.LogInformation(UserTourCreatedSuccessfully, userTours.UserTourId);

            await _repository.AddAsync(userTours);
            await _repository.SaveChangesAsync();

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = FreeTourAddedMessage;

            return _response;
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

            // var customerService = new CustomerService(stripeClient);

            var customer = await customerService.CreateAsync(customerOptions);

            logger.LogInformation(StripeCustomerCreatedSuccessfully, customer.Id);

            user.StripeCustomerId = customer.Id;

            await _repository.UpdateAsync(user);
            await _repository.SaveChangesAsync();

            return customer.Id;
        }

        public async Task<ApiResponse> FinalizePaymentAsync(string paymentIndentId)
        {
            var payment = await _repository.All<Payment>().FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIndentId)
                ?? throw new InvalidOperationException(PaymentNotFoundMessage);

            // If the payment is marked as succeeded, this means that the user already has the tour
            if (payment.Status == PaymentStatus.Succeeded)
                throw new InvalidOperationException(TourAlreadyPurchasedMessage);

            // Add the tour to the user
            var userTours = new UserTours
            {
                UserId = payment.UserId,
                TourId = payment.TourId,
                PurchaseDate = payment.PaymentDate,
            };

            logger.LogInformation(UserTourCreatedSuccessfully, userTours.UserTourId);

            // Mark the payment as succeeded
            payment.Status = PaymentStatus.Succeeded;
            payment.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(payment);

            logger.LogInformation(PyamentMarkedAsSucceeded, payment.PaymentId);

            await _repository.AddAsync(userTours);
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
            if (!tour.Price.HasValue || tour.Price == 0)
                throw new InvalidOperationException(TourPriceNotSetMessage);

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



            PaymentIntent stripeResponse = await paymentIntentService.CreateAsync(options);

            logger.LogInformation(PaymentIntentCreatedSuccessfully, stripeResponse.Id);

            #endregion

            var paymentResult = new PaymentResult
            {
                PaymentIntentId = stripeResponse.Id,
                ClientSecret = stripeResponse.ClientSecret,
                Amount = tour.Price.Value

            };

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

            _response.Result = paymentResult;
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return _response;
        }
    }
}
