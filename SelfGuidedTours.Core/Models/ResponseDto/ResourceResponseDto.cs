using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Models.ResponseDto
{
    public class ResourceResponseDto
    {
        public int ResourceId { get; set; }
        public string ResourceUrl { get; set; } = null!;
        public string ResourceType { get; set; } = null!;
    }
}
