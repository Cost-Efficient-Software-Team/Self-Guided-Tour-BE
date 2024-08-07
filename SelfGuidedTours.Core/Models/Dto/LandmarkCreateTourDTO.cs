﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SelfGuidedTours.Common.ValidationConstants.ValidationConstants.Landmark;
using static SelfGuidedTours.Common.MessageConstants.ErrorMessages;
namespace SelfGuidedTours.Core.Models.Dto
{
    public class LandmarkCreateTourDTO
    {
        public LandmarkCreateTourDTO()
        {
            Resources = new List<IFormFile>();
        }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Longitude { get; set; }

        [StringLength(CityMaxLength, MinimumLength = CityMinLength, ErrorMessage = LengthErrorMessage)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = LengthErrorMessage)]
        public string LocationName { get; set; } = null!;
        [Required]
        public int StopOrder { get; set; }

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = LengthErrorMessage)]
        public string Description { get; set; } = null!;

        public List<IFormFile> Resources { get; set; }
    }
}
