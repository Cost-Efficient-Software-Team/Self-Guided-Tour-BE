using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;

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
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel model)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model state for register input model!");

                return BadRequest(ModelState);
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
    }
}
