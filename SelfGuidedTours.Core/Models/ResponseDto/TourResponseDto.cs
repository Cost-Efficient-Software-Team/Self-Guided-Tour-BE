using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Models.ResponseDto
{
    public class TourResponseDto
    {
        public int TourId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal? Price { get; set; }
        public string Location { get; set; } = null!;
        public string ThumbnailImageUrl { get; set; } = null!;
        public int EstimatedDuration { get; set; }
        public string Status { get; set; } = null!;

        public ICollection<LandmarkResponseDto> Landmarks { get; set; } = new List<LandmarkResponseDto>();
    }
}
