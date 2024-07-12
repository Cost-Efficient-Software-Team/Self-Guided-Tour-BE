using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Core.Services.TokenGenerators;
using SelfGuidedTours.Core.Services.TokenValidators;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private SelfGuidedToursDbContext dbContext;
        private IRepository repository;
        private IAuthService service;

        private AccessTokenGenerator accessTokenGenerator;
        private RefreshTokenGenerator refreshTokenGenerator;
        private RefreshTokenValidator refreshTokenValidator;
        private IRefreshTokenService refreshTokenService;
        private ILogger<AuthService> logger;
        private TokenGenerator tokenGenerator;
        private ILoggerFactory loggerFactory;
        private UserManager<ApplicationUser> userManager;

        private IEnumerable<ApplicationUser> users;

        #region User and Admin
        
        private ApplicationUser User { get; set; }
        private ApplicationUser Admin { get; set; }
      
        #endregion

        [SetUp]
        public async Task SetupAsync(UserManager<ApplicationUser> appUser)
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            #region User and Admin initializing

            User = new ApplicationUser()
            {
                Id = "e3bdf03e-ec2c-4b2d-be4b-a10d35b431c0",
                Email = "user@selfguidedtours.bg",
                NormalizedEmail = "user@selfguidedtours.bg".ToUpper(),
                Name = "User Userov",
                NormalizedUserName = "User Userov".ToUpper(),
                PasswordHash = hasher.HashPassword(null!, "D01Parola")
            };

            Admin = new ApplicationUser()         
            {
                Id = "27d78708-8671-4b05-bd5e-17aa91392224",
                Email = "admin@selfguidedtours.bg",
                NormalizedEmail = "admin@selfguidedtours.bg".ToUpper(),
                Name = "Admin Adminov",
                NormalizedUserName = "Admin Adminov".ToUpper(),
                PasswordHash = hasher.HashPassword(null!, "D01Parola")
            };

            #endregion

            users = new List<ApplicationUser>() { User, Admin };

            var dbContextOptions = new DbContextOptionsBuilder<SelfGuidedToursDbContext>()
                        .UseInMemoryDatabase("SelfGuidedToursInMemoryDb"
                            + Guid.NewGuid().ToString())
                        .Options;

            dbContext = new SelfGuidedToursDbContext(dbContextOptions);

            await dbContext.AddRangeAsync(users);
            await dbContext.SaveChangesAsync();

            // Repository initialized
            repository = new Repository(dbContext);
            
            loggerFactory = new LoggerFactory();
            userManager = appUser;

            tokenGenerator = new TokenGenerator();
            accessTokenGenerator = new AccessTokenGenerator(tokenGenerator);
            refreshTokenGenerator = new RefreshTokenGenerator(tokenGenerator);
            refreshTokenValidator = new RefreshTokenValidator();
            refreshTokenService = new RefreshTokenService(repository);
            logger = new Logger<AuthService>(loggerFactory);

            // AuthService initialized
            service = new AuthService(repository, accessTokenGenerator, 
                refreshTokenGenerator, refreshTokenValidator, refreshTokenService, 
                userManager, logger);
        }
       
        [TearDown]
        public async Task TeardownAsync()
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.DisposeAsync();
            loggerFactory.Dispose();
            userManager.Dispose();
        }
    }
}
