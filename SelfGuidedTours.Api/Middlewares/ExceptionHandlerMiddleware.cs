using SelfGuidedTours.Core.Models.ErrorResponse;
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
                string errorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1";
                logger.LogError($"Invalid argument: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest, errorType);
            }
            catch (InvalidOperationException ex)
            {
                string errorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1";
                logger.LogError($"Invalid operation: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest, errorType);
            }
            catch (UnauthorizedAccessException ex)
            {
                string errorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2";
                logger.LogError($"Unauthorized access: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.Unauthorized, errorType);
            }
            catch (KeyNotFoundException ex)
            {
                string errorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5";
                logger.LogError($"Key not found: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound, errorType);
            }
            catch (TimeoutException ex)
            {
                string errorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.9";
                logger.LogError($"Request timed out: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.RequestTimeout, errorType);
            }
            catch (Exception ex)
            {
                string errorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";
                logger.LogError($"Unexpected error: {ex} ");
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, errorType);
            }
        }
        /// <summary>
        /// Creates a response with the error details
        /// </summary>
        /// <param name="context"> HTTP Context</param>
        /// <param name="exception">The caught exception, that needs to be handled</param>
        /// <param name="statusCode">The HTTP status code that gets converted to int</param>
        /// <param name="errorType">The type of the error and a link to documentation about it, following REST principals</param>
        private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, string errorType)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(new ErrorDetails()
            {
                ErrorId = Guid.NewGuid(),
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Type = errorType,
                Errors = { { "Error", exception.Message } }
            }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(json);
        }

       
        
    }
}
