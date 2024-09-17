using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class LandmarkResourceUpdateDTO
    {
        public int? LandmarkResourceId { get; set; }

        public IFormFile? ResourceFile { get; set; }

        public string? ResourceUrl { get; set; }

        [Required]
        public ResourceType Type { get; set; }

    }
}
