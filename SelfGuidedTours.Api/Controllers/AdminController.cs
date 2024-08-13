using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.ErrorResponse;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Data.Enums;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService adminService;
        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        [HttpGet("all-tours")]
        public async Task<IActionResult> AllTours(Status status)
        {
            var tours = await adminService.GetAllToursAsync(status);

            return Ok(tours);
        }

        [HttpPatch("approve-tour/{id:int}", Name = "approve-tour")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ApproveTour([FromRoute] int id)
        {
            var result = await adminService.ApproveTourAsync(id);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPatch("reject-tour/{id:int}", Name = "reject-tour")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RejectTour([FromRoute] int id)
        {
            var result = await adminService.RejectTourAsync(id);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
