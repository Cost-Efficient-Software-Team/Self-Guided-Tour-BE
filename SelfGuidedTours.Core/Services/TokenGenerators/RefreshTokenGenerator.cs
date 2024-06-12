using Microsoft.Extensions.Configuration;

namespace SelfGuidedTours.Core.Services.TokenGenerators
{
    public class RefreshTokenGenerator
    {
        private readonly IConfiguration configuration;
        private readonly TokenGenerator tokenGenerator;

        public RefreshTokenGenerator(IConfiguration configuration,
            TokenGenerator tokenGenerator)
        {
            this.configuration = configuration;
            this.tokenGenerator = tokenGenerator;
        }
        public string GenerateToken()
        {
            var key = Environment.GetEnvironmentVariable("REFRESHTOKEN_KEY") ??
                throw new ApplicationException("REFRESHTOKEN_KEY is not configured.");

            var keyExpiration = Environment.GetEnvironmentVariable("REFRESHTOKEN_EXPIRATIONMINUTES") ??
                throw new ApplicationException("REFRESHTOKEN_EXPIRATIONMINUTES is not configured.");

            return tokenGenerator.GenerateToken(
                key,
                configuration["Authentication:Issuer"]!,
                configuration["Authentication:Audience"]!,
                double.Parse(keyExpiration));
        }
    }
}
