namespace SelfGuidedTours.Core.Models.ResponseDto
{
    public class TourResponseDto
    {
        public int TourId { get; set; }
        public string CreatorId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public decimal? Price { get; set; }
        public string Destination { get; set; } = null!;
        public string ThumbnailImageUrl { get; set; } = null!;
        public int EstimatedDuration { get; set; }
        public string CreatedAt { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string TourType { get; set; } = null!;
        public decimal AverageRating { get; set; }

        public ICollection<LandmarkResponseDto> Landmarks { get; set; } = new List<LandmarkResponseDto>();
    }
}
