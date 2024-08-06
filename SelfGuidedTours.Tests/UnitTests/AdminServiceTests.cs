using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;

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

        private Tour TestTour { get; set; }

        [SetUp]
        public async Task SetupAsync()
        {
            TestTour = new Tour()
            {
                TourId = 1,
                CreatedAt = DateTime.UtcNow,
                Status = Status.Pending,
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

            await dbContext.AddAsync(TestTour);
            await dbContext.SaveChangesAsync();

            loggerFactory = new LoggerFactory();
            logger = new Logger<AdminService>(loggerFactory);

            repository = new Repository(dbContext);
            service = new AdminService(repository, logger);
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
            Status status = Status.Pending;
            var result = await service.GetAllToursAsync(status);

            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Test title"));
            Assert.That(result.First().Status, Is.EqualTo("Pending"));
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
    }
}
