using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Landmark;
namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Coordinate
    {
        /// <summary>
        /// Coordinate's Identifier
        /// </summary>
        [Key]
        [Comment("Coordinate's Identifier")]
        public int CoordinateId { get; set; }

        /// <summary>
        /// Coordinate's Latitude
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(38, 20)")]
        [Comment("Coordinate's Latitude")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Coordinate's Longitude
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(38, 20)")]
        [Comment("Coordinate's Longitude")]
        public decimal Longitude { get; set; }

        /// <summary>
        /// Coordinate's City
        /// </summary>
        [StringLength(CityMaxLength, ErrorMessage = LengthErrorMessage)]
        [Comment("Coordinate's City")]
        public string? City { get; set; }

        /// <summary>
        /// Coordinate's Country
        /// </summary>
        [Required]
        [Comment("Coordinate's Country")]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Coordinate Created At
        /// </summary>
        [Comment("Coordinate Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Coordinate Updated At
        /// </summary>
        [Comment("Coordinate Updated At")]
        public DateTime? UpdatedAt { get; set; }
    }
}
