using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using System.Security.Claims;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{tourId:int}")]
        public async Task<ActionResult<ApiResponse>> MakePayment([FromRoute] int tourId)
        {
            var userId = User.Claims.First().Value
                ?? throw new ArgumentNullException("User not found!");


            var response = await _paymentService.MakePaymentAsync(userId, tourId);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}