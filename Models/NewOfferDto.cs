using System.ComponentModel.DataAnnotations;

namespace NegotiationAPI.Models
{
    public class NewOfferDto
    {
        [Required(ErrorMessage = "New offer price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "New offer price must be greater than 0")]
        public decimal NewPrice { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string CustomerEmail { get; set; } = string.Empty;
    }
}
