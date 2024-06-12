﻿namespace SelfGuidedTours.Core.Models.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = null!;
    }
}
