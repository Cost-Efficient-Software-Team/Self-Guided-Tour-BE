using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Tour;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
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
        [StringLength(TitleMaxLength,MinimumLength = TitleMinLength,
            ErrorMessage =LengthErrorMessage)]
        public string Title { get; set; } = null!;
       
        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength,
                       ErrorMessage = LengthErrorMessage)]
        public string Description { get; set; } = null!;
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Price { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<TourLandmark> TourLandmarks { get; set; } = new HashSet<TourLandmark>();
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public virtual ICollection<Review> Reviews { get; set; }  = new HashSet<Review>();
        public virtual ICollection<UserTours> UserTours { get; set; } = new HashSet<UserTours>();
    }
}