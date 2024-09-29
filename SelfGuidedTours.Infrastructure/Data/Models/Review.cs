using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Review;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Review
    {
        /// <summary>
        /// Review's Identifier
        /// </summary>
        [Key]
        [Comment("Review's Identifier")]
        public int ReviewId { get; set; }

        /// <summary>
        /// Review's User's Identifier
        /// </summary>
        [Required]
        [Comment("Review's User's Identifier")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Reference to Review's User
        /// </summary>
        [ForeignKey("UserId")]
        [Comment("Reference to User")]
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Review's Tour's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(TourId))]
        [Comment("Review's Tour's Identifier")]
        public int TourId { get; set; }

        /// <summary>
        /// Reference to Review's Tour
        /// </summary>
        [Comment("Reference to Tour")]
        public Tour Tour { get; set; } = null!;

        /// <summary>
        /// Review's Rating
        /// </summary>
        [Required]
        [Range(1, 5)]
        [Comment("Review's Rating")]
        public int Rating { get; set; }

        /// <summary>
        /// Review's Comment
        /// </summary>
        [StringLength(CommentMaxLength, ErrorMessage = CommentLengthErrorMessage)]
        [Comment("Review's Comment")]
        public string? Comment { get; set; }

        /// <summary>
        /// Review's Date
        /// </summary>
        [Required]
        [Comment("Review's Date")]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Review Updated At
        /// </summary>
        [Comment("Review Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
}