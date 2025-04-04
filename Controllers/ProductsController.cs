using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NegotiationAPI.Models;

namespace NegotiationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products = new()
        {
            new Product { Id = 1, ProductName = "Laptop", BasePrice = 2999.99m },
            new Product { Id = 2, ProductName = "Smartphone", BasePrice = 1599.50m }
        };

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            return Ok(_products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> GetProductById(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Product> AddProduct([FromBody] CreateProductDto dtoProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newProduct = new Product
            {
                ProductName = dtoProduct.ProductName.Trim(),
                BasePrice = dtoProduct.BasePrice,
                Id = _products.Max(p => p.Id) + 1
            };

            _products.Add(newProduct);

            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }
    }
}
