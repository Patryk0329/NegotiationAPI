namespace NegotiationAPI.Models
{
    public enum NegotiationStatus
    {
        Open,
        Accepted,
        Rejected,
        Expired

    }

    public class Negotiation
    {
        public const int MaxAttempts = 3;
        public const int ExpirationDays = 7;

        public int Id { get; set; }
        public int ProductId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public decimal OfferedPrice { get; set; }
        public int AttemptCount { get; set; } = 1;
        public NegotiationStatus Status { get; set; } = NegotiationStatus.Open;
        public DateTime LastOfferDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpirationDate => LastOfferDate.AddDays(ExpirationDays);
    }
}
