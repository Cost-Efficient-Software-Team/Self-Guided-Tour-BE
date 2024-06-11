using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants;

namespace SelfGuidedTours.Core.Models
{
    public class LoginInputModel
    {
        [Display(Name = nameof(Email))]
        [Required(ErrorMessage = RequiredMessage)]
        [EmailAddress(ErrorMessage = InvalidEmailAddressMessage)]
        public string Email { get; set; } = null!;

        [Display(Name = nameof(Password))]
        [Required(ErrorMessage = RequiredMessage)]
        public string Password { get; set; } = null!;
    }
}
