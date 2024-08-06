using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Tour;
namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Tour
    {
        [Key]
        public int TourId { get; set; }

        [Required]
        public string CreatorId { get; set; } = null!;
        [ForeignKey(nameof(CreatorId))]
        public ApplicationUser Creator { get; set; } = null!;

        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, ErrorMessage = LengthErrorMessage)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(SummaryMaxLength, MinimumLength = SummaryMinLength, ErrorMessage = LengthErrorMessage)]
        public string Summary { get; set; } = null!;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Price { get; set; }

        [Required]
        [StringLength(DestinationMaxLength, MinimumLength = DestinationMinLength, ErrorMessage = LengthErrorMessage)]
        public string Destination { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //TODO: Add Image and Video Urls validation
        [Required]
        [MaxLength(ThumbnailImageUrlMaxLength, ErrorMessage = UrlLengthErrorMessage)]
        public string ThumbnailImageUrl { get; set; } = null!;

        [Required]
        [Comment("Estimated duration in minutes")]
        public int EstimatedDuration { get; set; }

        [Comment("On create, status is pending until approved or rejected by admin.")]
        public Status Status { get; set; } = Status.Pending;

        [Required]
        public TypeTour TypeTour { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Landmark> Landmarks { get; set; } = new HashSet<Landmark>();
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public virtual ICollection<UserTours> UserTours { get; set; } = new HashSet<UserTours>();
    }
}