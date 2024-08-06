using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using System.Security.Claims;

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

        [HttpPost("{tourId}")]
        public async Task<ActionResult<ApiResponse>> MakePayment([FromRoute] PaymentRequest paymentRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? throw new UnauthorizedAccessException();

            var response = await _paymentService.MakePaymentAsync(userId, paymentRequest);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}