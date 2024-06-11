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
            //var key = Environment.GetEnvironmentVariable("RefreshTokenSecret") ??
            //    throw new ApplicationException("Refresh token is not configured.");

            return tokenGenerator.GenerateToken(
                configuration["Authentication:RefreshTokenSecret"]!,
                configuration["Authentication:Issuer"]!,
                configuration["Authentication:Audience"]!,
                double.Parse(configuration["Authentication:RefreshTokenExpirationMinutes"]!));
        }
    }
}
