using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmail(SendEmailDto sendEmailRequest, string emailBodyFormat = "plain")
    {
        var templatePath = _configuration["EmailTemplates:GenericEmailTemplate"];
        var emailBody = await File.ReadAllTextAsync(templatePath);
        emailBody = emailBody.Replace("{{Subject}}", sendEmailRequest.Subject);
        emailBody = emailBody.Replace("{{Body}}", sendEmailRequest.Body);

        var emailSender = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME") ?? throw new ApplicationException("Email sender is not configured.");
        var emailHost = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_HOST") ?? throw new ApplicationException("Email host is not configured.");
        var emailPort = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PORT") ?? throw new ApplicationException("Email port is not configured.");
        var emailPassword = Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD") ?? throw new ApplicationException("Email password is not configured.");

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(emailSender));
        email.To.Add(MailboxAddress.Parse(sendEmailRequest.To));
        email.Subject = sendEmailRequest.Subject;
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = emailBody
        };

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; //Ignore certificate errors (for testing only)
        smtp.Connect(emailHost, int.Parse(emailPort), SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(emailSender, emailPassword);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        var templatePath = _configuration["EmailTemplates:PasswordResetEmailTemplate"];
        var emailBody = await File.ReadAllTextAsync(templatePath);
        emailBody = emailBody.Replace("{{UserName}}", email);
        emailBody = emailBody.Replace("{{ResetLink}}", resetLink);

        var mailMessage = new MimeMessage();
        mailMessage.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME")));
        mailMessage.To.Add(MailboxAddress.Parse(email));
        mailMessage.Subject = "Reset Your Password";
        mailMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = emailBody
        };

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; //Ignore certificate errors (for testing only)
        smtp.Connect(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_HOST"), int.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PORT")!), SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME"), Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD"));
        await smtp.SendAsync(mailMessage);
        smtp.Disconnect(true);
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        var templatePath = _configuration["EmailTemplates:ConfirmationEmailTemplate"];
        var emailBody = await File.ReadAllTextAsync(templatePath);
        emailBody = emailBody.Replace("{{UserName}}", email);
        emailBody = emailBody.Replace("{{ConfirmationLink}}", confirmationLink);

        var mailMessage = new MimeMessage();
        mailMessage.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME")));
        mailMessage.To.Add(MailboxAddress.Parse(email));
        mailMessage.Subject = "Confirm Your Email";
        mailMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = emailBody
        };

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; //Ignore certificate errors (for testing only)
        smtp.Connect(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_HOST"), int.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PORT")!), SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME"), Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD"));
        await smtp.SendAsync(mailMessage);
        smtp.Disconnect(true);
    }
}
