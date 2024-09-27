using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Models.ResponseDto
{
    public class ProfileToursResponseDto
    {
        public int TourId { get; set; }
        public string CreatorId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public decimal? Price { get; set; }
        public string Destination { get; set; } = null!;
        public string ThumbnailImageUrl { get; set; } = null!;
        public int EstimatedDuration { get; set; }
        public string CreatorName { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
