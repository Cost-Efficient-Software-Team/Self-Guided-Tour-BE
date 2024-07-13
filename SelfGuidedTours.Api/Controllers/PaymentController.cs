using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<ApiResponse>> MakePayment(string userId, [FromBody] PaymentRequest paymentRequest)
        {
            var response = await _paymentService.MakePaymentAsync(userId, paymentRequest);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}