using Microsoft.AspNetCore.Http;
using SelfGuidedTours.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Tour;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class TourUpdateDTO
    {
        public TourUpdateDTO()
        {
            Landmarks = new HashSet<LandmarkCreateTourDTO>();
        }

        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, ErrorMessage = LengthErrorMessage)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(SummaryMaxLength, MinimumLength = SummaryMinLength, ErrorMessage = LengthErrorMessage)]
        public string Summary { get; set; } = null!;

        [Range(PriceMinValue, int.MaxValue)]
        public decimal? Price { get; set; } = null!;

        [Required]
        [StringLength(DestinationMaxLength, MinimumLength = DestinationMinLength, ErrorMessage = LengthErrorMessage)]
        public string Destination { get; set; } = null!;

        [Required]
        public TypeTour TypeTour { get; set; }

        public IFormFile? ThumbnailImage { get; set; }

        [Required]
        [Range(EstimatedDurationMinValueInMinutes, EstimatedDurationMaxValueInMinutes)]
        public int EstimatedDuration { get; set; }

        public ICollection<LandmarkCreateTourDTO> Landmarks { get; set; }
    }
}