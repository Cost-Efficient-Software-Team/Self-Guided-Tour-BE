using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SelfGuidedTours.Api.Extensions;
using SelfGuidedTours.Api.Middlewares;
using SelfGuidedTours.Core.Models.ErrorResponse;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .ConfigureApiBehaviorOptions(options =>  //Customize the response for invalid model state, by overriding the default behavior
    {
        options.InvalidModelStateResponseFactory = ContextBoundObject =>
        {
            var errors = ContextBoundObject.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors[0].ErrorMessage
                  );
            var errorResponse = new ErrorDetails
            {
                ErrorId = Guid.NewGuid(),
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "One or more validation errors occured",
                Errors = errors,
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1"
            };
            

            return new BadRequestObjectResult(errorResponse)
            {
                ContentTypes = { "application/json" },
            };
        };
    });


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationDbContext(builder.Configuration);

builder.Services.AddApplicationIdentity(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();


//Apply swagger middleware on every enviroment
app.UseSwagger();
app.UseSwaggerUI();


// Add custom middleware for exception handling to the pipeline
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors("CorsPolicy"); // apply CORS policy from ServiceCollectionExtensions.cs
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();