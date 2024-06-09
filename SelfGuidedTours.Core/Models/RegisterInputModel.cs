using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants.Register;

namespace SelfGuidedTours.Core.Models
{
    public class RegisterInputModel
    {
        [Display(Name = nameof(Email))]
        [Required(ErrorMessage = RequiredMessage)]
        [EmailAddress(ErrorMessage = InvalidEmailAddressMessage)]
        public string Email { get; set; } = null!;

        [Display(Name = nameof(Name))]
        [Required(ErrorMessage = RequiredMessage)]
        public string Name { get; set; } = null!;

        [Display(Name = nameof(Password))]
        [Required(ErrorMessage = RequiredMessage)]
        [MinLength(PasswordMinLength, ErrorMessage = LengthErrorMessage)]
        public string Password { get; set; } = null!;

        [Display(Name = "Repeat Password")]
        [Required(ErrorMessage = RequiredMessage)]
        [Compare(nameof(Password), ErrorMessage = PasswordsDoNotMatchMessage)]
        public string RepeatPassword { get; set; } = null!;
    }
}
