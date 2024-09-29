namespace SelfGuidedTours.Core.Models.Dto.Review
{
    public class ReviewDTO
    {
        public int ReviewId { get; set; }
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? UserImg { get; set; }
        public int TourId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
