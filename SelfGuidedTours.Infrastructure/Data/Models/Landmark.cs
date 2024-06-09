using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Landmark;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Landmark
    {
        [Key]
        public int LandmarkId { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength,
            ErrorMessage = LengthErrorMessage)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength,
            ErrorMessage = LengthErrorMessage)]
        public string Description { get; set; } = null!;
        [Required]
        [StringLength(HistoryMaxLength, MinimumLength = HistoryMinLength,
            ErrorMessage = LengthErrorMessage)]
        public string History { get; set; } = null!;

        [Required]
        public int CoordinateId { get; set; }
        [ForeignKey(nameof(CoordinateId))]
        public Coordinate Coordinate { get; set; } = null!;
        //TODO: Add Image and Video Urls validation
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }

        public virtual ICollection<TourLandmark> TourLandmarks { get; set; } = new HashSet<TourLandmark>();
    }
}
