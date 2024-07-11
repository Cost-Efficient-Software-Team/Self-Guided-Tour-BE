using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants.Register;

namespace SelfGuidedTours.Core.Models.Auth
{
    public class RegisterInputModel
    {
        [Display(Name = nameof(Email))]
        [EmailAddress]
        [Required(ErrorMessage = RequiredMessage)]
        [RegularExpression(EmailRegex, ErrorMessage = InvalidEmailAddressMessage)]
        public string Email { get; set; } = null!;

        [Display(Name = nameof(Name))]
        [Required(ErrorMessage = RequiredMessage)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = RequiredMessage)]
        [Display(Name = nameof(Password))]
        [RegularExpression(PasswordRegex, ErrorMessage = InvalidPasswordMessage)]
        public string Password { get; set; } = null!;

        [Display(Name = "Repeat Password")]
        [Required(ErrorMessage = RequiredMessage)]
        [Compare(nameof(Password), ErrorMessage = PasswordsDoNotMatchMessage)]
        public string RepeatPassword { get; set; } = null!;
    }
}
