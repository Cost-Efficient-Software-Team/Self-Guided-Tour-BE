using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Enums;
using System.Security.Claims;
using SelfGuidedTours.Core.Models;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TourController : ControllerBase
    {
        private readonly SelfGuidedToursDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApiResponse _response;

        public TourController(SelfGuidedToursDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _response = new ApiResponse();
        }

        // Create
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateTour([FromForm] TourCreateDTO tourCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var creatorId = User.FindFirstValue("id");
                    if (creatorId == null)
                    {
                        _response.StatusCode = HttpStatusCode.Unauthorized;
                        _response.IsSuccess = false;
                        _response.ErrorMessages.Add("Unauthorized user.");
                        return Unauthorized(_response);
                    }

                    Tour tourToCreate = new Tour
                    {
                        Title = tourCreateDTO.Title,
                        Description = tourCreateDTO.Description,
                        Price = tourCreateDTO.Price,
                        Location = tourCreateDTO.Location,
                        CreatedAt = DateTime.Now,
                        ThumbnailImageUrl = tourCreateDTO.ThumbnailImageUrl,
                        EstimatedDuration = tourCreateDTO.EstimatedDuration,
                        CreatorId = creatorId,
                        Status = Status.Pending,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Tours.Add(tourToCreate);
                    await _context.SaveChangesAsync();
                    _response.Result = tourToCreate;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtAction(nameof(GetTour), new { id = tourToCreate.TourId }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        // Read by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetTour(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Landmarks)
                .Include(t => t.Payments)
                .Include(t => t.Reviews)
                .Include(t => t.UserTours)
                .FirstOrDefaultAsync(t => t.TourId == id);

            if (tour == null)
            {
                return NotFound();
            }

            return tour;
        }

        // Read all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tour>>> GetTours()
        {
            return await _context.Tours
                .Include(t => t.Landmarks)
                .Include(t => t.Payments)
                .Include(t => t.Reviews)
                .Include(t => t.UserTours)
                .ToListAsync();
        }

        // Update
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateTour(int id, [FromForm] TourUpdateDTO tourUpdateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (tourUpdateDTO == null || id != tourUpdateDTO.TourId)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest(_response);
                    }

                    var tour = await _context.Tours.FindAsync(id);
                    if (tour == null)
                    {
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.IsSuccess = false;
                        return NotFound(_response);
                    }

                    tour.Title = tourUpdateDTO.Title;
                    tour.Description = tourUpdateDTO.Description;
                    tour.Price = tourUpdateDTO.Price;
                    tour.Location = tourUpdateDTO.Location;
                    tour.ThumbnailImageUrl = tourUpdateDTO.ThumbnailImageUrl;
                    tour.EstimatedDuration = tourUpdateDTO.EstimatedDuration;
                    tour.UpdatedAt = DateTime.Now;
                    tour.Status = Status.Pending; 

                    _context.Entry(tour).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                    _response.StatusCode = HttpStatusCode.NoContent;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        // Delete
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse>> DeleteTour(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                var tour = await _context.Tours.FindAsync(id);
                if (tour == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode((int)HttpStatusCode.InternalServerError, _response);
            }
        }

        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.TourId == id);
        }
    }
}
