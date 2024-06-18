using SelfGuidedTours.Core.Contracts;
using SelfGuidedTours.Core.Models.Auth;
using SelfGuidedTours.Core.Models.ExternalLogin;
using System.Net.Http.Headers;
using System.Text.Json;
namespace SelfGuidedTours.Core.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IAuthService authService;

        public GoogleAuthService(IAuthService authService)
        {
            this.authService = authService;
        }
        public async Task<AuthenticateResponse> GoogleSignIn(GoogleSignInVM model)
        {
            var payload = await ValidateGoogleIdToken(model.IdToken);
            if (payload == null)
            {
                throw new Exception("Invalid Google Id Token");
            }
            var googleUser = JsonSerializer.Deserialize<GoogleUserDto>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (googleUser == null)
            {
                throw new Exception("Invalid Google Id Token");
            }

            var userResponse = await authService.GoogleSignInAsync(googleUser);

            return userResponse;
        }
        private async Task<string?> ValidateGoogleIdToken(string idToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var userInfo = await response.Content.ReadAsStringAsync();

            return userInfo;
        }
    }
}
