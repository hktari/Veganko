using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    [Route("api/product_requests")]
    [ApiController]
    public class ProductModRequestsController : ControllerBase
    {
        private readonly VegankoContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IProductRepository productRepository;
        private readonly ILogger<ProductModRequestsController> logger;
        private readonly IImageService imageService;

        public ProductModRequestsController(
            VegankoContext context,
            IHttpContextAccessor httpContextAccessor,
            IProductRepository productRepository,
            ILogger<ProductModRequestsController> logger,
            IImageService imageService)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.productRepository = productRepository;
            this.logger = logger;
            this.imageService = imageService;
        }

        // GET: api/ProductModRequests
        [HttpGet]
        public ActionResult<PagedList<ProductModRequest>> GetProductModRequests(int page = 1, int pageSize = 10)
        {
            logger.LogInformation($"GetProductModRequest({page}, {pageSize})");

            int pageIdx = page - 1;
            if (pageIdx < 0)
            {
                logger.LogError($"Page index is less 1");
                return BadRequest();
            }

            return Ok(new PagedList<ProductModRequest>
            {
                Items = context.ProductModRequests.Include(pmr => pmr.UnapprovedProduct),
                Page = page,
                PageSize = pageSize,
                TotalCount = context.ProductModRequests.Count(),
            });
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
            logger.LogInformation($"Executing PutProductModRequest({id})");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productModRequest.Id)
            {
                return BadRequest();
            }

            if (productModRequest.Action == ProductModRequestAction.Edit && string.IsNullOrEmpty(productModRequest.ChangedFields))
            {
                logger.LogWarning($"Change requests of action {ProductModRequestAction.Edit} require {nameof(ProductModRequest.ChangedFields)}");
                return BadRequest();
            }

            UnapprovedProduct unapprovedProduct = await productRepository.GetUnapproved(productModRequest.UnapprovedProduct.Id);
            if (unapprovedProduct == null)
            {
                logger.LogError($"Failed to find unapproved product with id: {productModRequest.UnapprovedProduct.Id}");
                return BadRequest();
            }

            Product product = new Product();
            productModRequest.UnapprovedProduct.MapToProduct(product);

            // Check if the auid already exists
            if (productRepository.Contains(product) is DuplicateProblemDetails err)
            {
                return new ConflictObjectResult(err);
            }

            productModRequest.UnapprovedProduct.Update(unapprovedProduct);
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
        public async Task<IActionResult> PostProductModRequest([FromBody] ProductModRequest productModRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            productModRequest.Action = productModRequest.ExistingProductId != null ? ProductModRequestAction.Edit : ProductModRequestAction.Add;

            Product product = new Product();

            if (productModRequest.Action == ProductModRequestAction.Edit)
            {
                if (string.IsNullOrEmpty(productModRequest.ChangedFields))
                {
                    logger.LogWarning($"Field {nameof(ProductModRequest.ChangedFields)} is required when action is {nameof(ProductModRequestAction.Edit)}");
                    return BadRequest();
                }

                logger.LogInformation($"Retrieving product with id: {productModRequest.ExistingProductId} which is being edited.");
                product = productRepository.Get(productModRequest.ExistingProductId);

                if (product == null)
                {
                    logger.LogInformation($"Product with id: {productModRequest.ExistingProductId} not found.");
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

            string userId = Identity.GetUserIdentityId(httpContextAccessor.HttpContext.User);
            if (productModRequest.UserId != userId
                && Identity.GetRole(httpContextAccessor.HttpContext.User) == Constants.Strings.Roles.Member)
            {
                logger.LogWarning($"Member user {userId} doesn't have rights to delete a product mod request {id}, which he didn't create.");
                return Forbid();
            }

            logger.LogInformation($"Removing product mod request with id: {id}");

            context.ProductModRequests.Remove(productModRequest);
            await context.SaveChangesAsync();

            imageService.DeleteImages(productModRequest.UnapprovedProduct.ImageName);

            return Ok(productModRequest);
        }

        [HttpPost("{id}/image")]
        public async Task<ActionResult<ProductModRequest>> PostImage(string id, [FromForm] ProductImageInput input)
        {
            logger.LogInformation($"PostImage({id})");

            ProductModRequest prodRequest = await context.ProductModRequests
                .Include(pmr => pmr.UnapprovedProduct)
                .FirstOrDefaultAsync(pmr => pmr.Id == id);

            if (prodRequest == null)
            {
                logger.LogWarning($"ProductModRequest not found");
                return NotFound();
            }

            // TODO: test for realz
            string role = Identity.GetRole(httpContextAccessor.HttpContext.User);
            string userId = Identity.GetUserIdentityId(httpContextAccessor.HttpContext.User);

            // If the user is not the author and not inside a privileged role
            if (!Roles.IsInsideRestrictedAccessRoles(role) && prodRequest.UserId != userId)
            {
                logger.LogWarning($"User {userId} forbidden from posting image");
                return Forbid();
            }

            try
            {
                string newImageName = await imageService.SaveImage(input);
                
                if (prodRequest.UnapprovedProduct.ImageName != null)
                {
                    imageService.DeleteImages(prodRequest.UnapprovedProduct.ImageName);
                }

                prodRequest.UnapprovedProduct.ImageName = newImageName;
                await productRepository.UpdateUnapproved(prodRequest.UnapprovedProduct);
                return prodRequest;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to save images for product with id: {id}");
                return BadRequest();
            }
        }

        private bool ProductModRequestExists(string id)
        {
            return context.ProductModRequests.Any(e => e.Id == id);
        }
    }
}