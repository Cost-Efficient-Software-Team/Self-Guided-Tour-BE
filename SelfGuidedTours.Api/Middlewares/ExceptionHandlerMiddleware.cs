using System.Net;
using System.Text.Json;

namespace SelfGuidedTours.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> logger;
        private readonly RequestDelegate next;  // This calls the next middleware in the pipeline

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }
        //Impelement the InvokeAsync method by convention
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch(ArgumentException ex)
            {
                logger.LogError($"Invalid argument: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError($"Invalid operation: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError($"Unauthorized access: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.Unauthorized);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError($"Key not found: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound);
            }
            catch (TimeoutException ex)
            {
                logger.LogError($"Request timed out: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                logger.LogError($"Unexpected error: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError);
            }
        }
        /// <summary>
        /// Creates a response with the error details
        /// </summary>
        /// <param name="context"> HTTP Context</param>
        /// <param name="exception">The caught exception, that needs to be handled</param>
        /// <param name="statusCode">The HTTP status code that gets converted to int</param>
        private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                ErrorId = Guid.NewGuid(),
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            }.ToString());
        }

        /// <summary>
        /// A class that holds the error details
        /// </summary>
        private class ErrorDetails
        {
            public Guid ErrorId { get; set; }
            public int StatusCode { get; set; }
            public string Message { get; set; } = string.Empty;

            public override string ToString()
            {
                return JsonSerializer.Serialize(this);
            }
        }
    }
}
