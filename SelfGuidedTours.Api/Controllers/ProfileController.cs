using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Api.CustomActionFilters;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.ErrorResponse;
using SelfGuidedTours.Core.Models.RequestDto;
using SelfGuidedTours.Core.Models.ResponseDto;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApiResponse _response;

        public ProfileController(IProfileService profileService, UserManager<ApplicationUser> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
            _response = new ApiResponse();
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserProfileDto), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await _profileService.GetProfileAsync(this.UserId);

            return profile == null ? NotFound() : Ok(profile);

        }

        [HttpPatch]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ValidateModel]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequestDto profile)
        {
           var updatedProfile = await _profileService.UpdateProfileAsync(this.UserId, profile);

            return updatedProfile == null ? NotFound() : Ok(updatedProfile);
        }

        [HttpGet]
        [Route("my-tours")]
        [ProducesResponseType(typeof(TourResponseDto), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetMyTours([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var tours = await _profileService.GetMyToursAsync(this.UserId, page, pageSize);

            return tours == null ? NotFound() : Ok(tours);
        }

        [HttpGet("bought-tours")]
        [ProducesResponseType(typeof(TourResponseDto), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetBoughtTours([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var tours = await _profileService.GetBoughtToursAsync(this.UserId, page, pageSize);

            return tours == null ? NotFound() : Ok(tours);
        }
        
        [HttpGet("transactions")]
        [ProducesResponseType(typeof(UserTransactionsResponseDto), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUserTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var transactions = await _profileService.GetUserTransactionsAsync(this.UserId,page,pageSize);

            return transactions == null ? NotFound() : Ok(transactions);
        }

        [HttpPatch("delete-my-tour/{id:int}", Name = "delete-my-tour")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteTour([FromRoute] int id)
        {
            var result = await _profileService.DeleteTourAsync(id, this.UserId);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
