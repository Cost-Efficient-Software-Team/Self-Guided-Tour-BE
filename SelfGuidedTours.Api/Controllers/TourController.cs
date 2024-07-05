using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;
using SelfGuidedTours.Api.CustomActionFilters;
using SelfGuidedTours.Core.Models.ResponseDto;


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

            var tour = await _tourService.CreateAsync(tourCreateDTO, creatorId);
            // Think about AutoMapper
            var tourResponse = new TourResponseDto
            {
                TourId = tour.TourId,
                ThumbnailImageUrl = tour.ThumbnailImageUrl,
                Location = tour.Location,
                Description = tour.Description,
                EstimatedDuration = tour.EstimatedDuration,
                Price = tour.Price,
                Status = tour.Status.ToString(),
                Title = tour.Title,
                Landmarks = tour.Landmarks.Select(l => new LandmarkResponseDto
                {
                    LandmarkId = l.LandmarkId,
                    LandmarkName = l.Name,
                    Description = l.Description,
                    StopOrder = l.StopOrder,
                    Latitude =l.Coordinate.Latitude,
                    Longitude = l.Coordinate.Longitude,
                    Resources = l.Resources.Select(r => new ResourceResponseDto
                    {
                        ResourceId = r.LandmarkResourceId,
                        ResourceUrl = r.Url,
                        ResourceType = r.Type.ToString()
                    }).ToList()
                }).ToList()
            };


            return CreatedAtAction(nameof(GetTour), new { id = (tourResponse.TourId) },tourResponse); 
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
