using SelfGuidedTours.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.LandmarkResource;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using Microsoft.AspNetCore.Http;
namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class LandmarkResource
    {
        [Key]
        public int LandmarkResourceId { get; set; }

        [Required]
        public int LandmarkId { get; set; }
        [ForeignKey(nameof(LandmarkId))]
        public Landmark Landmark { get; set; } = null!;

        [Required]
        [EnumDataType(typeof(ResourceType))]
        public ResourceType Type { get; set; }

        [Required]
        [MaxLength(UrlMaxLength,
            ErrorMessage = UrlLengthErrorMessage)]
        //TODO: figure out some url regex validation
        public string Url { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

    }
}
