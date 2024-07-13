using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;
using Stripe;
using System.Net;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly IConfiguration _configuration;
        private readonly SelfGuidedToursDbContext _db;

        public PaymentController(IConfiguration configuration, SelfGuidedToursDbContext db)
        {
            _configuration = configuration;
            _db = db;
            _response = new();
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<ApiResponse>> MakePayment(string userId, [FromBody] PaymentRequest paymentRequest)
        {
            if (paymentRequest == null || string.IsNullOrEmpty(userId) || paymentRequest.TourId <= 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessage = "Invalid payment request.";
                return BadRequest(_response);
            }

            // Check if the tour has already been purchased by the user
            var existingPayment = await _db.Payments.FirstOrDefaultAsync(p => p.UserId == userId && p.TourId == paymentRequest.TourId);
            if (existingPayment != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessage = "Tour already purchased.";
                return BadRequest(_response);
            }

            // Get information about the tour
            var tour = await _db.Tours.FirstOrDefaultAsync(t => t.TourId == paymentRequest.TourId);
            if (tour == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessage = "Tour not found.";
                return NotFound(_response);
            }

            // Check if the tour price is available
            if (!tour.Price.HasValue)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessage = "Tour price is not set.";
                return BadRequest(_response);
            }

            #region Create Payment Intent

            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

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
            PaymentIntent response = await service.CreateAsync(options);

            var paymentResult = new PaymentResult
            {
                PaymentIntentId = response.Id,
                ClientSecret = response.ClientSecret,
                Amount = tour.Price.Value
            };

            // Save the payment information to the database
            var payment = new Payment
            {
                UserId = userId,
                TourId = paymentRequest.TourId,
                PaymentIntentId = response.Id,
                Amount = tour.Price.Value,
                PaymentDate = DateTime.Now
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            #endregion

            _response.Result = paymentResult;
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
    }

    public class PaymentRequest
    {
        public int TourId { get; set; }
    }

    public class PaymentResult
    {
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Amount { get; set; }
    }

    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public object Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}
