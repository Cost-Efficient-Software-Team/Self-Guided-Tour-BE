namespace SelfGuidedTours.Core.Models.Auth
{
    public class AuthenticateResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = null!;
    }
}
