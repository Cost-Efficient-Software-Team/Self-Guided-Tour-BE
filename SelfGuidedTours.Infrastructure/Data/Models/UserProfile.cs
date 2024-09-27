using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class UserProfile
    {
        /// <summary>
        /// User's Identifier
        /// </summary>
        [Key]
        [Comment("User's Identifier")]
        public Guid UserId { get; set; }

        /// <summary>
        /// User's Name
        /// </summary>
        [Required]
        [StringLength(100)]
        [Comment("User's Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User's Email
        /// </summary>
        [Required]
        [EmailAddress]
        [Comment("User's Email")]
        public string Email { get; set; } = string.Empty;
    }
}