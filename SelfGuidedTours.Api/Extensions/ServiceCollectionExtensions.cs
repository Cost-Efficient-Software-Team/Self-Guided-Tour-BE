using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using SelfGuidedTours.Api.Filters;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Contracts.BlobStorage;
using SelfGuidedTours.Core.Models.ErrorResponse;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Core.Services.BlobStorage;
using SelfGuidedTours.Core.Services.TokenGenerators;
using SelfGuidedTours.Core.Services.TokenValidators;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;
using Stripe;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Text.Json.Serialization;

namespace SelfGuidedTours.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures and adds controllers with custom settings.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>IMvcBuilder for further configuration.</returns>
        public static IMvcBuilder AddCustomizedControllers(this IServiceCollection services)
        {
            return services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Ignore cyclic references to prevent serialization issues
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    // Add converter to serialize enums as strings
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options =>  // Customize the response for invalid model state
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value?.Errors[0].ErrorMessage
                              );
                        var errorResponse = new ErrorDetails
                        {
                            ErrorId = Guid.NewGuid(),
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = "One or more validation errors occurred",
                            Errors = errors,
                            Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1"
                        };

                        return new BadRequestObjectResult(errorResponse)
                        {
                            ContentTypes = { "application/json" },
                        };
                    };
                });
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Inject services here
            services.AddHttpContextAccessor();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<ITourService, TourService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<ILandmarkService, LandmarkService>();
            services.AddScoped<ILandmarkResourceService, LandmarkResourceService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IReviewService, Core.Services.ReviewService>();
            services.AddTransient<ISchemaFilter, EnumSchemaFilter>();
            // Token generators
            services.AddScoped<AccessTokenGenerator>();
            services.AddScoped<RefreshTokenGenerator>();
            services.AddScoped<TokenGenerator>();
            services.AddScoped<RefreshTokenValidator>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            // Setup Stripe
            var stripeKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
                        ?? config.GetValue<string>("StripeSettings:SecretKey")
                        ?? throw new ApplicationException("Stripe ENV variables are not configured.");
            services.AddSingleton<IStripeClient>(new StripeClient(stripeKey));

            // Add Stripe customer service in DI container
            services.AddScoped(provider =>
            {
                var stripeClient = provider.GetRequiredService<IStripeClient>();
                return new CustomerService(stripeClient);
            });

            // Add Stripe payment intent service in DI container
            services.AddScoped(provider =>
            {
                var stripeClient = provider.GetRequiredService<IStripeClient>();
                return new PaymentIntentService(stripeClient);
            });

            return services;
        }

        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection"); // Connection string from user secrets

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

            // Password requirements
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedEmail = true;
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