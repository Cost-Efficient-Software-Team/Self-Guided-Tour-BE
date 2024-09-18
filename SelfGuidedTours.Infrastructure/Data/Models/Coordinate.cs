using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Landmark;
namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Coordinate
    {
        [Key]
        public int CoordinateId { get; set; }

        [Required]
        [Column(TypeName = "decimal(38, 20)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(38, 20)")]
        public decimal Longitude { get; set; }

        [StringLength(CityMaxLength, ErrorMessage = LengthErrorMessage)]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
