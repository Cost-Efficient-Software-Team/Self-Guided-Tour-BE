using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Api.CustomActionFilters;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Dto.Review;
using SelfGuidedTours.Core.Models.ErrorResponse;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApiResponse _response;

        public ReviewController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
            _response = new ApiResponse();
        }

        [HttpPost("create-review/{tourId:int}")]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ValidateModel]
        public async Task<IActionResult> CreateReview([FromRoute] int tourId, [FromBody] ReviewCreateDTO reviewCreateDTO)
        {
            var userId = User.Claims.First().Value;

            try
            {
                var review = await _reviewService.CreateAsync(reviewCreateDTO, userId, tourId);
                _response.Result = review;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, _response);
            }
            catch (Exception ex)
            {
                // Log the exception (this should be done using a proper logging framework)
                Console.WriteLine($"Error creating review: {ex.Message}");
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return StatusCode((int)_response.StatusCode, _response);
            }
        }

        [HttpGet("{id:int}", Name = "get-review")]
        public async Task<IActionResult> GetReview(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);

            _response.Result = review;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews([FromQuery] int tourId)
        {
            var reviews = await _reviewService.GetReviewsByTourIdAsync(tourId);

            _response.Result = reviews;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }

        [HttpDelete("{id:int}", Name = "delete-review")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);

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

        [HttpPatch("update-review/{id:int}", Name = "update-review")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdateReview([FromRoute] int id, [FromBody] ReviewUpdateDTO reviewUpdateDTO)
        {
            var result = await _reviewService.UpdateReviewAsync(id, reviewUpdateDTO);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
