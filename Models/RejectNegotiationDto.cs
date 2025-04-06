using System.ComponentModel.DataAnnotations;

namespace NegotiationAPI.Models
{
    public class RejectNegotiationDto
    {
        [Required(ErrorMessage = "Rejection reason is required")]
        [StringLength(500, ErrorMessage = "Reason must be less than 500 characters")]
        public string? Reason { get; set; }
    }
}
