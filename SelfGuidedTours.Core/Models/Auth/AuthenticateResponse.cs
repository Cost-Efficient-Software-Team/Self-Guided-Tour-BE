namespace SelfGuidedTours.Core.Models.Auth
{
    public class AuthenticateResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ResponseMessage { get; set; }
        public long AccessTokenExpiration { get; set; }
        public string Email { get; set; } // Добавено свойство
        public string EmailConfirmationToken { get; set; } // Добавено свойство
    }

}
