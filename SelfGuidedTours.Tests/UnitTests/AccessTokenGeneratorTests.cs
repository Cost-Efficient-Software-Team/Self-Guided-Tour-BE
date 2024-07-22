using Microsoft.EntityFrameworkCore;
using Moq;
using SelfGuidedTours.Core.Services.TokenGenerators;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Security.Claims;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class AccessTokenGeneratorTests : IDisposable
    {
        private SelfGuidedToursDbContext dbContext;
        private IRepository repository;
        private AccessTokenGenerator accessTokenGenerator;
        private Mock<TokenGenerator> mockTokenGenerator;
        private ApplicationUser testUser;

        [SetUp]
        public async Task SetupAsync()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                        .UseInMemoryDatabase("SelfGuidedToursInMemoryDb"
                            + Guid.NewGuid().ToString())
                        .Options;

            dbContext = new SelfGuidedToursDbContext(dbContextOptions);
            repository = new Repository(dbContext);

            mockTokenGenerator = new Mock<TokenGenerator>();

            accessTokenGenerator = new AccessTokenGenerator(mockTokenGenerator.Object);

            testUser = new ApplicationUser()
            {
                Id = "e3bdf03e-ec2c-4b2d-be4b-a10d35b431c0",
                Email = "user@selfguidedtours.bg",
                Name = "User Userov"
            };

            await dbContext.Users.AddAsync(testUser);
            await dbContext.SaveChangesAsync();

            Environment.SetEnvironmentVariable("ACCESSTOKEN_KEY", "4y7XS2AHicSOs2uUJCxwlHWqTJNExW3UDUjMeXi96uLEso1YV4RazqQubpFBdx0zZGtdxBelKURhh0WXxPR0mEJQHk_0U9HeYtqcMManhoP3X2Ge8jgxh6k4C_Gd4UPTc6lkx0Ca5eRE16ciFQ6wmYDnaXC8NbngGqartHccAxE");
            Environment.SetEnvironmentVariable("ACCESSTOKEN_EXPIRATIONMINUTES", "50");
            Environment.SetEnvironmentVariable("ISSUER", "https://localhost:7038");
            Environment.SetEnvironmentVariable("AUDIENCE", "https://localhost:7038");
        }

        [Test]
        public void GenerateToken_ShouldReturnToken()
        {
            // Arrange
            var expectedToken = "generated_token";
            mockTokenGenerator.Setup(t => t.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<ICollection<Claim>?>()))
                              .Returns(expectedToken);

            // Act
            var token = accessTokenGenerator.GenerateToken(testUser);

            // Assert
            Assert.That(token, Is.EqualTo(expectedToken));
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        public void Dispose()
        {
            dbContext?.Dispose();
        }
    }
}
