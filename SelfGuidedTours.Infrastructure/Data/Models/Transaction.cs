using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Transaction
    {
        /// <summary>
        /// Transaction's Identifier
        /// </summary>
        [Key]
        [Comment("Transaction's Identifier")]
        public int TransactionId { get; set; }

        /// <summary>
        /// Transaction's Wallet's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(WalletId))]
        [Comment("Transaction's Wallet's Identifier")]
        public int WalletId { get; set; }

        /// <summary>
        /// Reference to Transaction's Wallet
        /// </summary>
        [Comment("Reference to Transaction's Wallet")]
        public Wallet Wallet { get; set; } = null!;

        /// <summary>
        /// Transaction's Amount
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(38, 20)")]
        [Comment("Transaction's Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Transaction's Date
        /// </summary>
        [Required]
        [Comment("Transaction's Date")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Transaction's Type
        /// </summary>
        [Required]
        [Comment("Transaction's Type")]
        public string TransactionType { get; set; } = null!; // Should be 'deposit' or 'withdrawal' could be a enum

        /// <summary>
        /// Transaction Updated At
        /// </summary>
        [Comment("Transaction Updated At")]
        public DateTime? UpdatedAt { get; set; }

    }
}
