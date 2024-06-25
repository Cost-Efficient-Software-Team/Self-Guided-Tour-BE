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
        public string ThumbnailImageUrl { get; set; } = null!;

        [Required]
        public int EstimatedDuration { get; set; }
    }
}