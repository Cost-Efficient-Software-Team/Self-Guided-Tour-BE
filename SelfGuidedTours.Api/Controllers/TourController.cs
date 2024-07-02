using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;
using System.Security.Claims;
using SelfGuidedTours.Core.Contracts;
using System.Linq;

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
        public async Task<IActionResult> CreateTour([FromQuery] TourCreateDTO tourCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(_response);
            }

            var creatorId = User.Claims.First().Value;

            var result = await _tourService.AddAsync(tourCreateDTO, creatorId);

            return CreatedAtAction(nameof(CreateTour), new { id = ((Tour)result.Result).TourId }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTours([FromQuery] string title = null, [FromQuery] string location = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null, [FromQuery] int? minEstimatedDuration = null, [FromQuery] int? maxEstimatedDuration = null)
        {
            var tours = await _tourService.GetFilteredTours(title, location, minPrice, maxPrice, minEstimatedDuration, maxEstimatedDuration);
            _response.Result = tours;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        //[HttpGet("{id:int}", Name = "GetTour")]
        //public async Task<IActionResult> GetTour(int id)
        //{
        //    if (id == 0)
        //    {
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        _response.IsSuccess = false;
        //        return BadRequest(_response);
        //    }

        //    var tour = await _tourService.GetTourById(id);
        //    if (tour == null)
        //    {
        //        _response.StatusCode = HttpStatusCode.NotFound;
        //        _response.IsSuccess = false;
        //        return NotFound(_response);
        //    }

        //    _response.Result = tour;
        //    _response.StatusCode = HttpStatusCode.OK;
        //    return Ok(_response);
        //}

        //[HttpPut("{id}")]
        //public async Task<ActionResult<ApiResponse>> UpdateTour(int id, [FromForm] TourUpdateDTO tourUpdateDTO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _tourService.UpdateTour(id, tourUpdateDTO);
        //        return Ok(result);
        //    }
        //    else
        //    {
        //        _response.IsSuccess = false;
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        _response.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        //        return BadRequest(_response);
        //    }
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> DeleteTour(int id)
        //{
        //    if (id == 0)
        //    {
        //        _response.StatusCode = HttpStatusCode.BadRequest;
        //        _response.IsSuccess = false;
        //        return BadRequest(_response);
        //    }

        //    var result = await _tourService.DeleteTour(id);

        //    if (!result.IsSuccess)
        //    {
        //        if (result.StatusCode == HttpStatusCode.BadRequest)
        //        {
        //            return BadRequest(result);
        //        }
        //        else if (result.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            return NotFound(result);
        //        }
        //    }

        //    return NoContent();
        //}
    }
}
