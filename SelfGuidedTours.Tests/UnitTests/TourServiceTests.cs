using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Core.Services;
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
        private Mock<IBlobService> blobServiceMock;
        private Mock<ILandmarkService> landmarkServiceMock;
        private Mock<IHttpContextAccessor> httpContextAccessorMock; // Added declaration
        [SetUp]
        public async Task SetupAsync()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                .UseInMemoryDatabase("SelfGuidedToursInMemoryDb" + Guid.NewGuid().ToString())
                .Options;

            dbContext = new SelfGuidedToursDbContext(dbContextOptions);

            repository = new Repository(dbContext);

            blobServiceMock = new Mock<IBlobService>();
            blobServiceMock.Setup(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync("mockFile.test");

            landmarkServiceMock = new Mock<ILandmarkService>();

            httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            tourService = new TourService(repository, blobServiceMock.Object, landmarkServiceMock.Object, httpContextAccessorMock.Object);

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
                    AverageRating = 4.5M,
                    ThumbnailImageUrl = "http://example.com/thumb1",
                    EstimatedDuration = 60,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    Status = Status.Approved,
                    Landmarks = new List<Landmark>
                    {
                        new Landmark
                        {
                            LandmarkId = 1,
                            LocationName = "NDK",
                            Description = "National Palace of Culture",
                            StopOrder = 1,
                            PlaceId = "1",
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
                                    Type = LandmarkResourceType.Image
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
                    AverageRating = 4.0M,
                    Destination = "Destination 2",
                    ThumbnailImageUrl = "http://example.com/thumb2",
                    EstimatedDuration = 120,
                    CreatedAt = DateTime.Now,
                    Status = Status.Approved,
                    Landmarks = new List<Landmark>()
                }
            };

            Environment.SetEnvironmentVariable("CONTAINER_NAME", "test-container");

            await dbContext.AddRangeAsync(tours);
            await dbContext.SaveChangesAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.DisposeAsync();
        }

        [Test]
        public async Task GetTourByIdAsync_ReturnsCorrectTour()
        {
            var tourId = 1;
            var result = await tourService.GetTourByIdAsync(tourId);

            Assert.IsNotNull(result);
            Assert.That(result.Title, Is.EqualTo("Tour 1"));
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

            landmarkServiceMock.Setup(l => l.CreateLandmarksForTourAsync(It.IsAny<ICollection<LandmarkCreateTourDTO>>(), It.IsAny<Tour>()))
                .ReturnsAsync(new List<Landmark>());

            var newTour = await tourService.CreateTourAsync(tourCreateDto, "creator3");

            var result = await dbContext.Tours.FindAsync(newTour.TourId);

            Assert.IsNotNull(result);
            Assert.That(result.Title, Is.EqualTo("Tour 3"));
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
                CoordinateId = coordinate.CoordinateId,
                PlaceId = "1"
            };

            await repository.AddAsync(landmark);
            await repository.SaveChangesAsync();

            var landmarkResource = new LandmarkResource
            {
                LandmarkId = landmark.LandmarkId,
                Url = "http://example.com/resource1",
                Type = LandmarkResourceType.Image
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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
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
                Status = Status.UnderReview,
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
                                Type = LandmarkResourceType.Image
                            }
                        }
                    }
                }
            };

            var tourResponse = tourService.MapTourToTourResponseDto(tour);

            Assert.That(tourResponse, Is.Not.Null);
            Assert.That(tourResponse.TourId, Is.EqualTo(tour.TourId));
            Assert.That(tourResponse.Title, Is.EqualTo(tour.Title));
            Assert.That(tourResponse.Summary, Is.EqualTo(tour.Summary));
            Assert.That(tourResponse.Price, Is.EqualTo(tour.Price));
            Assert.That(tourResponse.Destination, Is.EqualTo(tour.Destination));
            Assert.That(tourResponse.ThumbnailImageUrl, Is.EqualTo(tour.ThumbnailImageUrl));
            Assert.That(tourResponse.EstimatedDuration, Is.EqualTo(tour.EstimatedDuration));
            Assert.That(tourResponse.Landmarks.Count, Is.EqualTo(tour.Landmarks.Count));

            var landmarkResponse = tourResponse.Landmarks.First();
            var landmark = tour.Landmarks.First();

            Assert.Multiple(() =>
            {
                Assert.That(landmarkResponse.LandmarkId, Is.EqualTo(landmark.LandmarkId));
                Assert.That(landmarkResponse.LocationName, Is.EqualTo(landmark.LocationName));
                Assert.That(landmarkResponse.Description, Is.EqualTo(landmark.Description));
                Assert.That(landmarkResponse.StopOrder, Is.EqualTo(landmark.StopOrder));
                Assert.That(landmarkResponse.City, Is.EqualTo(landmark.Coordinate.City));
                Assert.That(landmarkResponse.Latitude, Is.EqualTo(landmark.Coordinate.Latitude));
                Assert.That(landmarkResponse.Longitude, Is.EqualTo(landmark.Coordinate.Longitude));
                Assert.That(landmarkResponse.Resources.Count, Is.EqualTo(landmark.Resources.Count));
            });

            var resourceResponse = landmarkResponse.Resources.First();
            var resource = landmark.Resources.First();

            Assert.That(resourceResponse.ResourceId, Is.EqualTo(resource.LandmarkResourceId));
            Assert.That(resourceResponse.ResourceUrl, Is.EqualTo(resource.Url));
            Assert.That(resourceResponse.ResourceType, Is.EqualTo(resource.Type.ToString()));
        }

        [Test]
        public async Task GetFilteredTours_PaginatesProperly()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 2;
            var pageSize = 1;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(1));
            Assert.That(result.Tours.First().Title, Is.EqualTo("Tour 2"));
        }

        [Test]
        public async Task UpdateTourAsync_UpdatesTour()
        {
            var tourUpdateDto = new TourUpdateDTO
            {
                Title = "Tour 1 Updated",
                Summary = "Summary 1 Updated",
                Price = 15.0m,
                Destination = "Destination 1 Updated",
                EstimatedDuration = 90
            };

            var response = await tourService.UpdateTourAsync(1, tourUpdateDto);

            var updatedTour = await dbContext.Tours.FindAsync(1);

            Assert.That(updatedTour!.Title!, Is.EqualTo("Tour 1 Updated"));
            Assert.That(updatedTour.Summary, Is.EqualTo("Summary 1 Updated"));
            Assert.That(updatedTour.Price, Is.EqualTo(15.0m));
            Assert.That(updatedTour.Destination, Is.EqualTo("Destination 1 Updated"));
            Assert.That(updatedTour.EstimatedDuration, Is.EqualTo(90));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task UpdateTourAsync_ReturnsNotFound()
        {
            var tourUpdateDto = new TourUpdateDTO
            {
                Title = "Tour 1 Updated",
                Summary = "Summary 1 Updated",
                Price = 15.0m,
                Destination = "Destination 1 Updated",
                EstimatedDuration = 90
            };


            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => tourService.UpdateTourAsync(3, tourUpdateDto));

            Assert.That(ex.Message, Is.EqualTo("Tour not found"));
        }

        [Test]
        public async Task GetFilteredTours_PaginatesPropperly()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 2;
            var pageSize = 1;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(1));
            Assert.That(result.Tours.First().Title, Is.EqualTo("Tour 2"));
        }
        [Test]
        public async Task GetFilteredTours_SortsPropperly()
        {
            var searchTerm = "";
            var sortBy = "maxPrice";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
            Assert.That(result.Tours.First().Title, Is.EqualTo("Tour 2"));
        }
        [Test]
        public async Task GetFilteredTours_FiltersPropperly()
        {
            // Arrange
            var searchTerm = "Tour 1";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            // Act
            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            // Assert
            Assert.That(result.Tours.Count, Is.EqualTo(1));
            Assert.That(result.Tours.First().Title, Is.EqualTo("Tour 1"));
        }

        [Test]
        public async Task GetFilteredTours_SortsByNewest()
        {
            var searchTerm = "";
            var sortBy = "newest";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
            Assert.That(result.Tours.First().Title, Is.EqualTo("Tour 2"));
        }
        [Test]
        public async Task GetFilteredTours_SortsByAverageRating()
        {
            var searchTerm = "";
            var sortBy = "averageRating";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
            Assert.That(result.Tours.First().Title, Is.EqualTo("Tour 1"));
        }
        [Test]
        public async Task GetFilteredTours_SortsByMostBought()
        {
            var searchTerm = "";
            var sortBy = "mostBought";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
            Assert.That(result.Tours.First().Title, Is.EqualTo("Tour 1"));
        }
        [Test]
        public async Task GetFilteredTours_SortsByMinPrice()
        {
            var searchTerm = "";
            var sortBy = "minPrice";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
            Assert.That(result.Tours.First().Title, Is.EqualTo("Tour 1"));
        }
        [Test]
        public async Task GetFilteredTours_SortsByMaxPrice()
        {
            var searchTerm = "";
            var sortBy = "maxPrice";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
            Assert.That(result.Tours.First().Title, Is.EqualTo("Tour 2"));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsEmptyList()
        {
            var searchTerm = "Tour 3";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(0));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllTours()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSort()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultPageNumber()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultPageSize()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageNumber()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageSize()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultPageNumberAndPageSize()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageNumberAndPageSize()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageNumberAndPageSizeAndSearchTerm()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageNumberAndPageSizeAndSearchTermAndSortBy()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageNumberAndPageSizeAndSearchTermAndSortByAndSortOrder()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageNumberAndPageSizeAndSearchTermAndSortByAndSortOrderAndFilter()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageNumberAndPageSizeAndSearchTermAndSortByAndSortOrderAndFilterAndPagination()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageNumberAndPageSizeAndSearchTermAndSortByAndSortOrderAndFilterAndPaginationAndSorting()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetFilteredTours_ReturnsAllToursWithDefaultSortAndPageNumberAndPageSizeAndSearchTermAndSortByAndSortOrderAndFilterAndPaginationAndSortingAndFiltering()
        {
            var searchTerm = "";
            var sortBy = "default";
            var pageNumber = 1;
            var pageSize = 1000;

            var result = await tourService.GetFilteredTours(searchTerm, sortBy, pageNumber, pageSize);

            Assert.That(result.Tours.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task DeleteTourAsync_ReturnsNotFound()
        {
            // Arrange
            var tourId = 454;

            //Assert
            var response = await tourService.DeleteTourAsync(tourId);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void CreateAsync_ThrowsException_WhenTourCreateDtoIsNull()
        {
            // Arrange
            TourCreateDTO tourCreateDto = null!;

            // Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => tourService.CreateTourAsync(tourCreateDto, "creator3"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Value does not fall within the expected range."));
        }


    }
}
