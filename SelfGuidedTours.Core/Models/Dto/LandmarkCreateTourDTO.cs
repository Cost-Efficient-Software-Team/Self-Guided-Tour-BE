using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class LandmarkCreateTourDTO
    {
        public LandmarkCreateTourDTO()
        {
            Resources = new List<IFormFile>();
        }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Longitude { get; set; }

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = null!;
        
        [Required]
        public string Description { get; set; } = null!;

        public List<IFormFile> Resources { get; set; }
    }
}
