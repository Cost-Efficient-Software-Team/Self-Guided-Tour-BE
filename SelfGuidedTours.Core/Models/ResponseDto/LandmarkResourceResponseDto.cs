namespace SelfGuidedTours.Core.Models.ResponseDto
{
    public class LandmarkResourceResponseDto
    {
        public int ResourceId { get; set; }
        public string ResourceUrl { get; set; } = null!;
        public string ResourceType { get; set; } = null!;
    }
}
