using System.ComponentModel.DataAnnotations;

namespace SelfGuidedTours.Core.Models.ExternalLogin
{
    public class GoogleSignInVM
    {
        [Required]
        public string IdToken { get; set; } = null!;
    }
}
