using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants.Register;
namespace SelfGuidedTours.Core.Models.RequestDto
{
    public class CreateOrChangePasswordRequestDto
    {
        public string? CurrentPassword { get; set; }
        [Required]
        [RegularExpression(PasswordRegex,
        ErrorMessage = InvalidPasswordMessage)]
        public string NewPassword { get; set; } = null!;
    }
}
