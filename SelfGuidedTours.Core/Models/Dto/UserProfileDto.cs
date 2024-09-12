namespace SelfGuidedTours.Core.Models.Dto
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? About { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
