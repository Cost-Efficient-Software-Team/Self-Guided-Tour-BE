using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class TourCreateDTO
    {
        public TourCreateDTO()
        {
            Landmarks = new HashSet<LandmarkCreateTourDTO>();
        }

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
        public ICollection<LandmarkCreateTourDTO> Landmarks { get; set; }
    }
}