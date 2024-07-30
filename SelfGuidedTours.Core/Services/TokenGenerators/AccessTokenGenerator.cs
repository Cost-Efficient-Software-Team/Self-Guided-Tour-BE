using SelfGuidedTours.Infrastructure.Data.Models;
using System.Security.Claims;

namespace SelfGuidedTours.Core.Services.TokenGenerators
{
    public class AccessTokenGenerator
    {
        private readonly TokenGenerator tokenGenerator;

        public AccessTokenGenerator(TokenGenerator tokenGenerator)
        {
            this.tokenGenerator = tokenGenerator;
        }

        public string GenerateToken(ApplicationUser user,string role)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, role)
            };

            var key = Environment.GetEnvironmentVariable("ACCESSTOKEN_KEY") ??
               throw new ApplicationException("ACCESSTOKEN_KEY is not configured.");
            
            var keyExpiration = Environment.GetEnvironmentVariable("ACCESSTOKEN_EXPIRATIONMINUTES") ??
               throw new ApplicationException("ACCESSTOKEN_EXPIRATIONMINUTES is not configured.");
           
            var issuer = Environment.GetEnvironmentVariable("ISSUER") ??
               throw new ApplicationException("ISSUER is not configured.");

            var audience = Environment.GetEnvironmentVariable("AUDIENCE") ??
               throw new ApplicationException("AUDIENCE is not configured.");

            return tokenGenerator.GenerateToken(
                key,
                issuer,
                audience,
                double.Parse(keyExpiration),
                claims);
        }
    }
}
