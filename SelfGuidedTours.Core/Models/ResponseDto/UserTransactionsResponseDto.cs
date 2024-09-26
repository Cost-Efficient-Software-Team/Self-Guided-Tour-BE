using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Models.ResponseDto
{
    public class UserTransactionsResponseDto
    {
        public string TourTitle { get; set; } = null!;
        public string Date { get; set; } = null!;
        public decimal Price { get; set; }

    }
}
