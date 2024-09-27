using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Payment
    {
        /// <summary>
        /// Payment's Identifier
        /// </summary>
        [Key]
        [Comment("Payment's Identifier")]
        public int PaymentId { get; set; }

        /// <summary>
        /// Payment's User's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(UserId))]
        [Comment("Payment's User's Identifier")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Reference to Application User
        /// </summary>
        [Comment("Reference to User")]
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Payment's Tour's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(TourId))]
        [Comment("Payment's Tour's Identifier")]
        public int TourId { get; set; }

        /// <summary>
        /// Reference to Payment's Tour
        /// </summary>
        [Comment("Reference to Payment's Tour")]
        public Tour Tour { get; set; } = null!;

        /// <summary>
        /// Payment's Amount
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(38, 20)")]
        [Comment("Payment's Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Payment's Status
        /// </summary>
        [Comment("Payment's Status")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        /// <summary>
        /// Payment's Date
        /// </summary>
        [Required]
        [Comment("Payment's Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Payment Updated At
        /// </summary>
        [Comment("Payment Updated At")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Payment's Intent Identifier
        /// </summary>
        [Required]
        [Comment("Payment's Intent Identifier")]
        public string PaymentIntentId { get; set; } = null!;
    }
}