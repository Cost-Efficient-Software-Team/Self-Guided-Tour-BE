using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class TourCreateDTO
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public decimal? Price { get; set; } = null!;

        [Required]
        public string Location { get; set; } = null!;

        [Required]
        public IFormFile ThumbnailImage { get; set; } = null!;

        [Required]
        public int EstimatedDuration { get; set; }
        [Required]
        public ICollection<LandmarkCreateTourDTO> Landmarks { get; set; } = new List<LandmarkCreateTourDTO>();
    }
}