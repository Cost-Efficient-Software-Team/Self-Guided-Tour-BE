using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;
using Stripe;
using System.Net;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class AdminServiceTests
    {
        private SelfGuidedToursDbContext dbContext;
        private IRepository repository;
        private IAdminService service;
        private ILogger<AdminService> logger;
        private ILoggerFactory loggerFactory;
        private Mock<ITourService> tourServiceMock;
        private ApplicationUser user;
        private Tour TestTour { get; set; }

        [SetUp]
        public async Task SetupAsync()
        {
            user = new ApplicationUser()
            {
                Id = "13ae25c7-7d22-48b3-844c-4738c055d648",
                UserName = "TestUser",
                Name = "TestName",
                Email = ""
            };

            TestTour = new Tour()
            {
                TourId = 1,
                CreatedAt = DateTime.UtcNow,
                Status = Status.UnderReview,
                Summary = "Test summary",
                CreatorId = "13ae25c7-7d22-48b3-844c-4738c055d648",
                Destination = "Test destination",
                EstimatedDuration = 120,
                Price = 10,
                Title = "Test title",       
                ThumbnailImageUrl = "random url test"
            };

            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                        .UseInMemoryDatabase("SelfGuidedToursInMemoryDb"
                            + Guid.NewGuid().ToString())
                        .Options;

            dbContext = new SelfGuidedToursDbContext(dbContextOptions);
            await dbContext.AddAsync(user);
            await dbContext.AddAsync(TestTour);
            await dbContext.SaveChangesAsync();

            loggerFactory = new LoggerFactory();
            logger = new Logger<AdminService>(loggerFactory);
            tourServiceMock = new Mock<ITourService>();
            repository = new Repository(dbContext);
            service = new AdminService(repository, logger, tourServiceMock.Object);
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.DisposeAsync();
            loggerFactory.Dispose();
        }

        [Test]
        public async Task Test_GetAllToursAsync_ShouldReturnCorrectResult()
        {
           
            var result = await service.GetAllToursAsync();

            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Test title"));
            Assert.That(result.First().Status, Is.EqualTo("Under Review"));
        }
       
        [Test]
        public async Task Test_GetAllToursAsync_ShouldThrowAnExceptionCorrectly()
        {
            try
            {
                Status status = Status.Approved;
                _ = await service.GetAllToursAsync(status);
            }
            catch(KeyNotFoundException kfe)
            {
                Assert.That(kfe.Message, Is.EqualTo("No tours found!"));
            }
        }
        [Test]
        public async Task ApproveTourAsync_ApprovesTour()
        {
            // Arrange
            var tourId = 1;
            var result = await service.ApproveTourAsync(tourId);
            // Act
            var tour = await dbContext.Tours.FindAsync(tourId);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(tour?.Status, Is.EqualTo(Status.Approved));
        }
        [Test]
        public async Task ApproveTourAsync_ReturnsAlreadyApproved()
        {
            // Arrange
            var tourId = 1;
            var tour = await dbContext.Tours.FindAsync(tourId);
            tour!.Status = Status.Approved;
            await dbContext.SaveChangesAsync();

            // Act && Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => service.ApproveTourAsync(tourId));
            Assert.That(ex.Message, Is.EqualTo("Tour is already approved"));
        }
        [Test]
        public async Task ApproveTourAsync_ReturnsNotFound()
        {
            // Arrange
            var tourId = 1; // Wrong Tour Id
            var tour = await dbContext.Tours.FindAsync(tourId);
            repository.Delete(tour!);  // Make sure that there is no tour assigned to the given id
            await dbContext.SaveChangesAsync();

            // Act && Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => service.ApproveTourAsync(tourId));
            Assert.That(ex.Message, Is.EqualTo("Tour not found"));
        }
        [Test]
        public async Task RejectTourAsync_RejectsTour()
        {
            // Arrange
            var tourId = 1;
            var result = await service.RejectTourAsync(tourId);
            // Act
            var tour = await dbContext.Tours.FindAsync(tourId);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(tour?.Status, Is.EqualTo(Status.Declined));

        }
        [Test]
        public async Task RejectTourAsync_ReturnsAlreadyRejected()
        {
            // Arrange
            var tourId = 1;
            var tour = await dbContext.Tours.FindAsync(tourId);
            tour!.Status = Status.Declined;
            await dbContext.SaveChangesAsync();

            // Act && Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => service.RejectTourAsync(tourId));
            Assert.That(ex.Message, Is.EqualTo("Tour is already rejected"));
        }
        [Test]
        public async Task RejectTourAsync_ReturnsNotFound()
        {
            // Arrange
            var tourId = 1; // Wrong Tour Id
            var tour = await dbContext.Tours.FindAsync(tourId);
            repository.Delete(tour!);  // Make sure that there is no tour assigned to the given id
            await dbContext.SaveChangesAsync();

            // Act && Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => service.RejectTourAsync(tourId));
            Assert.That(ex.Message, Is.EqualTo("Tour not found"));
        }
    }
}
