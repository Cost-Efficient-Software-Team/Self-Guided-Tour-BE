using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Landmark;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Landmark
    {
        /// <summary>
        /// Landmark's Identifier
        /// </summary>
        [Key]
        [Comment("Landmark's Identifier")]
        public int LandmarkId { get; set; }

        /// <summary>
        /// Landmark's Tour's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(TourId))]
        [Comment("Landmark's Tour's Identifier")]
        public int TourId { get; set; }

        /// <summary>
        /// Reference to Landmark's Tour
        /// </summary>
        [Comment("Reference to Landmark's Tour")]
        public Tour Tour { get; set; } = null!;

        /// <summary>
        /// Landmark's Stop Order
        /// </summary>
        [Required]
        [Comment("Landmark's Stop Order")]
        public int StopOrder { get; set; }

        /// <summary>
        /// Landmark's Location Name
        /// </summary>
        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = LengthErrorMessage)]
        [Comment("Landmark's Location Name")]
        public string LocationName { get; set; } = null!;

        /// <summary>
        /// Landmark's Description
        /// </summary>
        [StringLength(DescriptionMaxLength, ErrorMessage = LengthErrorMessage)]
        [Comment("Landmark's Description")]
        public string? Description { get; set; }

        /// <summary>
        /// Landmark's Coordinate Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(CoordinateId))]
        [Comment("Landmark's Coordinate Identifier")]
        public int CoordinateId { get; set; }

        /// <summary>
        /// Reference to Landmark's Coordinate
        /// </summary>
        [Required]
        [Comment("Reference to Landmark's Coordinate")]
        public Coordinate Coordinate { get; set; } = null!;

        /// <summary>
        /// Landmark's Place Identifier
        /// </summary>
        [Required]
        [Comment("Landmark's Place Identifier")]
        public string PlaceId { get; set; } = null!;

        /// <summary>
        /// Landmark Created At
        /// </summary>
        [Comment("Landmark Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Landmark Update At
        /// </summary>
        [Comment("Landmark Update At")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<LandmarkResource> Resources { get; set; } = new HashSet<LandmarkResource>();
    }
}
