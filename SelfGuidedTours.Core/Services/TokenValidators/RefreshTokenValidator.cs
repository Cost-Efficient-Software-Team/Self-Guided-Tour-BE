using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SelfGuidedTours.Core.Services.TokenValidators
{
    public class RefreshTokenValidator
    {
        public bool Validate(string refreshToken)
        {
            var key = Environment.GetEnvironmentVariable("REFRESHTOKEN_KEY") ??
                throw new ApplicationException("REFRESHTOKEN_KEY is not configured.");

            var issuer = Environment.GetEnvironmentVariable("ISSUER") ??
               throw new ApplicationException("ISSUER is not configured.");

            var audience = Environment.GetEnvironmentVariable("AUDIENCE") ??
               throw new ApplicationException("AUDIENCE is not configured.");

            TokenValidationParameters validationParameters = new TokenValidationParameters()
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

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToked);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
