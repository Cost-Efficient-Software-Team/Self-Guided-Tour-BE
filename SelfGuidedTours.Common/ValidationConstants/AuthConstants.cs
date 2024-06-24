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

            public const string InvalidPasswordMessage = "Password must contain at least 6 characters, one uppercase letter, one lowercase letter, one digit and one special character!";

            public const string PasswordsDoNotMatchMessage = "Passwords do not match!";

            public const string EmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$";

            public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{6,}$";

        }
    }
}
