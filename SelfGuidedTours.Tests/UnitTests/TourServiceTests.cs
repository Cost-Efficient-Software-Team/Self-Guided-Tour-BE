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
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;

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
                    CreatedAt = DateTime.Now,
                    Status = Status.Pending,
                    Landmarks = new List<Landmark>
                    {
                        new Landmark
                        {
                            LandmarkId = 1,
                            LocationName = "NDK",
                            Description = "National Palace of Culture",
                            StopOrder = 1,
                            Coordinate = new Coordinate
                            {
                                Latitude = 42.6863M,
                                Longitude = 23.3186M,
                                City = "Sofia",
                                Country = "Bulgaria"
                            },
                            Resources = new List<LandmarkResource>
                            {
                                new LandmarkResource
                                {
                                    LandmarkResourceId = 1,
                                    Url = "http://example.com/resource1",
                                    Type = ResourceType.Image
                                }
                            }
                        }
                    }
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
                    CreatedAt = DateTime.Now,
                    Status = Status.Approved,
                    Landmarks = new List<Landmark>()
                }
            };

            await dbContext.AddRangeAsync(tours);
            await dbContext.SaveChangesAsync();

            // Set environment variable for container name
            Environment.SetEnvironmentVariable("CONTAINER_NAME", "test-container");
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
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("thumbnail.jpg");
            formFileMock.Setup(f => f.Length).Returns(1024);
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[1024]));

            var tourCreateDto = new TourCreateDTO
            {
                Title = "Tour 3",
                Summary = "Summary 3",
                Price = 30.0m,
                Destination = "Destination 3",
                ThumbnailImage = formFileMock.Object,
                EstimatedDuration = 90,
                Landmarks = new List<LandmarkCreateTourDTO>
                {
                    new LandmarkCreateTourDTO
                    {
                        LocationName = "NDK",
                        Description = "National Palace of Culture",
                        Latitude = 42.6863M,
                        Longitude = 23.3186M,
                        City = "Sofia",
                        StopOrder = 1,
                        Resources = new List<IFormFile>()
                    }
                }
            };

            blobServiceMock.Setup(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>(), true))
                .ReturnsAsync("http://example.com/thumb3");

            landmarkServiceMock.Setup(l => l.CreateLandmarskForTourAsync(It.IsAny<ICollection<LandmarkCreateTourDTO>>(), It.IsAny<Tour>()))
                .ReturnsAsync(new List<Landmark>());

            var newTour = await tourService.CreateAsync(tourCreateDto, "creator3");

            var result = await dbContext.Tours.FindAsync(newTour.TourId);

            Assert.IsNotNull(result);
            Assert.AreEqual("Tour 3", result.Title);
        }

        [Test]
        public async Task DeleteTourAsync_DeletesTourAndRelatedEntities()
        {
            // Add a tour with landmarks and resources to the in-memory database
            var coordinate = new Coordinate
            {
                Latitude = 42.6863M,
                Longitude = 23.3186M,
                City = "Sofia",
                Country = "Bulgaria"
            };

            await repository.AddAsync(coordinate);
            await repository.SaveChangesAsync();

            var landmark = new Landmark
            {
                TourId = 1,
                StopOrder = 1,
                LocationName = "NDK",
                Description = "National Palace of Culture",
                Coordinate = coordinate,
                CoordinateId = coordinate.CoordinateId
            };

            await repository.AddAsync(landmark);
            await repository.SaveChangesAsync();

            var landmarkResource = new LandmarkResource
            {
                LandmarkId = landmark.LandmarkId,
                Url = "http://example.com/resource1",
                Type = ResourceType.Image
            };

            await repository.AddAsync(landmarkResource);
            await repository.SaveChangesAsync();

            blobServiceMock.Setup(b => b.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var response = await tourService.DeleteTourAsync(1);

            var deletedTour = await dbContext.Tours.FindAsync(1);
            var deletedLandmark = await dbContext.Landmarks.FindAsync(landmark.LandmarkId);
            var deletedResource = await dbContext.LandmarkResources.FindAsync(landmarkResource.LandmarkResourceId);
            var deletedCoordinate = await dbContext.Coordinates.FindAsync(coordinate.CoordinateId);

            Assert.IsNull(deletedTour);
            Assert.IsNull(deletedLandmark);
            Assert.IsNull(deletedResource);
            Assert.IsNull(deletedCoordinate);
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Test]
        public void MapTourToTourResponseDto_ReturnsCorrectResponseDto()
        {
            var tour = new Tour
            {
                TourId = 1,
                Title = "Tour 1",
                Summary = "Summary 1",
                Price = 10.0m,
                Destination = "Destination 1",
                ThumbnailImageUrl = "http://example.com/thumb1",
                EstimatedDuration = 60,
                CreatedAt = DateTime.Now,
                Status = Status.Pending,
                Landmarks = new List<Landmark>
                {
                    new Landmark
                    {
                        LandmarkId = 1,
                        LocationName = "NDK",
                        Description = "National Palace of Culture",
                        StopOrder = 1,
                        Coordinate = new Coordinate
                        {
                            Latitude = 42.6863M,
                            Longitude = 23.3186M,
                            City = "Sofia",
                            Country = "Bulgaria"
                        },
                        Resources = new List<LandmarkResource>
                        {
                            new LandmarkResource
                            {
                                LandmarkResourceId = 1,
                                Url = "http://example.com/resource1",
                                Type = ResourceType.Image
                            }
                        }
                    }
                }
            };

            var tourResponse = tourService.MapTourToTourResponseDto(tour);

            Assert.IsNotNull(tourResponse);
            Assert.AreEqual(tour.TourId, tourResponse.TourId);
            Assert.AreEqual(tour.Title, tourResponse.Title);
            Assert.AreEqual(tour.Summary, tourResponse.Summary);
            Assert.AreEqual(tour.Price, tourResponse.Price);
            Assert.AreEqual(tour.Destination, tourResponse.Destination);
            Assert.AreEqual(tour.ThumbnailImageUrl, tourResponse.ThumbnailImageUrl);
            Assert.AreEqual(tour.EstimatedDuration, tourResponse.EstimatedDuration);
            Assert.AreEqual(tour.Status.ToString(), tourResponse.Status);
            Assert.AreEqual(tour.Landmarks.Count, tourResponse.Landmarks.Count);

            var landmarkResponse = tourResponse.Landmarks.First();
            var landmark = tour.Landmarks.First();

            Assert.AreEqual(landmark.LandmarkId, landmarkResponse.LandmarkId);
            Assert.AreEqual(landmark.LocationName, landmarkResponse.LocationName);
            Assert.AreEqual(landmark.Description, landmarkResponse.Description);
            Assert.AreEqual(landmark.StopOrder, landmarkResponse.StopOrder);
            Assert.AreEqual(landmark.Coordinate.City, landmarkResponse.City);
            Assert.AreEqual(landmark.Coordinate.Latitude, landmarkResponse.Latitude);
            Assert.AreEqual(landmark.Coordinate.Longitude, landmarkResponse.Longitude);
            Assert.AreEqual(landmark.Resources.Count, landmarkResponse.Resources.Count);

            var resourceResponse = landmarkResponse.Resources.First();
            var resource = landmark.Resources.First();

            Assert.AreEqual(resource.LandmarkResourceId, resourceResponse.ResourceId);
            Assert.AreEqual(resource.Url, resourceResponse.ResourceUrl);
            Assert.AreEqual(resource.Type.ToString(), resourceResponse.ResourceType);
        }
        [Test]
        public async Task ApproveTourAsync_ApprovesTour()
        {
            // Arrange
            var tourId = 1;
            var result = await tourService.ApproveTourAsync(tourId);
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
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => tourService.ApproveTourAsync(tourId));
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
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => tourService.ApproveTourAsync(tourId));
            Assert.That(ex.Message, Is.EqualTo("Tour not found"));
        }
        [Test]
        public async Task RejectTourAsync_RejectsTour()
        {
            // Arrange
            var tourId = 1;
            var result = await tourService.RejectTourAsync(tourId);
            // Act
            var tour = await dbContext.Tours.FindAsync(tourId);
            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(tour?.Status, Is.EqualTo(Status.Rejected));

        }
        [Test]
        public async Task RejectTourAsync_ReturnsAlreadyRejected()
        {
            // Arrange
            var tourId = 1;
            var tour = await dbContext.Tours.FindAsync(tourId);
            tour!.Status = Status.Rejected;
            await dbContext.SaveChangesAsync();

            // Act && Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => tourService.RejectTourAsync(tourId));
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
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => tourService.RejectTourAsync(tourId));
            Assert.That(ex.Message, Is.EqualTo("Tour not found"));
        }
    }
}
