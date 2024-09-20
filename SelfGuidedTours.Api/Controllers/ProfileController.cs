using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Api.CustomActionFilters;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models.RequestDto;
using SelfGuidedTours.Core.Models.ResponseDto;
using SelfGuidedTours.Infrastructure.Data.Models;
using System;
using System.Net;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetMyTours()
        {
            var tours = await _profileService.GetMyToursAsync(this.UserId);

            return tours == null ? NotFound() : Ok(tours);
        }

        [HttpGet("bought-tours")]
        [ProducesResponseType(typeof(TourResponseDto), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetBoughtTours()
        {
            var tours = await _profileService.GetBoughtToursAsync(this.UserId);

            return tours == null ? NotFound() : Ok(tours);
        }
    }
}
