using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Enums;
using SelfGuidedTours.Infrastructure.Data.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private IPaymentService paymentService;
        [SetUp]
        public async Task SetupAsync()
        {
            //setup db
            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                       .UseInMemoryDatabase("SelfGuidedToursInMemoryDb"
                           + Guid.NewGuid().ToString())
                       .Options;
            var dbContext = new SelfGuidedToursDbContext(dbContextOptions);

            IRepository repository = new Repository(dbContext);

            var mockedStripeClient = new Mock<IStripeClient>();
              
            var mockedLogger = new Mock<ILogger<PaymentService>>();
            paymentService = new PaymentService(repository,mockedStripeClient.Object,mockedLogger.Object);
            // Seed data
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "testuserId",
                    Email = "test@da.b",
                    UserName = "testuserCreator",
                    CreatedAt = DateTime.Now,
                    Name = "Test User",
                    PhoneNumber = "1234567890",
                },
                new ApplicationUser
                {
                    Id = "testuserId2",
                    Email = "test2@da.b",
                    UserName = "testuserCreator2",
                    CreatedAt = DateTime.Now,
                    Name = "Test User 2",
                    PhoneNumber = "1234567890",
                }
            };
            var tours = new List<Tour>
            {
                new Tour
                {
                    TourId = 1,
                    Title = "Test Tour",
                    ThumbnailImageUrl = "https://example.com/image.jpg",
                    Summary = "Test Summary",
                    Status = Status.UnderReview,
                    CreatedAt = DateTime.Now,
                    CreatorId = "testuserId",
                    Destination="test",
                    EstimatedDuration=134,
                    Price=100,
                },
                new Tour
                {
                    TourId = 2,
                    Title = "Test Tour 2",
                    ThumbnailImageUrl = "https://example.com/image2.jpg",
                    Summary = "Test Summary 2",
                    Status = Status.Approved,
                    CreatedAt = DateTime.Now,
                    CreatorId = "testuserId",
                    Destination="test",
                    EstimatedDuration=134,
                    Price=200,
                }
            };
            await dbContext.Users.AddRangeAsync(users);
            await dbContext.Tours.AddRangeAsync(tours);
            await dbContext.SaveChangesAsync();


        }
        [Test]
        public async Task MakePaymentAsync_ShouldReturnApiResponse_WhenCalled()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;

            // Act
            var response = await paymentService.MakePaymentAsync(userId, tourId);
            // Assert
            Assert.IsInstanceOf<ApiResponse>(response);
        }
    }
}
