using MailKit.Net.Smtp;
using Moq;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Services;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class EmailServiceTests
    {
        private IEmailService emailService;
        private Mock<ISmtpClient> smtpClientMock;

        [SetUp]
        public void Setup()
        {
            smtpClientMock = new Mock<ISmtpClient>();
            emailService = new EmailService();

            // Set environment variables
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME", "testuser@example.com");
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_HOST", "smtp.example.com");
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_PORT", "587");
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD", "password");
        }

        [Test]
        public async Task SendEmail_ShouldSendEmailSuccessfully()
        {
            // Arrange
            var sendEmailDto = new SendEmailDto
            {
                To = "recipient@example.com",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await emailService.SendEmail(sendEmailDto, "plain"));
        }

        [Test]
        public void SendEmail_ShouldThrowArgumentException_ForInvalidEmailBodyFormat()
        {
            // Arrange
            var sendEmailDto = new SendEmailDto
            {
                To = "recipient@example.com",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await emailService.SendEmail(sendEmailDto, "invalidFormat"));

            StringAssert.Contains("Invalid email body format", ex.Message);
        }

        [Test]
        public async Task SendPasswordResetEmailAsync_ShouldSendEmailSuccessfully()
        {
            // Arrange
            var email = "recipient@example.com";
            var resetLink = "http://example.com/resetpassword";

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await emailService.SendPasswordResetEmailAsync(email, resetLink));
        }
    }
}