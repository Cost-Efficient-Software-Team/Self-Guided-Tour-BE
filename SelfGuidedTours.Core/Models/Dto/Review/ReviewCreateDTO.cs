using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Core.Models.Dto.Review
{
    public class ReviewCreateDTO
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment length cannot exceed 500 characters.")]
        public string? Comment { get; set; }
    }
}