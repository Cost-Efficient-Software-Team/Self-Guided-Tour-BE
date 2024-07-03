using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class LandmarkCreateTourDTO
    {
        [Required]
        public decimal Latitude { get; set; }

        [Required] 
        public decimal Longitude { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public ICollection<IFormFile> Resources { get; set; } = new List<IFormFile>();
    }
}
