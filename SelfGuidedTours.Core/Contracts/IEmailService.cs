using SelfGuidedTours.Core.Models;

namespace SelfGuidedTours.Core.Contracts
{
    public interface IEmailService
    {
        Task SendEmail(SendEmailDto sendEmailRequest, string emailBodyFormat);
        Task SendPasswordResetEmailAsync(string email, string resetLink);
        Task SendEmailConfirmationAsync(string email, string confirmationLink); // Added method
    }
}
