using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SelfGuidedTours.Core.Services.TokenValidators
{
    public class RefreshTokenValidator
    {
        private readonly IConfiguration config;

        public RefreshTokenValidator(IConfiguration config)
        {
            this.config = config;
        }

        public bool Validate(string refreshToken)
        {
            var key = Environment.GetEnvironmentVariable("REFRESHTOKEN_KEY") ??
                throw new ApplicationException("REFRESHTOKEN_KEY is not configured.");

            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Authentication:Issuer"], // Issuer from user secrets
                ValidAudience = config["Authentication:Audience"], // Audience from user secrets
                IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(key)), // Key from user secrets
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
