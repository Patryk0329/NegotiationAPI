using Microsoft.AspNetCore.Authorization;
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
            new Negotiation { Id = 4, ProductId = 1, CustomerEmail = "customer4@customer.com", OfferedPrice = 1100.00m, LastOfferDate = new DateTime(2025, 3, 25)}
        };


        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Negotiation>> GetAllNegotiations()
        {
            return Ok(_negotiations);
        }


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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public IActionResult AcceptNegotiation(int id)
        {
            var negotiation = _negotiations.FirstOrDefault(n => n.Id == id);
            if (negotiation is null)
                return NotFound();

            if (negotiation.GetEffectiveStatus() != NegotiationStatus.Open)
                return BadRequest("Negotiation is not active");

            negotiation.Status = NegotiationStatus.Accepted;
            negotiation.LastOfferDate = DateTime.UtcNow;

            return NoContent();
        }

        [HttpPatch("{id}/reject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public IActionResult RejectNegotiation(int id,[FromBody] RejectNegotiationDto dto)
        {
            var negotiation = _negotiations.FirstOrDefault(n => n.Id == id);

            if (negotiation is null)
                return NotFound();

            if (negotiation.GetEffectiveStatus() != NegotiationStatus.Open)
                return BadRequest("Negotiation is not active");


            negotiation.Status = NegotiationStatus.Rejected;
            negotiation.RejectionReason = dto.Reason.Trim();
            negotiation.LastOfferDate = DateTime.UtcNow;
            return NoContent();
        }

        [HttpPatch("{id}/reoffer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ProposeNewOffer(int id, [FromBody] NewOfferDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var negotiation = _negotiations.FirstOrDefault(n => n.Id == id);
            if (negotiation is null)
                return NotFound();

            if (negotiation.GetEffectiveStatus() != NegotiationStatus.Rejected)
                return BadRequest("Negotiation is not active");

            if (!negotiation.CustomerEmail.Equals(dto.CustomerEmail.Trim(), StringComparison.OrdinalIgnoreCase))
                return BadRequest("Customer email does not match the negotiation email");

            if (dto.NewPrice <= 0)
                return BadRequest("Price must be positive");

            if (dto.NewPrice >= negotiation.OfferedPrice)
                return BadRequest("New price must be lower than the current offered price");

            if (negotiation.AttemptCount >= Negotiation.MaxAttempts)
                return BadRequest("Negotiation has reached the maximum number of attempts");

            if (negotiation.ExpirationDate < DateTime.UtcNow)
                return BadRequest("Negotiation has expired");


            negotiation.Status = NegotiationStatus.Open;
            negotiation.AttemptCount++;
            negotiation.OfferedPrice = dto.NewPrice;
            negotiation.LastOfferDate = DateTime.UtcNow;
            return NoContent();
        }


    };
}
