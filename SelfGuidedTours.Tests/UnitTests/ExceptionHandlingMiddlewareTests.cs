using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SelfGuidedTours.Api.Middlewares;
using SelfGuidedTours.Core.CustomExceptions;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class ExceptionHandlingMiddlewareTests
    {
        private ILoggerFactory loggerFactory;
        private ILogger<ExceptionHandlerMiddleware> logger;
        private DefaultHttpContext context;
        [SetUp]
        public void Setup()
        {
            context = new DefaultHttpContext();
            loggerFactory = new LoggerFactory();
            logger = new Logger<ExceptionHandlerMiddleware>(loggerFactory);
        }
        [TearDown]
        public void TearDown()
        {
            loggerFactory.Dispose();
        }

        [Test]
        public async Task ExceptionHandlerMiddleware_ShouldReturnBadRequest()
        {
            // Arrange
            RequestDelegate next = (HttpContext context) => throw new ArgumentException();
            var middleware = new ExceptionHandlerMiddleware(logger, next);
            context.Response.Body = new MemoryStream();
            context.Response.StatusCode = 0;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(400));
        }
        [Test]
        public async Task ExceptionHandlerMiddleware_ShouldReturnUnauthorized()
        {
            // Arrange
            RequestDelegate next = (HttpContext context) => throw new UnauthorizedAccessException();
            var middleware = new ExceptionHandlerMiddleware(logger, next);
            context.Response.Body = new MemoryStream();
            context.Response.StatusCode = 0;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(401));
        }
        [Test]
        public async Task ExceptionHandlerMiddleware_ShouldReturnNotFound()
        {
            // Arrange
            RequestDelegate next = (HttpContext context) => throw new KeyNotFoundException();
            var middleware = new ExceptionHandlerMiddleware(logger, next);
            context.Response.Body = new MemoryStream();
            context.Response.StatusCode = 0;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(404));
        }
        [Test]
        public async Task ExceptionHandlerMiddleware_ShouldReturnRequestTimeout()
        {
            // Arrange
            RequestDelegate next = (HttpContext context) => throw new TimeoutException();
            var middleware = new ExceptionHandlerMiddleware(logger, next);
            context.Response.Body = new MemoryStream();
            context.Response.StatusCode = 0;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(408));
        }
        [Test]
        public async Task ExceptionHandlerMiddleware_ShouldReturnInternalServerError()
        {
            // Arrange
            RequestDelegate next = (HttpContext context) => throw new Exception();
            var middleware = new ExceptionHandlerMiddleware(logger, next);
            context.Response.Body = new MemoryStream();
            context.Response.StatusCode = 0;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(500));
        }
        [Test]
        public async Task ExceptionHandlerMiddleware_ShouldReturnBadRequestWithErrorMessage()
        {
            // Arrange
            RequestDelegate next = (HttpContext context) => throw new EmailAlreadyInUseException();
            var middleware = new ExceptionHandlerMiddleware(logger, next);
            context.Response.Body = new MemoryStream();
            context.Response.StatusCode = 0;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo(400));
            Assert.That(context.Response.Body.Length, Is.GreaterThan(0));
        }
    }
 }
