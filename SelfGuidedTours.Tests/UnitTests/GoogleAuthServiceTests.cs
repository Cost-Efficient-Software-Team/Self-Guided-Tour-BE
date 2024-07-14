using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.ExternalLogin;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class GoogleAuthServiceTests : IDisposable
    {
        private SelfGuidedToursDbContext dbContext;
        private IRepository repository;
        private IAuthService authService;
        private GoogleAuthService googleAuthService;

        private IEnumerable<ApplicationUser> users;
        private Mock<HttpMessageHandler> httpMessageHandlerMock;
        private HttpClient httpClient;

        #region User

        private ApplicationUser User { get; set; }

        #endregion

        [SetUp]
        public async Task SetupAsync()
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            #region User initializing

            User = new ApplicationUser()
            {
                Id = "e3bdf03e-ec2c-4b2d-be4b-a10d35b431c0",
                Email = "user@selfguidedtours.bg",
                NormalizedEmail = "user@selfguidedtours.bg".ToUpper(),
                Name = "User Userov",
                NormalizedUserName = "User Userov".ToUpper(),
                PasswordHash = hasher.HashPassword(null!, "D01Parola")
            };

            #endregion

            users = new List<ApplicationUser>() { User };

            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                        .UseInMemoryDatabase("SelfGuidedToursInMemoryDb"
                            + Guid.NewGuid().ToString())
                        .Options;

            dbContext = new SelfGuidedToursDbContext(dbContextOptions);

            await dbContext.AddRangeAsync(users);
            await dbContext.SaveChangesAsync();

            repository = new Repository(dbContext);

            var loggerFactory = new LoggerFactory();
            var logger = new Logger<GoogleAuthService>(loggerFactory);

            var authServiceMock = new Mock<IAuthService>();
            authServiceMock
                .Setup(x => x.GoogleSignInAsync(It.IsAny<GoogleUserDto>()))
                .ReturnsAsync(new AuthenticateResponse
                {
                    AccessToken = "dummy_access_token",
                    RefreshToken = "dummy_refresh_token"
                });

            authService = authServiceMock.Object;

            googleAuthService = new GoogleAuthService(authService);

            httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpClient = new HttpClient(httpMessageHandlerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            httpClient.Dispose();
        }

        [Test]
        public async Task GoogleSignIn_WithValidIdToken_ReturnsAuthenticateResponse()
        {
            // Arrange
            var idToken = "valid_id_token";
            var googleSignInVM = new GoogleSignInVM { IdToken = idToken };

            // Mock the HttpMessageHandler to return a valid response
            var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"id\": \"123\", \"email\": \"user@selfguidedtours.bg\" }")
            };
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req != null),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            // Inject the mock HttpClient into the GoogleAuthService
            var googleAuthService = new GoogleAuthService(authService);
            googleAuthService.SetHttpClient(httpClient);

            // Act
            var result = await googleAuthService.GoogleSignIn(googleSignInVM);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("dummy_access_token", result.AccessToken);
            Assert.AreEqual("dummy_refresh_token", result.RefreshToken);
        }

        [Test]
        public void GoogleSignIn_WithInvalidIdToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var idToken = "invalid_id_token";
            var googleSignInVM = new GoogleSignInVM { IdToken = idToken };

            // Mock the HttpMessageHandler to return an unauthorized response
            var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req != null),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            // Inject the mock HttpClient into the GoogleAuthService
            var googleAuthService = new GoogleAuthService(authService);
            googleAuthService.SetHttpClient(httpClient);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await googleAuthService.GoogleSignIn(googleSignInVM));
        }

        public void Dispose()
        {
            dbContext?.Dispose();
            httpClient?.Dispose();
        }
    }

    // Extension method to inject HttpClient into GoogleAuthService
    public static class GoogleAuthServiceExtensions
    {
        public static void SetHttpClient(this GoogleAuthService googleAuthService, HttpClient httpClient)
        {
            googleAuthService.GetType()
                             .GetProperty("HttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                             ?.SetValue(googleAuthService, httpClient);
        }
    }
}
