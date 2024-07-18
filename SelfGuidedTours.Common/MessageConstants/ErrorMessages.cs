namespace SelfGuidedTours.Common.MessageConstants
{
    public static class ErrorMessages
    {
        public const string LengthErrorMessage = "{0} must be between {2} and {1} characters long!";

        public const string CommentLengthErrorMessage = "{0} should not be more than {1} characters long!";

        public const string UrlLengthErrorMessage = "{0} should not be more than {1} characters long!";

        public const string BlobStorageErrorMessage = "Error uploading file";

        public const string ContainerNameErrorMessage = "Blob Storage name is not configured";

        public const string InvalidFileErrorMessage = "Invalid file type";

        public const string InvalidFileExtensionErrorMessage = "Invalid file extension";

        public const string InvalidFileSizeErrorMessage = "File size should not exceed {0} MB";

        public const string TourNotFoundErrorMessage = "Tour not found";

        public const string TourWithNoLandmarksErrorMessage = "Tour must have at least one landmark";

        public const string TourAlreadyApprovedErrorMessage = "Tour is already approved";

        public const string TourAlreadyRejectedErrorMessage = "Tour is already rejected";

        public const string UserWithEmailAlreadyExistsErrorMessage = "User with this email already exists!";
    }
}
