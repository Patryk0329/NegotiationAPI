using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NegotiationAPI.Models;

namespace NegotiationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NegotiationsController : ControllerBase
    {
        private static List<Negotiation> _negotiations = new List<Negotiation>
        {
            new Negotiation { Id = 1, ProductId = 1, CustomerEmail = "customer1@customer.com", OfferedPrice = 2000.00m},
            new Negotiation { Id = 2, ProductId = 1, CustomerEmail = "customer2@customer.com", OfferedPrice = 2700.00m},
            new Negotiation { Id = 3, ProductId = 2, CustomerEmail = "customer3@customer.com", OfferedPrice = 1000.00m, Status = NegotiationStatus.Rejected, AttemptCount = 2},
        };


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Negotiation), 200)]
        [ProducesResponseType(404)]
        public ActionResult<Negotiation> GetNegotiationById(int id)
        {
            var negotiation = _negotiations.FirstOrDefault(n => n.Id == id);
            if(negotiation is null)
                return NotFound();

            return Ok(negotiation);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Negotiation), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult StartNegotiation([FromBody] StartNegotiationDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var products = new List<Product>
            {
                new Product { Id = 1, ProductName = "Laptop", BasePrice = 2999.99m },
                new Product { Id = 2, ProductName = "Smartphone", BasePrice = 1599.50m }
            };

            if (!products.Any(p => p.Id == dto.ProductId))
                return BadRequest("Invalid product ID");

            if (dto.OfferedPrice <= 0)
                return BadRequest("Price must be positive");

            var newNegotiation = new Negotiation
            {
                Id = _negotiations.Count > 0 ? _negotiations.Max(n => n.Id) + 1 : 1,
                ProductId = dto.ProductId,
                CustomerEmail = dto.CustomerEmail.Trim(),
                OfferedPrice = dto.OfferedPrice,
                LastOfferDate = DateTime.UtcNow,
            };

            _negotiations.Add(newNegotiation);

            return CreatedAtAction(
                nameof(GetNegotiationById),
                new { id = newNegotiation.Id },
                newNegotiation
            );
        }


        [HttpPatch("{id}/accept")]
        public IActionResult AcceptNegotiation(int id)
        {
            var negotiation = _negotiations.FirstOrDefault(n => n.Id == id);
            if (negotiation is null)
                return NotFound();

            if (negotiation.Status != NegotiationStatus.Open)
                return BadRequest("Negotiation is not active");

            negotiation.Status = NegotiationStatus.Accepted;
            negotiation.LastOfferDate = DateTime.UtcNow;

            return NoContent();
        }

    };
}
