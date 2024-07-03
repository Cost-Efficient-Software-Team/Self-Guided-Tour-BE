using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;
using SelfGuidedTours.Api.CustomActionFilters;


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

            var result = await _tourService.AddAsync(tourCreateDTO, creatorId);

            return CreatedAtAction(nameof(CreateTour), new { id = ((Tour)result.Result!).TourId }, result);
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

        [HttpGet("{id:int}", Name = "get-tour")]
        public async Task<IActionResult> GetTour(int id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);//TODO: Change Tour to TourDTO model

            _response.Result = tour!;
            _response.StatusCode = HttpStatusCode.OK;
          
            return Ok(_response);
        }
    }
}
