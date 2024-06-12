using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;

namespace SelfGuidedTours.Core.Services
{
    public class EmailService : IEmailService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendEmailRequest"></param>
        /// <param name="emailBodyFormat">By default you get "plain" for simple text, you can add "html" and write html directly in the body</param>
        public async Task SendEmail(SendEmailDto sendEmailRequest, string emailBodyFormat = "plain")  // Possible formats "html, "plain". "rtf", "enriched" 
        {
            if(!Enum.TryParse(emailBodyFormat, true, out TextFormat _))
            {
                throw new ArgumentException("Invalid email body format, possible formats \"html, \"plain\". \"rtf\", \"enriched\"", nameof(emailBodyFormat));
            }

            //Get from env variables located in launchSettings.json
            var emailSender = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME") ?? throw new ApplicationException("Email sender is not configured.");
            var emailHost = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_HOST") ?? throw new ApplicationException("Email host is not configured.");
            var emailPort = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PORT") ?? throw new ApplicationException("Email port is not configured.");
            var emailPassword = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD") ?? throw new ApplicationException("Email password is not configured.");

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailSender));
            email.To.Add(MailboxAddress.Parse(sendEmailRequest.To));
            email.Subject = sendEmailRequest.Subject;
            email.Body = new TextPart(emailBodyFormat) //TODO: add support for HTML
            {
                Text = sendEmailRequest.Body
            };
            //SMTP Client MUST come from  MailKit.Net.Smtp not system
            using var smtp = new SmtpClient();
            smtp.Connect(emailHost, int.Parse(emailPort), MailKit.Security.SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(emailSender, emailPassword);
             await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
