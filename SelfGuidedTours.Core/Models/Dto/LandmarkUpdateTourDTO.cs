using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Landmark;
namespace SelfGuidedTours.Core.Models.Dto
{
    public class LandmarkUpdateTourDTO
    {
        public LandmarkUpdateTourDTO()
        {
            Resources = new List<LandmarkResourceUpdateDTO>();
        }

        [Required]
        [Column(TypeName = "decimal(10, 6)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 6)")]
        public decimal Longitude { get; set; }

        [StringLength(CityMaxLength, MinimumLength = CityMinLength, ErrorMessage = LengthErrorMessage)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = LengthErrorMessage)]
        public string LocationName { get; set; } = null!;

        [Required]
        public int StopOrder { get; set; }

        [StringLength(DescriptionMaxLength, ErrorMessage = LengthErrorMessage)]
        public string? Description { get; set; }

        [Required]
        public string PlaceId { get; set; } = null!;

        public List<LandmarkResourceUpdateDTO> Resources { get; set; }
    }
}
