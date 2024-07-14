using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Core.Services.BlobStorage;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class TourServiceTests
    {
        private SelfGuidedToursDbContext dbContext;
        private IRepository repository;
        private ITourService tourService;
        private IBlobService blobService;
        private ILandmarkService landmarkService;
        private ILogger<TourService> logger;
        private Mock<BlobServiceClient> blobServiceClientMock;
        private Mock<IBlobService> blobServiceMock;
        private Mock<ILandmarkService> landmarkServiceMock;

        [SetUp]
        public async Task SetupAsync()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                        .UseInMemoryDatabase("SelfGuidedToursInMemoryDb"
                            + Guid.NewGuid().ToString())
                        .Options;

            dbContext = new SelfGuidedToursDbContext(dbContextOptions);

            repository = new Repository(dbContext);

            blobServiceClientMock = new Mock<BlobServiceClient>();
            blobServiceMock = new Mock<IBlobService>();
            landmarkServiceMock = new Mock<ILandmarkService>();

            blobService = new BlobService(blobServiceClientMock.Object);

            logger = new Logger<TourService>(new LoggerFactory());

            tourService = new TourService(repository, blobServiceMock.Object, landmarkServiceMock.Object);

            var tours = new List<Tour>
            {
                new Tour
                {
                    TourId = 1,
                    CreatorId = "creator1",
                    Title = "Tour 1",
                    Summary = "Summary 1",
                    Price = 10.0m,
                    Destination = "Destination 1",
                    ThumbnailImageUrl = "http://example.com/thumb1",
                    EstimatedDuration = 60,
                    CreatedAt = DateTime.Now
                },
                new Tour
                {
                    TourId = 2,
                    CreatorId = "creator2",
                    Title = "Tour 2",
                    Summary = "Summary 2",
                    Price = 20.0m,
                    Destination = "Destination 2",
                    ThumbnailImageUrl = "http://example.com/thumb2",
                    EstimatedDuration = 120,
                    CreatedAt = DateTime.Now
                }
            };

            await dbContext.AddRangeAsync(tours);
            await dbContext.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetTourByIdAsync_ReturnsCorrectTour()
        {
            var tourId = 1;
            var result = await tourService.GetTourByIdAsync(tourId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Tour 1", result.Title);
        }

        [Test]
        public async Task CreateAsync_AddsTourToDatabase()
        {
            var tourCreateDto = new TourCreateDTO
            {
                Title = "Tour 3",
                Summary = "Summary 3",
                Price = 30.0m,
                Destination = "Destination 3",
                ThumbnailImage = new Mock<IFormFile>().Object,
                EstimatedDuration = 90,
                Landmarks = new List<LandmarkCreateTourDTO>()
            };

            blobServiceMock.Setup(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>(), true))
                .ReturnsAsync("http://example.com/thumb3");

            var newTour = await tourService.CreateAsync(tourCreateDto, "creator3");

            var result = await dbContext.Tours.FindAsync(newTour.TourId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Tour 3", result.Title);
        }
    }
}
