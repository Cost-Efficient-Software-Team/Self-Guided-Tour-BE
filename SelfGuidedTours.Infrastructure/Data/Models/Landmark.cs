using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Landmark;
namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Landmark
    {
        [Key]
        public int LandmarkId { get; set; }

        [Required]
        public int TourId { get; set; }
        [ForeignKey(nameof(TourId))]
        public Tour Tour { get; set; } = null!;

        [Required]
        public int StopOrder { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = LengthErrorMessage)]
        public string LocationName { get; set; } = null!;

        [StringLength(DescriptionMaxLength, ErrorMessage = LengthErrorMessage)]
        public string? Description { get; set; }

        [Required]
        public int CoordinateId { get; set; }

        [ForeignKey(nameof(CoordinateId))]
        public Coordinate Coordinate { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public string PlaceId { get; set; } = null!;
        public ICollection<LandmarkResource> Resources { get; set; } = new HashSet<LandmarkResource>();
    }
}
