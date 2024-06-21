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

            public const decimal PriceMinValue = 1.0m;

            public const int ThumbnailImageUrlMaxLength = 500;
        }

        public static class Landmark
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;

            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 500;

            public const int HistoryMinLength = 10;
            public const int HistoryMaxLength = 500;

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
    }
}
