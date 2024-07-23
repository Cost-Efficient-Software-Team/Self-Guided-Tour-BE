using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using SelfGuidedTours.Api.Extensions;
using SelfGuidedTours.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomizedControllers(); // This replaces the AddControllers method, comes from ServiceCollectionExtensions.cs

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