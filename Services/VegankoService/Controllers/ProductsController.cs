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
using static VegankoService.Helpers.Constants.Strings;

namespace VegankoService.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : Controller
    {
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
        [Authorize(Roles = Roles.RestrictedAccessRoles)]
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
        [Authorize(Roles = Roles.RestrictedAccessRoles)]
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
        [Authorize(Roles = Roles.RestrictedAccessRoles)]
        public IActionResult Delete(string id)
        {
            logger.LogInformation($"Delete({id})");

            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var product = productRepository.Get(id);
            if (product == null)
            {
                logger.LogWarning($"Product not found");
                return NotFound();
            }

            productRepository.Delete(id);

            if (product.ImageName != null)
            {
                logger.LogInformation($"Deleting images");
                imageService.DeleteImages(product.ImageName);
            }

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
        [Authorize(Roles = Roles.RestrictedAccessRoles)]
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
                string newImageName = await imageService.SaveImage(input);
                if (product.ImageName != null)
                {
                    imageService.DeleteImages(product.ImageName);
                }
                
                product.ImageName = newImageName;
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
