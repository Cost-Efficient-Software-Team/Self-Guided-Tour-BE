using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Auth;
using System.Security.Claims;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            this.authService = authService;
            this.logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel model)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model state for register input model!");

                return BadRequest("Invalid model state!");
            }

            try
            {
                var result = await authService.RegisterAsync(model);

                return Ok(result);
            }
            catch (ArgumentException aex)
            {
                logger.LogError(aex, "Auth/register[POST] - Argument exception");

                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Auth/register[POST] - Unexpected error");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Login([FromBody] LoginInputModel model)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model state for login input model!");

                return BadRequest("Invalid model state!");
            }

            try
            {
                var response = await authService.LoginAsync(model);

                if(response.AccessToken == null)
                {
                    logger.LogWarning("Unauthorized access attempt with email: {Email}", model.Email);

                    return Unauthorized(response.ResponseMessage);
                }

                return Ok(response);
            }
            catch (ArgumentException aex)
            {
                logger.LogError(aex, "Auth/login[POST] - Argument exception");

                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Auth/login[POST] - Unexpected error");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize]
        [HttpDelete("logout")]
        [ProducesResponseType(typeof(string), 204)]
        public async Task<IActionResult> Logout()
        {
            string userId = User.Claims.First().Value;

            await authService.LogoutAsync(userId);

            return NoContent();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var response = await authService.RefreshAsync(model);

                return Ok(response);
            }
            catch(ArgumentException aex)
            {
                logger.LogError(aex, "Auth/refresh[POST] - Argument exception");

                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Auth/refresh[POST] - Unexpected error");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet("check-login")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 401)]
        public IActionResult CheckLogin()
        {
            return Ok("User is logged in.");
        }
    }
}
