using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Data;
using VegankoService.Models;

namespace VegankoService.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        [HttpPost]
        public ActionResult<Product> Post(ProductInput input)
        {
            var product = new Product();
            input.MapToProduct(product);
            productRepository.Create(product);
            return CreatedAtAction(
                nameof(ProductsController.Get), "products", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public ActionResult<Product> Put(string id, ProductInput input)
        {
            var product = productRepository.Get(id);

            if (product == null)
                return NotFound();

            input.MapToProduct(product);
            productRepository.Update(product);

            return product;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var game = productRepository.Get(id);
            if (game == null)
                return NotFound();

            productRepository.Delete(id);

            return Ok();
        }

        [HttpGet]
        public ActionResult<PagedList<Product>> GetAll(int page = 1, int count = 10)
        {
            return  productRepository.GetAll(page, count);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            Product product = productRepository.Get(id);
            if (product == null)
                return NotFound();

            return product;
        }
    }
}
