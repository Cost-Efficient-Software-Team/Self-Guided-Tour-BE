using Microsoft.AspNetCore.Http;
using Moq;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class TourServiceTests
    {
        private Mock<IRepository> repositoryMock;
        private Mock<IBlobService> blobServiceMock;
        private Mock<ILandmarkService> landmarkServiceMock;
        private TourService tourService;

        [SetUp]
        public void Setup()
        {
            repositoryMock = new Mock<IRepository>();
            blobServiceMock = new Mock<IBlobService>();
            landmarkServiceMock = new Mock<ILandmarkService>();
            tourService = new TourService(repositoryMock.Object, blobServiceMock.Object, landmarkServiceMock.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateTourProperly()
        {
            // Arrange
            var fileContent = new MemoryStream(new byte[0]);
            var formFile = new FormFileMock(fileContent, "test.jpg", "image/jpeg");

            var tourCreateDTO = new TourCreateDTO
            {
                Title = "Test Tour",
                Summary = "Test Summary",
                Price = 100,
                Destination = "Test Destination",
                ThumbnailImage = formFile,
                EstimatedDuration = 120,
                Landmarks = new List<LandmarkCreateTourDTO>()
            };

            var creatorId = "test-creator-id";
            var containerName = "test-container";
            var fileName = "test-file-name";
            var thumbnailUrl = "http://example.com/test-thumbnail.jpg";

            Environment.SetEnvironmentVariable("CONTAINER_NAME", containerName);
            blobServiceMock.Setup(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<bool>()))
                           .ReturnsAsync(thumbnailUrl);

            // Act
            var result = await tourService.CreateAsync(tourCreateDTO, creatorId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(tourCreateDTO.Title, result.Title);
            Assert.AreEqual(thumbnailUrl, result.ThumbnailImageUrl);
        }

        [Test]
        public async Task DeleteTourAsync_ShouldDeleteTourProperly()
        {
            // Arrange
            var tourId = 1;
            var tour = new Tour { TourId = tourId, ThumbnailImageUrl = "http://example.com/test-thumbnail.jpg" };
            var landmarks = new List<Landmark> { new Landmark { TourId = tourId } };

            repositoryMock.Setup(r => r.GetByIdAsync<Tour>(tourId)).ReturnsAsync(tour);
            repositoryMock.Setup(r => r.All<Landmark>()).Returns(landmarks.AsQueryable());

            var containerName = "test-container";
            Environment.SetEnvironmentVariable("CONTAINER_NAME", containerName);

            // Act
            var result = await tourService.DeleteTourAsync(tourId);

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task GetTourByIdAsync_ShouldReturnTourProperly()
        {
            // Arrange
            var tourId = 1;
            var tour = new Tour { TourId = tourId, Title = "Test Tour" };

            repositoryMock.Setup(r => r.AllReadOnly<Tour>())
                          .Returns(new List<Tour> { tour }.AsQueryable());

            // Act
            var result = await tourService.GetTourByIdAsync(tourId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(tour.Title, result.Title);
        }

        [Test]
        public void GetTourByIdAsync_ShouldThrowExceptionIfTourNotFound()
        {
            // Arrange
            var tourId = 1;
            repositoryMock.Setup(r => r.AllReadOnly<Tour>()).Returns(Enumerable.Empty<Tour>().AsQueryable());

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => tourService.GetTourByIdAsync(tourId));
            Assert.AreEqual("Tour not found", ex.Message);
        }
    }
}
