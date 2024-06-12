using Microsoft.Extensions.Configuration;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.Security.Claims;

namespace SelfGuidedTours.Core.Services.TokenGenerators
{
    public class AccessTokenGenerator
    {
        private readonly IConfiguration configuration;
        private readonly TokenGenerator tokenGenerator;

        public AccessTokenGenerator(IConfiguration configuration,
            TokenGenerator tokenGenerator)
        {
            this.configuration = configuration;
            this.tokenGenerator = tokenGenerator;
        }

        public string GenerateToken(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var key = Environment.GetEnvironmentVariable("ACCESSTOKEN_KEY") ??
               throw new ApplicationException("ACCESSTOKEN_KEY is not configured.");
            
            var keyExpiration = Environment.GetEnvironmentVariable("ACCESSTOKEN_EXPIRATIONMINUTES") ??
               throw new ApplicationException("ACCESSTOKEN_EXPIRATIONMINUTES is not configured.");

            return tokenGenerator.GenerateToken(
                key,
                configuration["Authentication:Issuer"]!,
                configuration["Authentication:Audience"]!,
                double.Parse(keyExpiration),
                claims);
        }
    }
}
