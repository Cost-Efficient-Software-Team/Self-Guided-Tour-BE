namespace SelfGuidedTours.Core.Models.ResponseDto
{
    public class LandmarkResponseDto
    {
        public int LandmarkId { get; set; }

        public string City { get; set; } = string.Empty;

        public string LandmarkName { get; set; } = null!;

        public string Description { get; set; } = null!;
        public int StopOrder { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public ICollection<ResourceResponseDto> Resources { get; set; } = new HashSet<ResourceResponseDto>();
    }
}
