namespace SelfGuidedTours.Common.ValidationConstants
{
    public static class BlobStorageConstants
    {
        public const int MaxFileSyzeInBytes = 104857600;
        public const int MaxThumbnailFileSyzeInBytes = 10485760;
        public static readonly string[] AllowedExtensions =
                                                            {
                                                            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg",     // Image Files
                                                            ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv",            // Video Files
                                                             ".mp3", ".wav", ".aac", ".flac", ".ogg",                   // Audio Files
                                                             ".txt", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt",  // Text Files
                                                            };
        public static readonly string[] AllowedThumbnailExtensions ={".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".svg", };

    }
}
