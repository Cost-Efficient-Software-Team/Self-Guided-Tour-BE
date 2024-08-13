using Microsoft.IdentityModel.Tokens;
using SelfGuidedTours.Core.Services.TokenValidators;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class RefreshTokenValidatorTests
    {
        private RefreshTokenValidator validator;

        [SetUp]
        public void Setup()
        {
            validator = new RefreshTokenValidator();

            // Updated key to be 256 bits (32 characters)
            Environment.SetEnvironmentVariable("REFRESHTOKEN_KEY", "test_refresh_token_key_123456789012");
            Environment.SetEnvironmentVariable("ISSUER", "https://localhost:7038");
            Environment.SetEnvironmentVariable("AUDIENCE", "https://localhost:7038");
        }

        [Test]
        public void Validate_ValidToken_ReturnsTrue()
        {
            // Arrange
            var key = Environment.GetEnvironmentVariable("REFRESHTOKEN_KEY");
            var issuer = Environment.GetEnvironmentVariable("ISSUER");
            var audience = Environment.GetEnvironmentVariable("AUDIENCE");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            // Act
            var result = validator.Validate(tokenString);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Validate_InvalidToken_ReturnsFalse()
        {
            // Arrange
            var invalidToken = "invalid_token";

            // Act
            var result = validator.Validate(invalidToken);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Validate_MissingEnvironmentVariable_ThrowsException()
        {
            // Arrange
            Environment.SetEnvironmentVariable("REFRESHTOKEN_KEY", null);

            // Act & Assert
            var ex = Assert.Throws<ApplicationException>(() => validator.Validate("some_token"));
            Assert.That(ex.Message, Is.EqualTo("REFRESHTOKEN_KEY is not configured."));
        }
    }
}
