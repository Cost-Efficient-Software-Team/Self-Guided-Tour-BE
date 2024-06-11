using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Core.Models.Auth
{
    public class RefreshRequestModel
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
