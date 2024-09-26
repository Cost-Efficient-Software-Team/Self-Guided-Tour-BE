using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Application User's Name
        /// </summary>
        [Comment("Application User's Name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Id of the stripe customer associated with the user. Created when the user makes a payment.
        /// </summary>
        [Comment("Id of the stripe customer associated with the user. Created when the user makes a payment.")]
        public string? StripeCustomerId { get; set; }

        /// <summary>
        /// Application User's Credentials
        /// </summary>
        [Comment("Application User's Credentials")]
        public string? Credentials { get; set; }

        /// <summary>
        /// Application User's Profile Created At
        /// </summary>
        [Required]
        [Comment("Application User's Profile Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Reference to Application User's Wallet
        /// </summary>
        [Comment("Reference to Application User's Wallet")]
        public Wallet Wallet { get; set; } = null!;

        /// <summary>
        /// Application User's Profile Updated At
        /// </summary>
        [Comment("Application User's Profile Updated At")]
        public DateTime? UpdatedAt { get; set;}

        public virtual ICollection<Tour> CreatedTours { get; set; } = new HashSet<Tour>();
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public virtual ICollection<UserTours> UserTours { get; set; } = new HashSet<UserTours>();
    }
}
