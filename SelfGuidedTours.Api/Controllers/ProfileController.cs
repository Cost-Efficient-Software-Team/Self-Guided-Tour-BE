﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Models;
using System.Net;
using SelfGuidedTours.Api.CustomActionFilters;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
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

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            var profile = await _profileService.GetProfileAsync(userId);
            if (profile == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Profile not found");
                return NotFound(_response);
            }
            _response.Result = profile;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(UserProfile), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ValidateModel]
        public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UserProfile profile)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Invalid data");
                return BadRequest(_response);
            }
            var updatedProfile = await _profileService.UpdateProfileAsync(userId, profile);
            if (updatedProfile == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Profile not found");
                return NotFound(_response);
            }
            _response.Result = updatedProfile;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}