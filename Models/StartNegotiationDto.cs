using System.ComponentModel.DataAnnotations;

namespace NegotiationAPI.Models
{
    public class StartNegotiationDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal OfferedPrice { get; set; }

        [Required(ErrorMessage = "Customer email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string CustomerEmail { get; set; }

    }
}
