﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Infrastructure.Data.Models
{
    public class Coordinate
    {
        [Key]
        public int CoordinateId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Latitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Longitude { get; set; }

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
