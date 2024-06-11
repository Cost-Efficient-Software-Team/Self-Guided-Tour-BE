using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Core.Models.Auth
{
    public class LogoutInputModel
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
