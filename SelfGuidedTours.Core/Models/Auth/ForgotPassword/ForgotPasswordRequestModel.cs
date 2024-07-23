using System;
namespace SelfGuidedTours.Core.Models.Auth.ResetPassword
{
    public class ForgotPasswordRequestModel
    {
        public string Email { get; set; } = string.Empty;
    }
}