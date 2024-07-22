using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.CustomExceptions;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Services;
using SelfGuidedTours.Core.Services.TokenGenerators;
using SelfGuidedTours.Core.Services.TokenValidators;
using SelfGuidedTours.Infrastructure.Common;
using SelfGuidedTours.Infrastructure.Data;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Net;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;

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
        public async Task SetupAsync()
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

            tokenGenerator = new TokenGenerator();
            accessTokenGenerator = new AccessTokenGenerator(tokenGenerator);
            refreshTokenGenerator = new RefreshTokenGenerator(tokenGenerator);
            refreshTokenValidator = new RefreshTokenValidator();
            refreshTokenService = new RefreshTokenService(repository);
            logger = new Logger<AuthService>(loggerFactory);

            // AuthService initialized
            service = new AuthService(repository, accessTokenGenerator,
                refreshTokenGenerator, refreshTokenValidator, refreshTokenService, userManager, logger);

            // Environment Variables
            Environment.SetEnvironmentVariable("ACCESSTOKEN_KEY", "4y7XS2AHicSOs2uUJCxwlHWqTJNExW3UDUjMeXi96uLEso1YV4RazqQubpFBdx0zZGtdxBelKURhh0WXxPR0mEJQHk_0U9HeYtqcMManhoP3X2Ge8jgxh6k4C_Gd4UPTc6lkx0Ca5eRE16ciFQ6wmYDnaXC8NbngGqartHccAxE");
            Environment.SetEnvironmentVariable("ACCESSTOKEN_EXPIRATIONMINUTES", "50");
            Environment.SetEnvironmentVariable("REFRESHTOKEN_KEY", "a7990245-e3f1-4dcd-95a4-c2cde60eb8df");
            Environment.SetEnvironmentVariable("REFRESHTOKEN_EXPIRATIONMINUTES", "131400");
            Environment.SetEnvironmentVariable("ISSUER", "https://localhost:7038");
            Environment.SetEnvironmentVariable("AUDIENCE", "https://localhost:7038");
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.DisposeAsync();
            loggerFactory.Dispose();
        }

        [Test]
        public async Task Test_GetByEmailAsync_ShouldWorkProperly()
        {
            var properResult = await service.GetByEmailAsync("user@selfguidedtours.bg");
            var nullResult = await service.GetByEmailAsync("wrongEmail@selfguidedtours.bg");

            Assert.Multiple(() =>
            {
                Assert.That(properResult, Is.Not.Null);
                Assert.That(nullResult, Is.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(properResult.Email, Is.EqualTo("user@selfguidedtours.bg"));
                Assert.That(properResult.Id, Is.EqualTo("e3bdf03e-ec2c-4b2d-be4b-a10d35b431c0"));
                Assert.That(properResult.Name, Is.EqualTo("User Userov"));
            });
        }

        [Test]
        public async Task Test_RegisterAsync_ShouldThrowAnExceptionIfUserWithProvidedEmailAlreadyExists()
        {
            RegisterInputModel model = new RegisterInputModel()
            {
                Email = "user@selfguidedtours.bg",
                Name = "New User",
                Password = "password123",
                RepeatPassword = "password123"
            };

            try
            {
                _ = await service.RegisterAsync(model);
            }
            catch (EmailAlreadyInUseException ex)
            {
                Assert.That(ex.Message, Is.EqualTo(UserWithEmailAlreadyExistsErrorMessage));
            }
        }

        [Test]
        public async Task Test_RegisterAsync_ShouldThrowAnExceptionIfPasswordsDoNotMatch()
        {
            RegisterInputModel model = new RegisterInputModel()
            {
                Email = "newUser@selfguidedtours.bg",
                Name = "New User",
                Password = "password123",
                RepeatPassword = "wrongPass"
            };

            try
            {
                _ = await service.RegisterAsync(model);
            }
            catch (ArgumentException aex)
            {
                Assert.That(aex.Message, Is.EqualTo("Passwords do not match!"));
            }
        }

        [Test]
        public async Task Test_RegisterAsync_ShouldWorkProperly()
        {
            RegisterInputModel model = new RegisterInputModel()
            {
                Email = "newUser@selfguidedtours.bg",
                Name = "New User",
                Password = "password123",
                RepeatPassword = "password123"
            };

            var result = await service.RegisterAsync(model);

            Assert.That(result.ResponseMessage, Is.EqualTo("User registered successfully!"));
        }

        [Test]
        public async Task Test_LoginAsync_ShouldThrowExceptionsProperly()
        {
            var invalidEmailModel = new LoginInputModel()
            {
                Email = "invalidEmail",
                Password = "pass123"
            };

            try
            {
                _ = await service.LoginAsync(invalidEmailModel);
            }
            catch (ArgumentException aex)
            {
                Assert.That(aex.Message, Is.EqualTo("Email or password is incorrect!"));
            }

            var invalidPassModel = new LoginInputModel()
            {
                Email = "user@selfguidedtours.bg",
                Password = "invalidPass"
            };

            try
            {
                _ = await service.LoginAsync(invalidPassModel);
            }
            catch (ArgumentException aex)
            {
                Assert.That(aex.Message, Is.EqualTo("Email or password is incorrect!"));
            }
        }

        [Test]
        public async Task Test_LoginAsync_ShouldWorkProperly()
        {
            var model = new LoginInputModel()
            {
                Email = "user@selfguidedtours.bg",
                Password = "D01Parola"
            };

            var result = await service.LoginAsync(model);

            Assert.That(result.ResponseMessage, Is.EqualTo("Successfully logged in!"));
        }


        [Test]
        public async Task Test_ChangePasswordAsync_ShouldWorkProperly()
        {
            ChangePasswordModel model = new ChangePasswordModel()
            {
                UserId = User.Id,
                CurrentPassword = "D01Parola",
                NewPassword = "NewPassword123"
            };

            var response = await service.ChangePasswordAsync(model);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Result, Is.EqualTo("Password changed successfully!"));
        }

        [Test]
        public async Task Test_ChangePasswordAsync_ShouldThrowExceptionIfPasswordsAreSame()
        {
            ChangePasswordModel model = new ChangePasswordModel()
            {
                UserId = User.Id,
                CurrentPassword = "D01Parola",
                NewPassword = "D01Parola"
            };

            try
            {
                _ = await service.ChangePasswordAsync(model);
            }
            catch (ArgumentException aex)
            {
                Assert.That(aex.Message, Is.EqualTo("New password can't be the same as the current one!"));
            }
        }

        [Test]
        public async Task Test_ChangePasswordAsync_ShouldThrowExceptionIfCurrentPasswordIsWrong()
        {
            ChangePasswordModel model = new ChangePasswordModel()
            {
                UserId = User.Id,
                CurrentPassword = "WrongPassword",
                NewPassword = "NewPassword123"
            };

            try
            {
                _ = await service.ChangePasswordAsync(model);
            }
            catch (UnauthorizedAccessException uex)
            {
                Assert.That(uex.Message, Is.EqualTo("Invalid password"));
            }
        }

        [Test]
        public async Task Test_LogoutAsync_ShouldWorkProperly()
        {
            var model = new LoginInputModel()
            {
                Email = "user@selfguidedtours.bg",
                Password = "D01Parola"
            };

            var loginResult = await service.LoginAsync(model);

            var refreshToken = await refreshTokenService.GetByTokenAsync(loginResult.RefreshToken);
            Assert.That(refreshToken, Is.Not.Null);

            await service.LogoutAsync(User.Id);

            var deletedRefreshToken = await refreshTokenService.GetByTokenAsync(loginResult.RefreshToken);
            Assert.That(deletedRefreshToken, Is.Null);
        }


        [Test]
        public async Task Test_RefreshAsync_ShouldWorkProperly()
        {
            var model = new LoginInputModel()
            {
                Email = "user@selfguidedtours.bg",
                Password = "D01Parola"
            };

            var loginResult = await service.LoginAsync(model);

            RefreshRequestModel refreshRequestModel = new RefreshRequestModel()
            {
                RefreshToken = loginResult.RefreshToken
            };

            var refreshResult = await service.RefreshAsync(refreshRequestModel);

            Assert.That(refreshResult.ResponseMessage, Is.EqualTo("Successfully got new tokens!"));
            Assert.That(refreshResult.AccessToken, Is.Not.Empty);
            Assert.That(refreshResult.RefreshToken, Is.Not.Empty);
        }

        [Test]
        public async Task Test_RefreshAsync_ShouldThrowExceptionIfRefreshTokenIsInvalid()
        {
            RefreshRequestModel model = new RefreshRequestModel()
            {
                RefreshToken = "InvalidRefreshToken"
            };

            try
            {
                _ = await service.RefreshAsync(model);
            }
            catch (ArgumentException aex)
            {
                Assert.That(aex.Message, Is.EqualTo("Invalid refresh token!"));
            }
        }


    }
}