using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class UserProfile
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}