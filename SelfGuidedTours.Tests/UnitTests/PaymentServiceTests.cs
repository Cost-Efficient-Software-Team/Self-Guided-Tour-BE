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
        private  IRepository repository;
        [SetUp]
        public async Task SetupAsync()
        {
            //setup db
            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                       .UseInMemoryDatabase("SelfGuidedToursInMemoryDb"
                           + Guid.NewGuid().ToString())
                       .Options;
            var dbContext = new SelfGuidedToursDbContext(dbContextOptions);

            repository = new Repository(dbContext);

            var testCustomer = new Customer
            {
                Id = "cus_test123",
                Email = "test@example.com",
                Name = "Test Customer"
            };



            // Mock dependencies
            var mockedStripeClient = new Mock<IStripeClient>();
            var mockedPaymentIntentService = new Mock<PaymentIntentService>();
            var mockedCustomerService = new Mock<CustomerService>(MockBehavior.Strict);
            // Mock CreateAsync method to always return a customer
            mockedCustomerService.Setup(x => x.CreateAsync(It.IsAny<CustomerCreateOptions>(),null,It.IsAny<CancellationToken>())).ReturnsAsync(testCustomer);
            mockedPaymentIntentService.Setup(service => service.CreateAsync(It.IsAny<PaymentIntentCreateOptions>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentIntent() { Id = "pi_test123" });

            var mockedLogger = new Mock<ILogger<PaymentService>>();

            paymentService = new PaymentService(repository, mockedStripeClient.Object, mockedLogger.Object,
                mockedCustomerService.Object, mockedPaymentIntentService.Object);
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
                },
                new Tour
                {
                    TourId = 3,
                    Title = "Test Tour 3",
                    ThumbnailImageUrl = "https://example.com/image3.jpg",
                    Summary = "Test Summary 3",
                    Status = Status.Approved,
                    CreatedAt = DateTime.Now,
                    CreatorId = "testuserId",
                    Destination="test",
                    EstimatedDuration=134,
                },
                new Tour
                {
                    TourId = 4,
                    Title = "Test Tour 4",
                    ThumbnailImageUrl = "https://example.com/image4.jpg",
                    Summary = "Test Summary 4",
                    Status = Status.Approved,
                    CreatedAt = DateTime.Now,
                    CreatorId = "testuserId",
                    Destination="test",
                    EstimatedDuration=134,
                    Price=0,
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
            Assert.That(response, Is.InstanceOf<ApiResponse>());
        }
        [Test]
        public async Task MakePaymentAsync_ShouldReturnApiResponseWithSuccessTrue_WhenCalled()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            // Act
            var response = await paymentService.MakePaymentAsync(userId, tourId);
            // Assert
            Assert.That(response.IsSuccess, Is.True);
        }
        [Test]
        public  void MakePaymentAsync_ShouldReturnUserNotFountMessage_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "testuserId3";
            var tourId = 1;
            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(()=>paymentService.MakePaymentAsync(userId,tourId));
            
            // Assert
            Assert.That(ex.Message, Is.EqualTo("User not found!"));
        }

        [Test]
        public void MakePaymentAsync_ShouldReturnTourNotFoundMessage_WhenTourDoesNotExist()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 332;
            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => paymentService.MakePaymentAsync(userId, tourId));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Tour not found!"));
        }
        [Test]
        public void MakePaymentAsync_ShouldReturnTourPriceNotSetMessage_WhenTourPriceIsNotSet()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 3; // tour with no price
            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => paymentService.MakePaymentAsync(userId, tourId));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Tour price is not set!"));
        }
        [Test]
        public async Task MakePaymentAsync_ShouldCreatePaymentIntent_WhenCalled()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            // Act
            var response = await paymentService.MakePaymentAsync(userId, tourId);
            // Assert
            Assert.That(response.IsSuccess, Is.True);
        }

        [Test]
        public async Task MakePaymentAsync_ShouldCreatePayment()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            var paymentsCount = await repository.All<Payment>().CountAsync();
            // Act
            var response = await paymentService.MakePaymentAsync(userId, tourId);
            var newPaymentsCount = await repository.All<Payment>().CountAsync();
            // Assert
            Assert.That(newPaymentsCount, Is.EqualTo(paymentsCount+1));
        }

        [Test]
        public async Task MakePaymentAsync_ShouldCreatePaymentWithStatusPending()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            // Act
            var response = await paymentService.MakePaymentAsync(userId, tourId);
            var payment = await repository.All<Payment>()
                .FirstOrDefaultAsync(p=>p.UserId==userId 
                                       && p.TourId == tourId);
            // Assert
            Assert.That(payment!.Status, Is.EqualTo(PaymentStatus.Pending));
        }
        [Test]
        public async Task MakePaymentAsync_ShouldCreatePaymentIntentWithAmount()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;

            // Act
            var tour = await repository.All<Tour>().FirstOrDefaultAsync(t=>t.TourId == tourId);
            var response = await paymentService.MakePaymentAsync(userId, tourId);
            var paymentResult = response.Result as PaymentResult;

            // Assert
            Assert.That(paymentResult!.Amount, Is.EqualTo(tour!.Price));
        }
        [Test]
        public void FinalizePaymentAsync_ShouldReturnPaymentNotFound()
        {
            // Arrange
            var paymentIntentId = "wrong payment id";
            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => paymentService.FinalizePaymentAsync(paymentIntentId));
            // Assert
            Assert.That(ex.Message, Is.EqualTo("Payment not found!"));
        }
        [Test]
        public async Task FinalizePaymentAsync_ShouldReturnTourAlreadyPurchased()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            var payment = new Payment
            {
                PaymentId = 123,
                UserId = userId,
                TourId = tourId,
                PaymentIntentId = "pi_test123",
                Status = PaymentStatus.Succeeded,
                PaymentDate = DateTime.Now,
            };
            await repository.AddAsync(payment);
            await repository.SaveChangesAsync();
            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => paymentService.FinalizePaymentAsync("pi_test123"));
            // Assert
            Assert.That(ex.Message, Is.EqualTo("Tour already purchased!"));
        }
        [Test]
        public async Task FinalizePaymentAsync_ShouldReturnApiResponse()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            var payment = new Payment
            {
                PaymentId = 123,
                UserId = userId,
                TourId = tourId,
                PaymentIntentId = "pi_test123",
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.Now,
            };
            await repository.AddAsync(payment);
            await repository.SaveChangesAsync();
            // Act
            var response = await paymentService.FinalizePaymentAsync("pi_test123");
            // Assert
            Assert.That(response, Is.InstanceOf<ApiResponse>());
        }
        [Test]
        public async Task FinalizePaymentAsync_ShouldReturnApiResponseWithSuccessTrue()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            var payment = new Payment
            {
                PaymentId = 123,
                UserId = userId,
                TourId = tourId,
                PaymentIntentId = "pi_test123",
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.Now,
            };
            await repository.AddAsync(payment);
            await repository.SaveChangesAsync();
            // Act
            var response = await paymentService.FinalizePaymentAsync("pi_test123");
            // Assert
            Assert.That(response.IsSuccess, Is.True);
        }
        [Test]
        public async Task FinalizePaymentAsync_ShouldReturnPaymentMarkedAsSucceeded()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            var payment = new Payment
            {
                PaymentId = 123,
                UserId = userId,
                TourId = tourId,
                PaymentIntentId = "pi_test123",
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.Now,
            };
            await repository.AddAsync(payment);
            await repository.SaveChangesAsync();
            // Act
            var response = await paymentService.FinalizePaymentAsync("pi_test123");
            var updatedPayment = await repository.All<Payment>().FirstOrDefaultAsync(p => p.PaymentIntentId == "pi_test123");
            // Assert
            Assert.That(updatedPayment!.Status, Is.EqualTo(PaymentStatus.Succeeded));
        }
        [Test]
        public async Task FinalizePaymentAsync_ShouldCreateUserTour()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            var payment = new Payment
            {
                PaymentId = 123,
                UserId = userId,
                TourId = tourId,
                PaymentIntentId = "pi_test123",
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.Now,
            };
            await repository.AddAsync(payment);
            await repository.SaveChangesAsync();
            // Act
            var response = await paymentService.FinalizePaymentAsync("pi_test123");
            var userTour = await repository.All<UserTours>().FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TourId == tourId);
            // Assert
            Assert.That(userTour, Is.Not.Null);
        }
        [Test]
        public async Task CreateOrGetCustomerAsync_ShouldReturnUserNotFound()
        {
            // Arrange
            var userId = "wrongUserId";
            // Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => paymentService.CreateOrGetCustomerAsync(userId));
            // Assert
            Assert.That(ex.Message, Is.EqualTo("User not found!"));
        }
        [Test]
        public async Task CreateOrGetCustomerAsync_ShouldReturnCustomer()
        {
            // Arrange
            var userId = "testuserId";
            // Act
            var customer = await paymentService.CreateOrGetCustomerAsync(userId);
            // Assert
            Assert.That(customer, Is.Not.Null);
        }
        [Test]
        public async Task CreateOrGetCustomerAsync_ShouldAddCustomerIdToUser()
        {
            // Arrange
            var userId = "testuserId";
            // Act
            var customer = await paymentService.CreateOrGetCustomerAsync(userId);
            var user = await repository.All<ApplicationUser>().FirstOrDefaultAsync(u => u.Id == userId);
            // Assert
            Assert.That(user!.StripeCustomerId, Is.Not.Null);
        }
        [Test]
        public async Task CreateOrGetCustomerAsync_ShouldReturnAlreadyExistinCustomerId()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "testuserIdWithCUstomerId",
                Email = "test@da.b",
                UserName = "testuserCreator",
                CreatedAt = DateTime.Now,
                Name = "Test User",
                PhoneNumber = "1234567890",
                StripeCustomerId = "cus_test123"
            };
            await repository.AddAsync(user);
            await repository.SaveChangesAsync();

            //Act
            var customerId = await paymentService.CreateOrGetCustomerAsync(user.Id);

            //Assert
            Assert.That(customerId, Is.EqualTo(user.StripeCustomerId));
        }
        [Test]
        public void AddFreeTour_ShouldReturnTourIsNotFree()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 1;
            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => paymentService.AddFreeTour(userId, tourId));
            // Assert
            Assert.That(ex.Message, Is.EqualTo("Tour is not free!"));
        }
        [Test]
        public void AddFreeTour_ShouldReturnTourNotFound()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 123;
            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => paymentService.AddFreeTour(userId, tourId));
            // Assert
            Assert.That(ex.Message, Is.EqualTo("Tour not found!"));
        }
        [Test]
        public async Task AddFreeTour_ShouldReturnApiResponse()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 4;
            // Act
            var response = await paymentService.AddFreeTour(userId, tourId);
            // Assert
            Assert.That(response, Is.InstanceOf<ApiResponse>());
        }
        [Test]
        public async Task AddFreeTour_ShouldReturnApiResponseWithSuccessTrue()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 4;
            // Act
            var response = await paymentService.AddFreeTour(userId, tourId);
            // Assert
            Assert.That(response.IsSuccess, Is.True);
        }
        [Test]
        public async Task AddFreeTour_ShouldCreateUserTour()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 4;
            // Act
            var response = await paymentService.AddFreeTour(userId, tourId);
            var userTour = await repository.All<UserTours>().FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TourId == tourId);
            // Assert
            Assert.That(userTour, Is.Not.Null);
        }
        [Test]
        public async Task AddFreeTour_ShouldReturnUserTourCreatedSuccessfully()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 4;
            // Act
            var response = await paymentService.AddFreeTour(userId, tourId);
            // Assert
            Assert.That(response.Result, Is.EqualTo("Tour successfully added to collection!" ));
        }
        [Test]
        public async Task AddFreeTour_ShouldReturnUserTourCreatedSuccessfullyWithUserTourId()
        {
            // Arrange
            var userId = "testuserId";
            var tourId = 4;
            // Act
            var response = await paymentService.AddFreeTour(userId, tourId);
            var userTour = await repository.All<UserTours>().FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TourId == tourId);
            // Assert
            Assert.That(response.Result, Is.EqualTo($"Tour successfully added to collection!"));
        }

    }
}
