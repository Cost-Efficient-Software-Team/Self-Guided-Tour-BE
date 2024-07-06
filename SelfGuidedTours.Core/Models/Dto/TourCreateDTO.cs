using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Tour;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
namespace SelfGuidedTours.Core.Models.Dto
{
    public class TourCreateDTO
    {
        public TourCreateDTO()
        {
            Landmarks = new HashSet<LandmarkCreateTourDTO>();
        }
        //TODO: add validation
        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, ErrorMessage = LengthErrorMessage)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = LengthErrorMessage)]
        public string Description { get; set; } = null!;

        [Range(PriceMinValue, int.MaxValue)]
        public decimal? Price { get; set; } = null!;

        [Required]
        [StringLength(LocationMaxLength, MinimumLength = LocationMinLength, ErrorMessage = LengthErrorMessage)]
        public string Location { get; set; } = null!;

        [Required]
        public IFormFile ThumbnailImage { get; set; } = null!;

        [Required]
        [Range(EstimatedDurationMinValueInMinutes, EstimatedDurationMaxValueInMinutes)]
        public int EstimatedDuration { get; set; }

        [Required]
        public ICollection<LandmarkCreateTourDTO> Landmarks { get; set; }
    }
}