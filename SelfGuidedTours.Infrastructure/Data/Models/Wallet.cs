using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Wallet
    {
        /// <summary>
        /// Wallet's Identifier
        /// </summary>
        [Key]
        [Comment("Wallet's Identifier")]
        public int WalletId { get; set; }

        /// <summary>
        /// Wallet's User's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(UserId))]
        [Comment("Wallet's User's Identifier")]
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Reference to Wallet's User
        /// </summary>
        [Comment("Reference to Wallet's User")]
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Wallet's Balance
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(38, 20)")]
        [Comment("Wallet's Balance")]
        public decimal Balance { get; set; } = 0;

        /// <summary>
        /// Wallet Created At
        /// </summary>
        [Comment("Wallet Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Wallet's Updated At
        /// </summary>
        [Comment("Wallet's Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
}