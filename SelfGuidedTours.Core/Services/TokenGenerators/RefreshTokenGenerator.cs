namespace SelfGuidedTours.Core.Services.TokenGenerators
{
    public class RefreshTokenGenerator
    {
        private readonly TokenGenerator tokenGenerator;

        public RefreshTokenGenerator(TokenGenerator tokenGenerator)
        {
            this.tokenGenerator = tokenGenerator;
        }
        public string GenerateToken()
        {
            var key = Environment.GetEnvironmentVariable("REFRESHTOKEN_KEY") ??
                throw new ApplicationException("REFRESHTOKEN_KEY is not configured.");

            var keyExpiration = Environment.GetEnvironmentVariable("REFRESHTOKEN_EXPIRATIONMINUTES") ??
                throw new ApplicationException("REFRESHTOKEN_EXPIRATIONMINUTES is not configured.");

            var issuer = Environment.GetEnvironmentVariable("ISSUER") ??
               throw new ApplicationException("ISSUER is not configured.");

            var audience = Environment.GetEnvironmentVariable("AUDIENCE") ??
               throw new ApplicationException("AUDIENCE is not configured.");

            return tokenGenerator.GenerateToken(
                key,
                issuer,
                audience,
                double.Parse(keyExpiration));
        }
    }
}
