namespace SelfGuidedTours.Common.ValidationConstants
{
    public static class EmailValidationConstants
    {
        public const string EmailAddressErrorMessage = "Please provide a valid email address";

        public const int MaxSubjectLength = 100;
        public const string SubjectLengthErrorMessage = "Subject cannot be longer than 100 characters";

        public const int MaxBodyLength = 1000;
        public const string BodyLengthErrorMessage = "Body cannot be longer than 1000 characters";

    }
}
