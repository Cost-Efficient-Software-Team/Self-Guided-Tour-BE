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
        public void Setup()
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
            Assert.That(tokens.Count, Is.EqualTo(1));
            Assert.That(tokens.First().Token, Is.EqualTo(refreshToken.Token));
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
            Assert.That(tokens.Count, Is.EqualTo(0));
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
            Assert.That(tokens.Count, Is.EqualTo(0));
        }

        [Test]
        public void DeleteAsync_ShouldThrowArgumentExceptionForInvalidToken()
        {
            // Arrange

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await refreshTokenService.DeleteAsync(Guid.NewGuid()));
            Assert.That(ex.Message, Is.EqualTo("Token is invalid!"));
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
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Token, Is.EqualTo(refreshToken.Token));
        }

        [Test]
        public async Task GetByTokenAsync_ShouldReturnNullForInvalidToken()
        {
            // Act
            var result = await refreshTokenService.GetByTokenAsync("invalid-token");

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
