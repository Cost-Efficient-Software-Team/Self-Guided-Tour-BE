using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SelfGuidedTours.Core.Models.Dto
{
    public class TourCreateDTO
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal? Price { get; set; }

        public string Location { get; set; } = null!;

        public string ThumbnailImageUrl { get; set; } = null!;

        public int EstimatedDuration { get; set; }
    }
}