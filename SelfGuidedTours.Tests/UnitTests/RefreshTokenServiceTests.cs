using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class RefreshTokenServiceTests
    {
        private SelfGuidedToursDbContext dbContext;
        private IRepository repository;
        private IRefreshTokenService refreshTokenService;

        [SetUp]
        public async Task SetupAsync()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                .UseInMemoryDatabase("SelfGuidedToursInMemoryDb" + Guid.NewGuid().ToString())
                .Options;

            dbContext = new SelfGuidedToursDbContext(dbContextOptions);

            repository = new Repository(dbContext);
            refreshTokenService = new RefreshTokenService(repository);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task CreateAsync_ShouldAddRefreshToken()
        {
            // Arrange
            var refreshToken = new RefreshToken
            {
                UserId = "test-user-id",
                Token = "test-token"
            };

            // Act
            await refreshTokenService.CreateAsync(refreshToken);

            // Assert
            var tokens = await repository.All<RefreshToken>().ToListAsync();
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(refreshToken.Token, tokens.First().Token);
        }

        [Test]
        public async Task DeleteAllAsync_ShouldDeleteAllTokensForUser()
        {
            // Arrange
            var refreshToken1 = new RefreshToken
            {
                UserId = "test-user-id",
                Token = "test-token-1"
            };

            var refreshToken2 = new RefreshToken
            {
                UserId = "test-user-id",
                Token = "test-token-2"
            };

            await repository.AddAsync(refreshToken1);
            await repository.AddAsync(refreshToken2);
            await repository.SaveChangesAsync();

            // Act
            await refreshTokenService.DeleteAllAsync("test-user-id");

            // Assert
            var tokens = await repository.All<RefreshToken>().ToListAsync();
            Assert.AreEqual(0, tokens.Count);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteTokenById()
        {
            // Arrange
            var refreshToken = new RefreshToken
            {
                UserId = "test-user-id",
                Token = "test-token"
            };

            await repository.AddAsync(refreshToken);
            await repository.SaveChangesAsync();

            // Act
            await refreshTokenService.DeleteAsync(refreshToken.Id);

            // Assert
            var tokens = await repository.All<RefreshToken>().ToListAsync();
            Assert.AreEqual(0, tokens.Count);
        }

        [Test]
        public void DeleteAsync_ShouldThrowArgumentExceptionForInvalidToken()
        {
            // Arrange

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await refreshTokenService.DeleteAsync(Guid.NewGuid()));
            Assert.AreEqual("Token is invalid!", ex.Message);
        }

        [Test]
        public async Task GetByTokenAsync_ShouldReturnCorrectToken()
        {
            // Arrange
            var refreshToken = new RefreshToken
            {
                UserId = "test-user-id",
                Token = "test-token"
            };

            await repository.AddAsync(refreshToken);
            await repository.SaveChangesAsync();

            // Act
            var result = await refreshTokenService.GetByTokenAsync("test-token");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(refreshToken.Token, result.Token);
        }

        [Test]
        public async Task GetByTokenAsync_ShouldReturnNullForInvalidToken()
        {
            // Act
            var result = await refreshTokenService.GetByTokenAsync("invalid-token");

            // Assert
            Assert.IsNull(result);
        }
    }
}
