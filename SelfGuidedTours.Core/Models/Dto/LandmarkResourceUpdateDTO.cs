using Microsoft.AspNetCore.Http;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class LandmarkResourceUpdateDTO
    {
        public int? ResourceId { get; set; }
        public string? ResourceUrl { get; set; }
        public int? ResourceType { get; set; }
        public IFormFile? ResourceFile { get; set; }
    }
}
