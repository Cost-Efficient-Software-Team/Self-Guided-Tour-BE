using Microsoft.EntityFrameworkCore;
using SelfGuidedTours.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.LandmarkResource;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class LandmarkResource
    {
        /// <summary>
        /// Landmark' Resource's Identifier
        /// </summary>
        [Key]
        [Comment("Landmark' Resource's Identifier")]
        public int LandmarkResourceId { get; set; }

        /// <summary>
        /// Landmark's Identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(LandmarkId))]
        [Comment("Landmark's Identifier")]
        public int LandmarkId { get; set; }

        /// <summary>
        /// Reference to Landmark
        /// </summary>
        [Required]
        [Comment("Reference to Landmark")]
        public Landmark Landmark { get; set; } = null!;

        /// <summary>
        /// Landmark's Resource's Type
        /// </summary>
        [Required]
        [EnumDataType(typeof(LandmarkResourceType))]
        [Comment("Landmark's Resource's Type")]
        public LandmarkResourceType Type { get; set; }

        /// <summary>
        /// Landmark's Resource's Url
        /// </summary>
        [Required]
        [MaxLength(UrlMaxLength,
            ErrorMessage = UrlLengthErrorMessage)]
        [Comment("Landmark's Resource's Url")]
        //TODO: figure out some url regex validation
        public string Url { get; set; } = null!;

        /// <summary>
        /// Landmark's Resource Created At
        /// </summary>
        [Comment("Landmark's Resource Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Landmark's Resource Updated At
        /// </summary>
        [Comment("Landmark's Resource Updated At")]
        public DateTime? UpdatedAt { get; set; }

    }
}
