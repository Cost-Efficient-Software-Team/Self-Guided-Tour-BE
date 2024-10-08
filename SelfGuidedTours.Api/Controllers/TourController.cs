using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Api.CustomActionFilters;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApiResponse _response;

        public TourController(ITourService tourService, UserManager<ApplicationUser> userManager)
        {
            _tourService = tourService;
            _userManager = userManager;
            _response = new ApiResponse();
        }

        [HttpPost("create-tour")]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ValidateModel]
        public async Task<IActionResult> CreateTour([FromForm] TourCreateDTO tourCreateDTO)
        {
            var creatorId = User.Claims.First().Value;

            var tour = await _tourService.CreateTourAsync(tourCreateDTO, creatorId);

            var tourResponse = _tourService.MapTourToTourResponseDto(tour);

            return CreatedAtAction(nameof(GetTour), new { id = (tourResponse.TourId) }, tourResponse);

        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllTours([FromQuery] string searchTerm = "", [FromQuery] string sortBy = "default", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var (tours, totalPages) = await _tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            var toursResponse = tours
                .Select(t => _tourService.MapTourToTourResponseDto(t))
                .ToList();

            _response.Result = new
            {
                Tours = toursResponse,
                TotalPages = totalPages
            };
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        [HttpPut("update-tour/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ValidateModel]
        public async Task<IActionResult> UpdateTour(int id, [FromForm] TourUpdateDTO tourUpdateDTO)
        {
            var result = await _tourService.UpdateTourAsync(id, tourUpdateDTO);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id:int}", Name = "delete-tour")]
        public async Task<IActionResult> DeleteTour([FromRoute] int id)
        {
            var result = await _tourService.DeleteTourAsync(id);

            if (!result.IsSuccess)
            {
                if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                    return BadRequest(result);
                }
                else if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(result);
                }
            }

            return NoContent();
        }
        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "get-tour")]
        public async Task<IActionResult> GetTour(int id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);

            _response.Result = _tourService.MapTourToTourResponseDto(tour!);
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
    }
}
