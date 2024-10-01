namespace SelfGuidedTours.Core.Models.ResponseDto
{
    public class UserTransactionsResponseDto
    {
        public string TourTitle { get; set; } = null!;
        public string Date { get; set; } = null!;
        public decimal Price { get; set; }

    }
}
