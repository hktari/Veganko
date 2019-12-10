using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using VegankoService.Data;
using VegankoService.Helpers;
using VegankoService.Models;
using VegankoService.Models.Products;

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

        public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
        {
            this.productRepository = productRepository;
            this.logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = RestrictedAccessRoles)]
        public ActionResult<Product> Post(ProductInput input)
        {
            var product = new Product();
            input.MapToProduct(product);
            productRepository.Create(product);

            return CreatedAtAction(
                nameof(ProductsController.Get), "products", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RestrictedAccessRoles)]
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
        [Authorize(Roles = RestrictedAccessRoles)]
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
        public ActionResult<PagedList<Product>> GetAll(int page = 1, int pageSize = 10)
        {
            return productRepository.GetAll(page, pageSize);
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

        [HttpPost("{id}/image")]
        public async Task<ActionResult<Product>> PostImage(string id, [FromForm] ProductImageInput input)
        {
            Product product = productRepository.Get(id);

            if (product == null)
            {
                return NotFound();
            }

            string trustedFileNameForStorage = Guid.NewGuid().ToString();

            if (!ValidateImage(input.DetailImage, "detail", trustedFileNameForStorage, out string detailImagePath)
                || !ValidateImage(input.ThumbImage, "thumb", trustedFileNameForStorage, out string thumbImagePath))
            {
                return BadRequest();
            }

            await SaveImage(input.DetailImage, detailImagePath);
            await SaveImage(input.ThumbImage, thumbImagePath);

            string thumbImageName = Path.GetFileName(thumbImagePath);
            string detailImageName = Path.GetFileName(detailImagePath);
            if (detailImageName != thumbImageName)
            {
                logger.LogError("Failed to update image for product with id: {id}. " +
                    "Thumb and detail image names don't match {detailImageName} != {thumbImageName}.",
                    product.Id, detailImageName, thumbImageName);
            }

            product.ImageName = detailImageName;
            productRepository.Update(product);

            return product;
        }

        private bool ValidateImage(IFormFile image, string folder, string trustedFileNameForStorage, out string targetFilePath)
        {
            targetFilePath = null;

            if (image.Length <= 0)
            {
                return false;
            }

            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                    image.FileName);
            double fileSize = image.Length / 1000000.0d;

            logger.LogDebug("Received image with name: {trustedFileNameForDisplay}, " +
                "size: {fileSize} MB.", trustedFileNameForDisplay, fileSize);

            string[] permittedExtensions = { ".jpg" };
            
            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                // The extension is invalid ... discontinue processing the file
                return false;
            }

            targetFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "images", folder, trustedFileNameForStorage + ext);
            return true;
        }

        private async Task SaveImage(IFormFile image, string targetFilePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

            using (var targetStream = System.IO.File.Create(targetFilePath))
            {
                await image.CopyToAsync(targetStream);

                logger.LogInformation("Uploaded file '{TrustedFileNameForDisplay}'", targetFilePath);
            }
        }
    }
}
