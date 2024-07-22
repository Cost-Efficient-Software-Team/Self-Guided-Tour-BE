using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.ErrorResponse;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Core.Services.BlobStorage;
using SelfGuidedTours.Core.Services.TokenGenerators;
using SelfGuidedTours.Core.Services.TokenValidators;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data.Models;
using SelfGuidedTours.Infrastructure.Data;
using System.Text;
using System.Text.Json.Serialization;

namespace SelfGuidedTours.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomizedControllers(this IServiceCollection services)
        {
            services.AddControllers()
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

            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            //Inject services here
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<ITourService, TourService>();
            services.AddScoped<ILandmarkService, LandmarkService>();
            services.AddScoped<ILandmarkResourceService, LandmarkResourceService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBlobService, BlobService>();


            //Token generators
            services.AddScoped<AccessTokenGenerator>();
            services.AddScoped<RefreshTokenGenerator>();
            services.AddScoped<TokenGenerator>();
            services.AddScoped<RefreshTokenValidator>();

            services.AddCors(Options =>
            {
                Options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                    .WithOrigins("http://localhost:3000", "https://self-guided-tour-fe.vercel.app/") // This will work for the local enviroment and with the current deployment URL
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader();
                });
            });

            return services;
        }
        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection"); //Connection string from user secrets
            services.AddDbContext<SelfGuidedToursDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            var storageConnectionString = config.GetConnectionString("BlobStorage");

            services.AddAzureClients(azureBuilder =>
            {
                azureBuilder.AddBlobServiceClient(storageConnectionString);
            });

            services.AddScoped<IRepository, Repository>();

            return services;
        }
        public static IServiceCollection AddApplicationIdentity(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("SelfGuidedTours")
                .AddEntityFrameworkStores<SelfGuidedToursDbContext>()
                .AddDefaultTokenProviders();


            //Password requirements
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var key = Environment.GetEnvironmentVariable("ACCESSTOKEN_KEY") ??
                         throw new ApplicationException("ACCESSTOKEN_KEY is not configured.");

                    var issuer = Environment.GetEnvironmentVariable("ISSUER") ??
                         throw new ApplicationException("ISSUER is not configured.");

                    var audience = Environment.GetEnvironmentVariable("AUDIENCE") ??
                         throw new ApplicationException("AUDIENCE is not configured.");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(key)),
                        ClockSkew = TimeSpan.Zero
                    };
                });


            return services;
        }
    }
}