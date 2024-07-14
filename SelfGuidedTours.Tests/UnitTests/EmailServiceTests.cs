using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class EmailServiceTests : IDisposable
    {
        private SelfGuidedToursDbContext dbContext;
        private IRepository repository;
        private IEmailService emailService;
        private Mock<ILogger<EmailService>> loggerMock;
        private Mock<ISmtpClient> smtpClientMock;

        [SetUp]
        public void SetupAsync()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                .UseInMemoryDatabase("SelfGuidedToursInMemoryDb" + Guid.NewGuid().ToString())
                .Options;

            dbContext = new SelfGuidedToursDbContext(dbContextOptions);
            repository = new Repository(dbContext);

            loggerMock = new Mock<ILogger<EmailService>>();
            smtpClientMock = new Mock<ISmtpClient>();

            emailService = new EmailService();

            // Set environment variables
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_USERNAME", "testuser@example.com");
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_HOST", "smtp.example.com");
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_PORT", "587");
            Environment.SetEnvironmentVariable("ASPNETCORE_SMTP_PASSWORD", "password");
        }

        [TearDown]
        public void Dispose()
        {
            dbContext?.Dispose();
        }

        //[Test]
        //public async Task SendEmail_ShouldSendEmailSuccessfully()
        //{
        //    // Arrange
        //    var sendEmailDto = new SendEmailDto
        //    {
        //        To = "recipient@example.com",
        //        Subject = "Test Subject",
        //        Body = "Test Body"
        //    };

        //    smtpClientMock.Setup(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SecureSocketOptions>(), It.IsAny<System.Threading.CancellationToken>()))
        //        .Returns(Task.CompletedTask);
        //    smtpClientMock.Setup(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
        //        .Returns(Task.CompletedTask);
        //    smtpClientMock.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<System.Threading.CancellationToken>(), null))
        //        .Returns(Task.CompletedTask);
        //    smtpClientMock.Setup(x => x.DisconnectAsync(true, It.IsAny<System.Threading.CancellationToken>()))
        //        .Returns(Task.CompletedTask);

        //    // Act & Assert
        //    Assert.DoesNotThrowAsync(async () => await emailService.SendEmail(sendEmailDto, "plain"));
        //}

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

            StringAssert.Contains("Invalid email body format, possible formats \"html\", \"plain\", \"rtf\", \"enriched\"", ex.Message);
        }

        //[Test]
        //public async Task SendPasswordResetEmailAsync_ShouldSendEmailSuccessfully()
        //{
        //    // Arrange
        //    var email = "recipient@example.com";
        //    var resetLink = "http://example.com/resetpassword";

        //    smtpClientMock.Setup(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SecureSocketOptions>(), It.IsAny<System.Threading.CancellationToken>()))
        //        .Returns(Task.CompletedTask);
        //    smtpClientMock.Setup(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
        //        .Returns(Task.CompletedTask);
        //    smtpClientMock.Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<System.Threading.CancellationToken>(), null))
        //        .Returns(Task.CompletedTask);
        //    smtpClientMock.Setup(x => x.DisconnectAsync(true, It.IsAny<System.Threading.CancellationToken>()))
        //        .Returns(Task.CompletedTask);

        //    // Act & Assert
        //    Assert.DoesNotThrowAsync(async () => await emailService.SendPasswordResetEmailAsync(email, resetLink));
        //}
    }
}