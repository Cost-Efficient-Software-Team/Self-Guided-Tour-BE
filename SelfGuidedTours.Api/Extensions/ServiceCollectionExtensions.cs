using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Core.Services.TokenGenerators;
using SelfGuidedTours.Core.Services.TokenValidators;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            //Inject services here
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();

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
                   .WithOrigins("http://localhost:3000") // This is the Client app URL TODO: Change this after FE deployment
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
