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

            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
        }

        [Test]
        public void GenerateToken_ShouldThrowException_WhenKeyIsMissing()
        {
            Environment.SetEnvironmentVariable("REFRESHTOKEN_KEY", null);

            var ex = Assert.Throws<ApplicationException>(() => refreshTokenGenerator.GenerateToken());
            Assert.That(ex.Message, Is.EqualTo("REFRESHTOKEN_KEY is not configured."));
        }

        [Test]
        public void GenerateToken_ShouldThrowException_WhenExpirationIsMissing()
        {
            Environment.SetEnvironmentVariable("REFRESHTOKEN_EXPIRATIONMINUTES", null);

            var ex = Assert.Throws<ApplicationException>(() => refreshTokenGenerator.GenerateToken());
            Assert.That(ex.Message, Is.EqualTo("REFRESHTOKEN_EXPIRATIONMINUTES is not configured."));
        }

        [Test]
        public void GenerateToken_ShouldThrowException_WhenIssuerIsMissing()
        {
            Environment.SetEnvironmentVariable("ISSUER", null);

            var ex = Assert.Throws<ApplicationException>(() => refreshTokenGenerator.GenerateToken());
            Assert.That(ex.Message, Is.EqualTo("ISSUER is not configured."));
        }

        [Test]
        public void GenerateToken_ShouldThrowException_WhenAudienceIsMissing()
        {
            Environment.SetEnvironmentVariable("AUDIENCE", null);

            var ex = Assert.Throws<ApplicationException>(() => refreshTokenGenerator.GenerateToken());
            Assert.That(ex.Message, Is.EqualTo("AUDIENCE is not configured."));
        }
    }
}
