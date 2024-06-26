using System.ComponentModel.DataAnnotations;
namespace SelfGuidedTours.Core.Models.Auth
{
    public class ChangePasswordModel
    {
        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        public string CurrentPassword { get; set; } = null!;
        [Required]
        public string NewPassword { get; set; } = null!;
    }
}
