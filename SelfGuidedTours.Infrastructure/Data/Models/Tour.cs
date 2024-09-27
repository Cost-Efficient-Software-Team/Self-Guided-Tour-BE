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
        /// <summary>
        /// Tour's Identifier
        /// </summary>
        [Key]
        [Comment("Tour's Identifier")]
        public int TourId { get; set; }

        /// <summary>
        /// Tour's Creator's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(CreatorId))]
        [Comment("Tour's Creator's Identifier")]
        public string CreatorId { get; set; } = null!;

        /// <summary>
        /// Reference to Review's Creator
        /// </summary>
        [Comment("Reference to Tour's Creator")]
        public ApplicationUser Creator { get; set; } = null!;

        /// <summary>
        /// Tour's Title
        /// </summary>
        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, ErrorMessage = LengthErrorMessage)]
        [Comment("Tour's Title")]
        public string Title { get; set; } = null!;

        /// <summary>
        /// Tour's Summary
        /// </summary>
        [Required]
        [StringLength(SummaryMaxLength, MinimumLength = SummaryMinLength, ErrorMessage = LengthErrorMessage)]
        [Comment("Tour's Summary")]
        public string Summary { get; set; } = null!;

        /// <summary>
        /// Tour's Price
        /// </summary>
        [Column(TypeName = "decimal(38, 20)")]
        [Comment("Tour's Price")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Tour's Destination
        /// </summary>
        [Required]
        [StringLength(DestinationMaxLength, MinimumLength = DestinationMinLength, ErrorMessage = LengthErrorMessage)]
        [Comment("Tour's Destination")]
        public string Destination { get; set; } = null!;

        /// <summary>
        /// Tour Created At
        /// </summary>
        [Required]
        [Comment("Tour Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Tour's Average Rating
        /// </summary>
        [Column(TypeName = "decimal(3, 2)")]
        [Comment("Tour's Average Rating")]
        public decimal AverageRating { get; set; }

        /// <summary>
        /// Tour's Thumbnail Image Url
        /// </summary>
        [Required]
        [MaxLength(ThumbnailImageUrlMaxLength, ErrorMessage = UrlLengthErrorMessage)]
        [Comment("Tour's Thumbnail Image Url")]
        public string ThumbnailImageUrl { get; set; } = null!;

        /// <summary>
        /// Tour's Estimated duration in minutes
        /// </summary>
        [Required]
        [Comment("Estimated duration in minutes")]
        public int EstimatedDuration { get; set; }

        /// <summary>
        /// Tour's Status
        /// </summary>
        [Comment("On create, status is UnderReview until approved or rejected by admin.")]
        public Status Status { get; set; } = Status.UnderReview;

        /// <summary>
        /// Tour's Type
        /// </summary>
        [Required]
        [Comment("Tour's Type")]
        public TypeTour TypeTour { get; set; } = TypeTour.Walking;
        
        /// <summary>
        /// Tour Updated At
        /// </summary>
        [Comment("Tour Updated At")]
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Landmark> Landmarks { get; set; } = new HashSet<Landmark>();
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public virtual ICollection<UserTours> UserTours { get; set; } = new HashSet<UserTours>();
    }
}
