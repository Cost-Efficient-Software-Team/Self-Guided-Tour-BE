namespace SelfGuidedTours.Core.Models.Dto
{
    public class FinalizePaymentRequest
    {
        public string PaymentIntentId { get; set; } = null!;
        public int TourId { get; set; }
    }
}
