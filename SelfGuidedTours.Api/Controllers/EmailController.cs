using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;

namespace SelfGuidedTours.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService emailService;

        public EmailController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        [HttpPost("send")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailDto sendEmailRequest)
        {
            await emailService.SendEmail(sendEmailRequest, "html");

            return Ok("Email sent successfully!");
        }

        [HttpPost("send-html")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> SendHtmlEmail([FromBody] SendEmailDto sendEmailRequest)
        {
            await emailService.SendEmail(sendEmailRequest, "html");

            return Ok("Email sent successfully!");
        }
    }


}
