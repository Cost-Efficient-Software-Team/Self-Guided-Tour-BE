namespace SelfGuidedTours.Core.Models
{
    public class PaymentResult
    {
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Amount { get; set; }
    }
}