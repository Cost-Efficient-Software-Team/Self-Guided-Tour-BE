using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants.Register;

namespace SelfGuidedTours.Core.Models.Auth.ResetPassword
{
    public class ResetPasswordRequestModel
    {
        [Required]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = RequiredMessage)]
        [Display(Name = nameof(Password))]
        [MinLength(PasswordMinLength, ErrorMessage = LengthErrorMessage)]
        [RegularExpression(PasswordRegex, ErrorMessage = InvalidPasswordMessage)]
        public string Password { get; set; } = null!;
    }
}
