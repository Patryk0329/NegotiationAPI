using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NegotiationAPI.Models;

namespace NegotiationAPI.Controllers
{


    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, ProductName = "Laptop", BasePrice = 2999.99m },
            new Product { Id = 2, ProductName = "Smartphone", BasePrice = 1599.50m }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            return Ok(_products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> AddProduct(Product newProduct)
        {
            if (newProduct is null)
                return BadRequest();

            newProduct.Id = _products.Max(p => p.Id) + 1;
            _products.Add(newProduct);

            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }
    }
}
