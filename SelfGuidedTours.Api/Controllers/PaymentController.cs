using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using Stripe;
using System.Security.Claims;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            this.logger = logger;
        }

        [HttpPost("{tourId:int}")]
        public async Task<ActionResult<ApiResponse>> MakePayment([FromRoute] int tourId)
        {
            var userId = User.Claims.First().Value
                ?? throw new ArgumentNullException("User not found!");


            var response = await _paymentService.MakePaymentAsync(userId, tourId);

            return StatusCode((int)response.StatusCode, response);
        }

        // This gets called by the Stripe API when a payment is successful
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> FinilizePaymentWebHook()
        {
            var endpointSecret = Environment.GetEnvironmentVariable("STRIPE_ENDPOINT_SECRET")
                ?? throw new ArgumentNullException("Stripe endpoint secret variable not setuped!");

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                    if (paymentIntent == null)
                    {
                        return BadRequest();
                    }
                    // Fulfill the purchase...
                    await _paymentService.FinalizePaymentAsync(paymentIntent.Id);
                }
                else
                {
                    logger.LogWarning("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
        }
            
        [HttpPost("free/{tourId:int}")]
        public async Task<ActionResult<ApiResponse>> AddFreeTour([FromRoute] int tourId)
        {
            var userId = User.Claims.First().Value
                ?? throw new ArgumentNullException("User not found!");

            var response = await _paymentService.AddFreeTour(userId, tourId);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}