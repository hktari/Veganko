using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Veganko.Common.Models.Products;
using VegankoService.Data;
using VegankoService.Helpers;
using VegankoService.Models.ErrorHandling;

namespace VegankoService.Controllers
{
    [Route("api/product_requests")]
    [ApiController]
    public class ProductModRequestsController : ControllerBase
    {
        private readonly VegankoContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IProductRepository productRepository;
        private readonly ILogger<ProductModRequestsController> logger;

        public ProductModRequestsController(
            VegankoContext context,
            IHttpContextAccessor httpContextAccessor,
            IProductRepository productRepository,
            ILogger<ProductModRequestsController> logger)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.productRepository = productRepository;
            this.logger = logger;
        }

        // GET: api/ProductModRequests
        [HttpGet]
        public IEnumerable<ProductModRequest> GetProductModRequests()
        {
            return context.ProductModRequests.Include(pmr => pmr.UnapprovedProduct);
        }

        // GET: api/ProductModRequests/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductModRequest([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productModRequest = await context.ProductModRequests.Include(pmr => pmr.UnapprovedProduct)
                                                                    .FirstOrDefaultAsync(pmr => pmr.Id == id);

            if (productModRequest == null)
            {
                return NotFound();
            }

            return Ok(productModRequest);
        }

        // PUT: api/ProductModRequests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductModRequest([FromRoute] string id, [FromBody] ProductModRequest productModRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productModRequest.Id)
            {
                return BadRequest();
            }

            logger.LogInformation($"Updating product mod request with id: {id}");

            UnapprovedProduct unapprovedProduct = await productRepository.GetUnapproved(productModRequest.UnapprovedProduct.Id);
            if (unapprovedProduct == null)
            {
                logger.LogError($"Failed to find unapproved product with id: {productModRequest.UnapprovedProduct.Id}");
                return BadRequest();
            }

            unapprovedProduct.Update(productModRequest.UnapprovedProduct);
            await productRepository.UpdateUnapproved(unapprovedProduct);

            logger.LogInformation($"Updated unapproved product with id: {unapprovedProduct.Id}");

            productModRequest.Timestamp = DateTime.Now;

            context.Entry(productModRequest).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductModRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductModRequests
        [HttpPost]
        public async Task<IActionResult> PostProductModRequest([FromQuery] string productId, [FromBody] ProductModRequest productModRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            productModRequest.Action = productId != null ? ProductModRequestAction.Edit : ProductModRequestAction.Add;
            
            Product product = new Product();

            if (productModRequest.Action == ProductModRequestAction.Edit)
            {
                if (productId != productModRequest.ExistingProductId)
                {
                    logger.LogWarning($"Product id and ExistingProductId don't match.");
                    return BadRequest();
                }

                logger.LogInformation($"Retrieving product with id: {productId} which is being edited.");
                product = productRepository.Get(productId);

                if (product == null)
                {
                    logger.LogInformation($"Product with id: {productId} not found.");
                    return BadRequest();
                }
            }

            productModRequest.UnapprovedProduct.MapToProduct(product);

            // Check if the auid already exists
            if (productRepository.Contains(product) is DuplicateProblemDetails err)
            {
                return new ConflictObjectResult(err);
            }

            await productRepository.CreateUnapproved(productModRequest.UnapprovedProduct);

            productModRequest.UserId = Identity.GetUserIdentityId(httpContextAccessor.HttpContext.User);
            productModRequest.Timestamp = DateTime.Now;

            context.ProductModRequests.Add(productModRequest);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetProductModRequest", new { id = productModRequest.Id }, productModRequest);
        }

        // DELETE: api/ProductModRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductModRequest([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productModRequest = await context.ProductModRequests.Include(pmr => pmr.UnapprovedProduct)
                                                                    .FirstOrDefaultAsync(pmr => pmr.Id == id);
            if (productModRequest == null)
            {
                logger.LogWarning($"Product mod request with id: {id} not found.");
                return NotFound();
            }

            UnapprovedProduct product = await productRepository.GetUnapproved(productModRequest.UnapprovedProduct.Id);
            if (product != null)
            {
                logger.LogInformation($"Removing unapproved product with id: {product.Id}");
                await productRepository.DeleteUnapproved(product);
            }
            else 
            {
                logger.LogWarning($"Failed to remove unapproved product with id: {productModRequest.UnapprovedProduct.Id}. Product doesn't exist.");
            }

            logger.LogInformation($"Removing product mod request with id: {id}");
            
            context.ProductModRequests.Remove(productModRequest);
            await context.SaveChangesAsync();
            
            return Ok(productModRequest);
        }

        private bool ProductModRequestExists(string id)
        {
            return context.ProductModRequests.Any(e => e.Id == id);
        }
    }
}