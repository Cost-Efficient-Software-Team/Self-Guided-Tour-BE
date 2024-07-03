using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SelfGuidedTours.Api.CustomActionFilters;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.ExternalLogin;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;
        private readonly IGoogleAuthService googleAuthService;
        private readonly IEmailService emailService; // Добавяне на emailService
      

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IGoogleAuthService googleAuthService, IEmailService emailService)
        {
            this.authService = authService;
            this.logger = logger;
            this.googleAuthService = googleAuthService;
            this.emailService = emailService; // Инжектиране на emailService
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthenticateResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel model)
        {
            var result = await authService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthenticateResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] LoginInputModel model)
        {
            var response = await authService.LoginAsync(model);

            if (response.AccessToken == null)
            {
                logger.LogWarning("Unauthorized access attempt with email: {Email}", model.Email);
                return Unauthorized(response.ResponseMessage);
            }

            return Ok(response);
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
        [ProducesResponseType(typeof(AuthenticateResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ValidateModel]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestModel model)
        {
            var response = await authService.RefreshAsync(model);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("check-login")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 401)]
        public IActionResult CheckLogin()
        {
            return Ok("User is logged in.");
        }

        [HttpPost("google-signin")]
        [ProducesResponseType(typeof(AuthenticateResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        public async Task<IActionResult> HandleGoogleToken([FromBody] GoogleSignInVM model)
        {
            if (model == null || string.IsNullOrEmpty(model.IdToken))
            {
                return BadRequest("Invalid access token.");
            }

            var userInfo = await googleAuthService.GoogleSignIn(model);
            return Ok(userInfo);
        }

        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model state for change password request!");
                return BadRequest("Invalid model state");
            }

            string userId = User.Claims.First().Value;

            var changePasswordModel = new ChangePasswordModel
            {
                UserId = userId,
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword
            };

            var response = await authService.ChangePasswordAsync(changePasswordModel);
            // In case something goes wrong, an exception is thrown in the authService and it gets automatically caught from the middleware

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await authService.GetByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("User not found.");

            var token = await authService.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Auth", new { token }, Request.Scheme);

            await emailService.SendPasswordResetEmailAsync(model.Email, resetLink);

            return Ok("Password reset link has been sent to your email.");
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authService.ResetPasswordAsync(model.Token, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Password reset failed: {errors}");
            }

            return Ok("Password has been reset.");
        }




        [HttpGet("reset-password")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult ResetPassword([FromQuery] string token)
        {
            return Ok($"Token received: {token}");
        }

       

    }
}
