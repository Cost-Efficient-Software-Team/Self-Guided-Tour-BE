using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class LandmarkCreateTourDTO
    {
        [Required]
        public CoordinateCreateTourDTO Coordinates { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;
        
        [Required]
        public string Description { get; set; } = null!;

        public List<IFormFile>? Resources { get; set; }
    }
}
