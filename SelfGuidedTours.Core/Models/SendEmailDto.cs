using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.ValidationConstants.EmailValidationConstants;
namespace SelfGuidedTours.Core.Models
{
    public class SendEmailDto
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage =("Please provide a valid email address"))] //TODO: move this to constants after merge
        [Required]
        public string To { get; set; } = null!;
        [Required]
        [MaxLength(MaxSubjectLength,
            ErrorMessage = SubjectLengthErrorMessage)]
        public string Subject { get; set; } = null!;
        [MaxLength(MaxBodyLength,
            ErrorMessage = BodyLengthErrorMessage)]
        public string? Body { get; set; }
    }
}
