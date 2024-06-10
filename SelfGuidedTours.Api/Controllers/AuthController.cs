using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Services;

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

                if (response.AccessToken == null)
                {
                    logger.LogWarning($"Unauthorized access attempt with email: {model.Email}");

                    return Unauthorized(response.ResponseMessage);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Auth/login[POST] - Unexpected error");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet("test-login")]
        public IActionResult TestLogin()
        {
            return Ok("User is logged in!");
        }
    }
}
