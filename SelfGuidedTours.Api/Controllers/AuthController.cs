using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SelfGuidedTours.Api.CustomActionFilters;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.Auth.ResetPassword;
using SelfGuidedTours.Core.Models.ExternalLogin;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ILogger<AuthController> logger;
        private readonly IGoogleAuthService googleAuthService;
        private readonly IEmailService emailService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IGoogleAuthService googleAuthService, IEmailService emailService)
        {
            this.authService = authService;
            this.logger = logger;
            this.googleAuthService = googleAuthService;
            this.emailService = emailService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthenticateResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel model)
        {
            var result = await authService.RegisterAsync(model);

            if (result != null)
            {
                var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
                if (string.IsNullOrEmpty(baseUrl))
                {
                    return StatusCode(500, "Base URL is not configured.");
                }

                var confirmationLink = $"{baseUrl}/api/Auth/confirm-email?userId={result.UserId}&token={Uri.EscapeDataString(result.EmailConfirmationToken)}";

                await emailService.SendEmailConfirmationAsync(result.Email, confirmationLink);

                return Ok(new { message = "Registration successful! Please check your email to confirm your registration.", userId = result.UserId, token = result.EmailConfirmationToken });
            }

            return BadRequest("Registration failed.");
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
                logger.LogWarning("Unauthorized login attempt with email: {Email}", model.Email);
                return Unauthorized(response.ResponseMessage);
            }

            logger.LogInformation("User {Email} has successfully logged in.", model.Email);
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
        [ValidateModel]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto model)
        {
            string userId = User.Claims.First().Value;

            var changePasswordModel = new ChangePasswordModel
            {
                UserId = userId,
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword
            };

            var response = await authService.ChangePasswordAsync(changePasswordModel);

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
            {
                logger.LogWarning("Password reset attempt for non-existent user: {Email}", model.Email);
                return BadRequest("User not found.");
            }

            var token = await authService.GeneratePasswordResetTokenAsync(user);

            // Combine userId and token into one token using Base64 URL encoding
            var combinedToken = Base64UrlEncoder.Encode($"{user.Id}:{token}");
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");

            if (string.IsNullOrEmpty(baseUrl))
            {
                logger.LogError("BASE_URL is not configured.");
                return StatusCode(500, "Base URL is not configured.");
            }

            var resetLink = $"{baseUrl}/reset-password?token={combinedToken}";
            logger.LogInformation("Password reset link for user {Email}: {ResetLink}", model.Email, resetLink);

            var emailDto = new SendEmailDto
            {
                To = model.Email,
                Subject = "Reset Your Password",
                Body = $"You can reset your password by clicking the link below:\n\n<a href='{resetLink}'>Reset Password</a>"
            };

            await emailService.SendEmail(emailDto, "html");

            return Ok("Password reset link has been sent to your email.");
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(model.Token))
            {
                return BadRequest("Token is required.");
            }

            var result = await authService.ResetPasswordAsync(model.Token, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogWarning("Password reset failed: {Errors}", errors);
                return BadRequest($"Password reset failed: {errors}");
            }

            logger.LogInformation("Password has been reset successfully.");
            return Ok("Password has been reset.");
        }
        [HttpGet("reset-password")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ResetPassword([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required.");
            }

            // Decode the combined token using Base64 URL decoding
            var decodedToken = Base64UrlEncoder.Decode(token);
            var parts = decodedToken.Split(':');

            if (parts.Length != 2)
            {
                return BadRequest("Invalid token.");
            }

            var userId = parts[0];
            var resetToken = parts[1];

            var user = await authService.GetByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            var isTokenValid = await authService.VerifyPasswordResetTokenAsync(user, resetToken);
            if (!isTokenValid)
            {
                return BadRequest("Invalid or expired token.");
            }

            return Ok("Token is valid.");
        }

        [HttpGet("confirm-email")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                logger.LogError("UserId or Token is missing.");
                return BadRequest("UserId and Token are required.");
            }

            logger.LogInformation($"ConfirmEmail called with userId: {userId}, token: {token}");//remove this later to avoid injection attacks

            var result = await authService.ConfirmEmailAsync(userId, token);
            if (result.Succeeded)
            {
                logger.LogInformation("Email confirmed successfully.");
                return Ok("Email confirmed successfully!");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogError($"Email confirmation failed. Errors: {errors}");
            return BadRequest(new { message = "Email confirmation failed.", errors });
        }
    }
}
