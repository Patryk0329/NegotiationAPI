using System.ComponentModel.DataAnnotations;

namespace NegotiationAPI.Models
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name must be ≤ 100 characters")]
        public string ProductName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be ≥ 0.01")]
        public decimal BasePrice { get; set; }
    }
}
