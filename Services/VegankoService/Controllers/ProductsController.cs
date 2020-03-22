using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using VegankoService.Data;
using VegankoService.Helpers;
using VegankoService.Models;
using VegankoService.Models.ErrorHandling;
using VegankoService.Models.Products;
using VegankoService.Services.Images;

namespace VegankoService.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : Controller
    {
        private const string RestrictedAccessRoles = Constants.Strings.Roles.Admin + ", " + Constants.Strings.Roles.Manager + ", " + Constants.Strings.Roles.Moderator;
        private readonly IProductRepository productRepository;
        private readonly ILogger<ProductsController> logger;
        private readonly VegankoContext context;
        private readonly IImageService imageService;

        public ProductsController(
            IProductRepository productRepository,
            ILogger<ProductsController> logger,
            VegankoContext context,
            IImageService imageService)
        {
            this.productRepository = productRepository;
            this.logger = logger;
            this.context = context;
            this.imageService = imageService;
        }

        [HttpPost]
        [Authorize(Roles = RestrictedAccessRoles)]
        public ActionResult<Product> Post(Product input)
        {
            var product = new Product();
            product.Update(input);

            if (productRepository.Contains(product) is DuplicateProblemDetails err)
            {
                return new ConflictObjectResult(err);
            }
          
            productRepository.Create(product);

            return CreatedAtAction(
                nameof(ProductsController.Get), "products", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RestrictedAccessRoles)]
        public ActionResult<Product> Put(string id, Product input)
        {
            var product = productRepository.Get(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Update(input);

            if (productRepository.Contains(product) is DuplicateProblemDetails err)
            {
                return new ConflictObjectResult(err);
            }

            productRepository.Update(product);

            return product;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RestrictedAccessRoles)]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var game = productRepository.Get(id);
            if (game == null)
            {
                return NotFound();
            }

            productRepository.Delete(id);

            return Ok();
        }

        [HttpGet]
        public ActionResult<PagedList<Product>> GetAll(int page = 1, int pageSize = 10)
        {
            return productRepository.GetAll(page, pageSize);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            Product product = productRepository.Get(id);
            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost("{id}/image")]
        public async Task<ActionResult<Product>> PostImage(string id, [FromForm] ProductImageInput input)
        {
            logger.LogInformation($"PostImage({id})");

            Product product = productRepository.Get(id);

            if (product == null)
            {
                return NotFound();
            }

            try
            {
                product.ImageName = await imageService.SaveImage(input);
                productRepository.Update(product);
                return product;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to save images for product with id: {id}");
                return BadRequest();
            }
        }

    }
}
