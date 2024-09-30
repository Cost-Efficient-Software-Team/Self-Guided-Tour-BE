using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.ValidationConstants.AuthConstants.Register;
namespace SelfGuidedTours.Core.Models.RequestDto
{
    public class CreatePasswordRequestDto
    {
        [Required]
        [RegularExpression(PasswordRegex,
        ErrorMessage = InvalidPasswordMessage)]
        public string Password { get; set; } = null!;
    }
}
