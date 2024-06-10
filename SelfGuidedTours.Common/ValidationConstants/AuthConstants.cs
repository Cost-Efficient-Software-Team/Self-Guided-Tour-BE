namespace SelfGuidedTours.Common.ValidationConstants
{
    public static class AuthConstants
    {
        public const string RequiredMessage = "{0} field is required!";
        public const string LengthErrorMessage = "{0} must be at least {1} characters long!";
        public const string InvalidEmailAddressMessage = "Invalid Email Address!";
        public static class Register
        {
            public const int PasswordMinLength = 6;

            public const string PasswordsDoNotMatchMessage = "Passwords do not match!";
        }
    }
}
