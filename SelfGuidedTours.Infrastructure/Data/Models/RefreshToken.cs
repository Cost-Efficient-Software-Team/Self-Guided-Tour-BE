using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class RefreshToken
    {
        /// <summary>
        /// Refresh Token's Identifier
        /// </summary>
        [Required]
        [Comment("Refresh Token's Identifier")]
        public Guid Id { get; set; }

        /// <summary>
        /// Token secret
        /// </summary>
        [Required]
        [Comment("Token Secret")]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Refresh Token's User's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(UserId))]
        [Comment("Refresh Token's User's Identifier")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Reference to Application User
        /// </summary>
        [Comment("Reference to User")]
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Refresh Token Created At
        /// </summary>
        [Comment("Refresh Token Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
