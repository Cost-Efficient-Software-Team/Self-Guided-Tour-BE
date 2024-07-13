using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<ApiResponse>> MakePayment(string userId, [FromBody] PaymentRequest paymentRequest)
        {
            var response = await paymentService.MakePaymentAsync(userId, paymentRequest);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}