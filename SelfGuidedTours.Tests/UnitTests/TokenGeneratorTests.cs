﻿using Microsoft.IdentityModel.Tokens;
using SelfGuidedTours.Core.Services.TokenGenerators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class TokenGeneratorTests
    {
        private TokenGenerator tokenGenerator;

        [SetUp]
        public void Setup()
        {
            tokenGenerator = new TokenGenerator();
        }

        [Test]
        public void GenerateToken_ValidInputs_ReturnsToken()
        {
            // Arrange
            string secretKey = "supersecretkey";
            string issuer = "https://localhost:7038";
            string audience = "https://localhost:7038";
            double expirationMinutes = 50;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test.user@selfguidedtours.bg")
            };

            // Act
            var token = tokenGenerator.GenerateToken(secretKey, issuer, audience, expirationMinutes, claims);

            // Assert
            Assert.IsNotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            Assert.IsNotNull(validatedToken);
            Assert.AreEqual(2, principal.Claims.Count());
        }

        [Test]
        public void GenerateToken_NoClaims_ReturnsToken()
        {
            // Arrange
            string secretKey = "supersecretkey";
            string issuer = "https://localhost:7038";
            string audience = "https://localhost:7038";
            double expirationMinutes = 50;

            // Act
            var token = tokenGenerator.GenerateToken(secretKey, issuer, audience, expirationMinutes);

            // Assert
            Assert.IsNotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            Assert.IsNotNull(validatedToken);
        }
    }
}
