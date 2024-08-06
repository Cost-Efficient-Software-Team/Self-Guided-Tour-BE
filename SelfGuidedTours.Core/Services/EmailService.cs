using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;

namespace SelfGuidedTours.Core.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmail(SendEmailDto sendEmailRequest, string emailBodyFormat = "plain")
        {
            if (!Enum.TryParse(emailBodyFormat, true, out TextFormat _))
            {
                throw new ArgumentException("Invalid email body format, possible formats \"html\", \"plain\", \"rtf\", \"enriched\"", nameof(emailBodyFormat));
            }

            var emailSender = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME") ?? throw new ApplicationException("Email sender is not configured.");
            var emailHost = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_HOST") ?? throw new ApplicationException("Email host is not configured.");
            var emailPort = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PORT") ?? throw new ApplicationException("Email port is not configured.");
            var emailPassword = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD") ?? throw new ApplicationException("Email password is not configured.");

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailSender));
            email.To.Add(MailboxAddress.Parse(sendEmailRequest.To));
            email.Subject = sendEmailRequest.Subject;
            email.Body = new TextPart(emailBodyFormat)
            {
                Text = sendEmailRequest.Body
            };

            using var smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Игнориране на сертификатните грешки (само за тестване)
            smtp.Connect(emailHost, int.Parse(emailPort), SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(emailSender, emailPassword);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME")));
            mailMessage.To.Add(MailboxAddress.Parse(email));
            mailMessage.Subject = "Reset Your Password";
            mailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = $"Please reset your password by clicking on the link: <a href='{resetLink}'>Reset Password</a>"
            };

            using var smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Игнориране на сертификатните грешки (само за тестване)
            smtp.Connect(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_HOST"), int.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PORT")!), SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME"), Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD"));
            await smtp.SendAsync(mailMessage);
            smtp.Disconnect(true);
        }

        public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME")));
            mailMessage.To.Add(MailboxAddress.Parse(email));
            mailMessage.Subject = "Confirm Your Email";
            mailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = $"Please confirm your email by clicking on the link: <a href='{confirmationLink}'>Confirm Email</a>"
            };

            using var smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Игнориране на сертификатните грешки (само за тестване)
            smtp.Connect(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_HOST"), int.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PORT")!), SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME"), Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD"));
            await smtp.SendAsync(mailMessage);
            smtp.Disconnect(true);
        }
    }
}
