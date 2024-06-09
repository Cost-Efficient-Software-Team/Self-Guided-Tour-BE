using System.ComponentModel.DataAnnotations;
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
    }
}
