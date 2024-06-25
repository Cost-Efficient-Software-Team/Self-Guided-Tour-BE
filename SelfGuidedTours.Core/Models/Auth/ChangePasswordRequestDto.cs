using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants.Register;
namespace SelfGuidedTours.Core.Models.Auth
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        [MinLength(PasswordMinLength,
            ErrorMessage = InvalidPasswordMessage)]
        public string NewPassword { get; set; } = null!;
    }
}
