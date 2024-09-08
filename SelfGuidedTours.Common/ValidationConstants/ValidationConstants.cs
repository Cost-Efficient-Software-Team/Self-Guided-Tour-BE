namespace SelfGuidedTours.Common.ValidationConstants
{
    public static class ValidationConstants
    {
        public static class Tour
        {
            public const int TitleMinLength = 3;
            public const int TitleMaxLength = 50;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 500;

            public const int SummaryMinLength = 10;
            public const int SummaryMaxLength = 500;

            public const double PriceMinValue = 0.0;

            public const int ThumbnailImageUrlMaxLength = 500;

            public const int LocationMinLength = 3;
            public const int LocationMaxLength = 50;

            public const int DestinationMinLength = 3;
            public const int DestinationMaxLength = 50;

            public const int EstimatedDurationMinValueInMinutes = 15;
            public const int EstimatedDurationMaxValueInMinutes = 1440; //24 hours
        }

        public static class Landmark
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 500;

            //public const int CityMinLength = 2;
            public const int CityMaxLength = 500;

        }

        public static class Review
        {
            public const int CommentMinLength = 10;
            public const int CommentMaxLength = 500;
        }
        public static class LandmarkResource
        {
            public const int UrlMinLength = 10;
            public const int UrlMaxLength = 500;
        }
        public static class User
        {
            public const int NameMaxLenght = 50;

            public const int BioMaxLength = 500;
        }
    }
}
