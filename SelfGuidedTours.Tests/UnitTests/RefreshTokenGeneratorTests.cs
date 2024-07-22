using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Services.TokenGenerators;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class RefreshTokenGeneratorTests : IDisposable
    {
        private SelfGuidedToursDbContext dbContext;
        private IRepository repository;
        private RefreshTokenGenerator refreshTokenGenerator;
        private TokenGenerator tokenGenerator;

        [SetUp]
        public async Task SetupAsync()
        {
            // Set environment variables from launchSettings.json
            Environment.SetEnvironmentVariable("REFRESHTOKEN_KEY", "a7990245-e3f1-4dcd-95a4-c2cde60eb8df");
            Environment.SetEnvironmentVariable("REFRESHTOKEN_EXPIRATIONMINUTES", "131400");
            Environment.SetEnvironmentVariable("ISSUER", "https://localhost:7038");
            Environment.SetEnvironmentVariable("AUDIENCE", "https://localhost:7038");

            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                .UseInMemoryDatabase("SelfGuidedToursInMemoryDb"
                    + Guid.NewGuid().ToString())
                .Options;

            dbContext = new SelfGuidedToursDbContext(dbContextOptions);
            repository = new Repository(dbContext);
            tokenGenerator = new TokenGenerator();
            refreshTokenGenerator = new RefreshTokenGenerator(tokenGenerator);

            await dbContext.Database.EnsureCreatedAsync();
        }

        [TearDown]
        public void Dispose()
        {
            dbContext?.Dispose();
        }

        [Test]
        public void GenerateToken_ShouldReturnToken_WhenConfigurationIsValid()
        {
            var token = refreshTokenGenerator.GenerateToken();

            Assert.IsNotNull(token);
            Assert.IsNotEmpty(token);
        }

        [Test]
        public void GenerateToken_ShouldThrowException_WhenKeyIsMissing()
        {
            Environment.SetEnvironmentVariable("REFRESHTOKEN_KEY", null);

            var ex = Assert.Throws<ApplicationException>(() => refreshTokenGenerator.GenerateToken());
            Assert.AreEqual("REFRESHTOKEN_KEY is not configured.", ex.Message);
        }

        [Test]
        public void GenerateToken_ShouldThrowException_WhenExpirationIsMissing()
        {
            Environment.SetEnvironmentVariable("REFRESHTOKEN_EXPIRATIONMINUTES", null);

            var ex = Assert.Throws<ApplicationException>(() => refreshTokenGenerator.GenerateToken());
            Assert.AreEqual("REFRESHTOKEN_EXPIRATIONMINUTES is not configured.", ex.Message);
        }

        [Test]
        public void GenerateToken_ShouldThrowException_WhenIssuerIsMissing()
        {
            Environment.SetEnvironmentVariable("ISSUER", null);

            var ex = Assert.Throws<ApplicationException>(() => refreshTokenGenerator.GenerateToken());
            Assert.AreEqual("ISSUER is not configured.", ex.Message);
        }

        [Test]
        public void GenerateToken_ShouldThrowException_WhenAudienceIsMissing()
        {
            Environment.SetEnvironmentVariable("AUDIENCE", null);

            var ex = Assert.Throws<ApplicationException>(() => refreshTokenGenerator.GenerateToken());
            Assert.AreEqual("AUDIENCE is not configured.", ex.Message);
        }
    }
}
