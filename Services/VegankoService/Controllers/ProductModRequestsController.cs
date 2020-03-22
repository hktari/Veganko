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
            return context.ProductModRequests.Include(pmr => pmr.Product);
        }

        // GET: api/ProductModRequests/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductModRequest([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productModRequest = await context.ProductModRequests.Include(pmr => pmr.Product)
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

            logger.LogDebug($"Updating product mod request with id: {id}");

            UnapprovedProduct product = await productRepository.GetUnapproved(productModRequest.Product.Id);
            if (product == null)
            {
                logger.LogError($"Failed to find unapproved product with id: {productModRequest.Product.Id}");
                return BadRequest();
            }

            product.Update(productModRequest.Product);
            await productRepository.UpdateUnapproved(product);

            logger.LogDebug($"Updated unapproved product with id: {product.Id}");

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

            if (productModRequest.Action == ProductModRequestAction.Edit)
            {
                if (productId != productModRequest.ExistingProductId)
                {
                    logger.LogWarning($"Product id and ExistingProductId don't match.");
                    return BadRequest();
                }

                logger.LogDebug($"Retrieving product with id: {productId} which is being edited.");
                Product product = productRepository.Get(productId);

                if (product == null)
                {
                    logger.LogDebug($"Product with id: {productId} not found.");
                    return BadRequest();
                }
            }

            // Check if the auid already exists
            if (productRepository.Contains(productModRequest.Product) is DuplicateProblemDetails err)
            {
                return new ConflictObjectResult(err);
            }

            await productRepository.CreateUnapproved(productModRequest.Product);

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

            // TODO: include
            var productModRequest = await context.ProductModRequests.FindAsync(id);
            if (productModRequest == null)
            {
                logger.LogDebug($"Product mod request with id: {id} not found.");
                return NotFound();
            }

            UnapprovedProduct product = await productRepository.GetUnapproved(productModRequest.Product.Id);
            if (product != null)
            {
                logger.LogDebug($"Removing unapproved product with id: {product.Id}");
                await productRepository.DeleteUnapproved(product);
            }
            else 
            {
                logger.LogWarning($"Failed to remove unapproved product with id: {productModRequest.Product.Id}. Product doesn't exist.");
            }

            logger.LogDebug($"Removing product mod request with id: {id}");
            
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