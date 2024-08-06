using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
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
    }
}
