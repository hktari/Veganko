using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Data;
using VegankoService.Helpers;
using VegankoService.Models;

namespace VegankoService.Controllers
{
    [Authorize(
        Policy = "ApiUser",
        Roles = Constants.Strings.Roles.Admin + ", " + Constants.Strings.Roles.Manager + ", " + Constants.Strings.Roles.Moderator)]
    [Route("api/products")]
    [ApiController]
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
            if (input.ImageBase64Encoded  == null)
            {
                return new BadRequestResult();
            }

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

        [Authorize(Roles = Constants.Strings.Roles.Member)]
        [HttpGet]
        public ActionResult<PagedList<Product>> GetAll(int page = 1, int count = 10)
        {
            return productRepository.GetAll(page, count);
        }

        [Authorize(Roles = Constants.Strings.Roles.Member)]
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
