using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class UserTours
    {
        /// <summary>
        /// UserTour's Identifier
        /// </summary>
        [Required]
        [Key]
        [Comment("UserTour's Identifier")]
        public int UserTourId { get; set; }

        /// <summary>
        /// User's Identifier
        /// </summary>
        /// [ForeignKey(nameof(UserId))]
        [Comment("User's Identifier")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Reference to User
        /// </summary>
        [Comment("Reference to User")]
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Tour's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(TourId))]
        [Comment("Tour's Identifier")]
        public int TourId { get; set; }

        /// <summary>
        /// Reference to Tour
        /// </summary>
        [Comment("Reference to Tour")]
        public Tour Tour { get; set; } = null!;

        /// <summary>
        /// UserTour's Purchase Date
        /// </summary>
        [Required]
        [Comment("UserTours's Purchase Date")]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        /// <summary>
        /// UserTours Updated At
        /// </summary>
        [Comment("UserTours Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
}
