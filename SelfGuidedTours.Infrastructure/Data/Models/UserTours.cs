using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class UserTours
    {
        [Required]
        [Key]
        public int UserTourId { get; set; }

        public string UserId { get; set; } = null!;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int TourId { get; set; }
        [ForeignKey(nameof(TourId))]
        public Tour Tour { get; set; } = null!;

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
