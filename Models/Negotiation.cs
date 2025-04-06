using System.Data;
using System.Text.Json.Serialization;

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
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal OfferedPrice { get; set; }
        public string? RejectionReason { get; set; }
        public int AttemptCount { get; set; } = 1;

        [JsonIgnore]
        public NegotiationStatus Status { get; set; } = NegotiationStatus.Open;
        public DateTime LastOfferDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpirationDate => LastOfferDate.AddDays(ExpirationDays);

        [JsonPropertyName("EffectiveStatus")]
        public NegotiationStatus EffectiveStatus => GetEffectiveStatus();
        public NegotiationStatus GetEffectiveStatus()
        {
            if (Status == NegotiationStatus.Open && DateTime.UtcNow > ExpirationDate)
                return NegotiationStatus.Expired;

            return Status;
        }
    }
}
