using Microsoft.Extensions.Configuration;
using Moq;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Services;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class EmailServiceTests
    {
        private IEmailService emailService;

        [SetUp]
        public void Setup()
        {

            
            var blobServiceMock = new Mock<IBlobService>();
            blobServiceMock.Setup(x => x.GetEmailTemplateAsync(It.IsAny<string>()))
                .ReturnsAsync("Test email template");

            emailService = new EmailService(blobServiceMock.Object);

            // Set environment variables for testing
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME", "testuser@example.com");
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_HOST", "smtp.gmail.com");
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_PORT", "587");
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD", "testpassword");
            Environment.SetEnvironmentVariable("GenericEmailTemplate", "genericEmailTemplate");
            Environment.SetEnvironmentVariable("PasswordResetEmailTemplate", "passwordResetEmailTemplate");
           
        }

        [TearDown]
        public void TearDown()
        {
            // Clear environment variables after each test
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME", null);
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_HOST", null);
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_PORT", null);
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD", null);
        }

       

        [Test]
        public void SendEmail_ShouldThrowApplicationException_WhenEnvironmentVariablesAreMissing()
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME", null);

            var sendEmailDto = new SendEmailDto
            {
                To = "recipient@example.com",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ApplicationException>(async () =>
                await emailService.SendEmail(sendEmailDto, "plain"));

            StringAssert.Contains("Email sender is not configured", ex.Message);
        }

        [Test]
        public void SendPasswordResetEmailAsync_ShouldThrowArgumentNullException_WhenEnvironmentVariablesAreMissing()
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_HOST", null);

            var email = "recipient@example.com";
            var resetLink = "http://example.com/resetpassword";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await emailService.SendPasswordResetEmailAsync(email, resetLink));

            StringAssert.Contains("Value cannot be null. (Parameter 'host')", ex.Message);
        }
    
    }
}
