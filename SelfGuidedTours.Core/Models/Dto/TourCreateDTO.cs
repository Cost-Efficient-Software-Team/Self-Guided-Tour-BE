using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class TourCreateDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "The Title length must be between 5 and 100 characters.")]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "The Description length must be between 10 and 500 characters.")]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Price { get; set; }

        [Required]
        public string Location { get; set; } = null!;

        [Required]
        [MaxLength(255, ErrorMessage = "The ThumbnailImageUrl length must not exceed 255 characters.")]
        public string ThumbnailImageUrl { get; set; } = null!;

        [Required]
        [Range(1, 1000, ErrorMessage = "The Estimated Duration must be between 1 and 1000 minutes.")]
        public int EstimatedDuration { get; set; }

    }
}