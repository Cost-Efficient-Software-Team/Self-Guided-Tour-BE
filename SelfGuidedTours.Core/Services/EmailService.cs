using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models;

public class EmailService : IEmailService
{
    private readonly IBlobService blobService;

    public EmailService( IBlobService blobService)
    {
        this.blobService = blobService;
    }

    public async Task SendEmail(SendEmailDto sendEmailRequest, string emailBodyFormat = "html")
    {
        var templateUrl = Environment.GetEnvironmentVariable("GenericEmailTemplate") 
             ?? throw new ApplicationException("Email template is not configured.");
        
       

        var emailBody = await blobService.GetEmailTemplateAsync(templateUrl);
        emailBody = emailBody.Replace("{{EmailBody}}", sendEmailRequest.Body);
        emailBody = emailBody.Replace("{{Subject}}", sendEmailRequest.Subject);

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
            Text = emailBody
        };

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Ignore certificate errors (for testing only)
        smtp.Connect(emailHost, int.Parse(emailPort), SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(emailSender, emailPassword);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        var templateUrl = Environment.GetEnvironmentVariable("PasswordResetEmailTemplate") 
                ?? throw new ApplicationException("Password reset email template is not configured.");


        var emailBody = await blobService.GetEmailTemplateAsync(templateUrl);
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
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Ignore certificate errors (for testing only)
        smtp.Connect(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_HOST"), int.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PORT")!), SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME"), Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD"));
        await smtp.SendAsync(mailMessage);
        smtp.Disconnect(true);
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        var templateUrl = Environment.GetEnvironmentVariable("ConfirmationEmailTemplate")
                ?? throw new ApplicationException("Email confirmation template is not configured.");

        var emailBody = await blobService.GetEmailTemplateAsync(templateUrl);
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
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Ignore certificate errors (for testing only)
        smtp.Connect(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_HOST"), int.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PORT")!), SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME"), Environment.GetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD"));
        await smtp.SendAsync(mailMessage);
        smtp.Disconnect(true);
    }
}
