using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Veganko.Common.Models.Products;
using VegankoService.Data;
using VegankoService.Data.ProductModRequests;
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
        private readonly IProductModRequestsRepository productModReqRepository;

        public ProductModRequestsController(
            VegankoContext context,
            IHttpContextAccessor httpContextAccessor,
            IProductRepository productRepository,
            ILogger<ProductModRequestsController> logger,
            IImageService imageService,
            IProductModRequestsRepository productModReqRepository)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.productRepository = productRepository;
            this.logger = logger;
            this.imageService = imageService;
            this.productModReqRepository = productModReqRepository;
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

            return Ok(productModReqRepository.GetAll(page, pageSize));
        }

        // GET: api/ProductModRequests/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductModRequest([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productModRequest = await productModReqRepository.Get(id);

            if (productModRequest == null)
            {
                return NotFound();
            }

            return Ok(productModRequest);
        }

        // PUT: api/ProductModRequests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductModRequest([FromRoute] string id, [FromBody] ProductModRequest input)
        {
            logger.LogInformation($"Executing PutProductModRequest({id})");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != input.Id)
            {
                return BadRequest();
            }

            ProductModRequest productModRequest = await productModReqRepository.Get(id);

            if(productModRequest == null)
            {
                logger.LogWarning("ProductModRequest not found");
                return NotFound();
            }

            productModRequest.Update(input);

            if (productModRequest.Action == ProductModRequestAction.Edit && string.IsNullOrEmpty(productModRequest.ChangedFields))
            {
                logger.LogWarning($"Change requests of action {ProductModRequestAction.Edit} require {nameof(ProductModRequest.ChangedFields)}");
                return BadRequest();
            }

            if (productModRequest.UnapprovedProduct == null)
            {
                logger.LogError($"Failed to find unapproved product with id: {input.UnapprovedProduct.Id}");
                return BadRequest();
            }

            Product product = new Product();
            productModRequest.UnapprovedProduct.MapToProduct(product, mapAllFields: true);

            // Check if the barcode already exists
            if (productRepository.Contains(product) is DuplicateProblemDetails err)
            {
                return new ConflictObjectResult(err);
            }

            // TODO: move to ProductModReqRepository ?
            await productRepository.UpdateUnapproved(productModRequest.UnapprovedProduct);

            logger.LogInformation($"Updated unapproved product with id: {productModRequest.UnapprovedProduct.Id}");

            productModRequest.Timestamp = DateTime.Now;

            await productModReqRepository.Update(productModRequest);

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

            await productModReqRepository.Create(productModRequest);

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

            var productModRequest = await productModReqRepository.Get(id);
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

            await productModReqRepository.Delete(productModRequest);

            if (productModRequest.UnapprovedProduct.ImageName != null)
            {
                imageService.DeleteImages(productModRequest.UnapprovedProduct.ImageName);
            }

            return Ok(productModRequest);
        }

        [HttpPost("{id}/image")]
        public async Task<ActionResult<ProductModRequest>> PostImage(string id, [FromForm] ProductImageInput input)
        {
            logger.LogInformation($"PostImage({id})");

            ProductModRequest prodRequest = await productModReqRepository.Get(id);

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

        // TODO: submit / accept mode request
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveProductModRequest([FromRoute] string id, [FromBody] ProductModRequest proModReqUpdate)
        {
            logger.LogInformation($"Executing ApproveProductModRequest({id}, productModRequest)");

            if (id != proModReqUpdate.Id)
            {
                logger.LogWarning("Route id and model id don't match");
                return BadRequest();
            }

            ProductModRequest productRequest = await productModReqRepository.Get(id);
            if (productRequest == null)
            {
                logger.LogWarning($"ProductModRequest not found");
                return NotFound();
            }

            productRequest.UnapprovedProduct.Update(proModReqUpdate.UnapprovedProduct);

            IActionResult result;
            ProductModRequestState? newState;

            Product product = null;
            if (productRequest.Action == ProductModRequestAction.Add)
            {
                product = new Product();
                productRequest.UnapprovedProduct.MapToProduct(product, mapAllFields: true);
                productRepository.Create(product);

                newState = ProductModRequestState.Approved;
                result = Ok(product);
            }
            else if (productRequest.Action == ProductModRequestAction.Edit)
            {
                product = productRepository.Get(productRequest.ExistingProductId);

                if (product == null)
                {
                    logger.LogError($"Product with id: {productRequest.ExistingProductId} not found. Might've been deleted.");

                    newState = ProductModRequestState.Missing;
                    result = new RequestError(
                        nameof(ProductModRequest.ExistingProductId), "Product to update was not found. It might've been deleted.")
                        .SetStatusCode((int)HttpStatusCode.BadRequest)
                        .ToActionResult();
                }
                else
                {
                    // Delete old images before updating the reference to them
                    if (product.ImageName != null && product.ImageName != productRequest.UnapprovedProduct.ImageName)
                    {
                        imageService.DeleteImages(product.ImageName);
                    }

                    productRequest.UnapprovedProduct.MapToProduct(product);
                    // ImageName is readonly and not being mapped by MapToProduct()
                    product.ImageName = productRequest.UnapprovedProduct.ImageName;

                    productRepository.Update(product);

                    newState = ProductModRequestState.Approved;
                    result = Ok(product);
                }
            }
            else
            {
                logger.LogError($"Unhandled product request action: {productRequest.Action}");
                throw new NotImplementedException($"Unhandled product request action: {productRequest.Action}");
            }

            // Add info regarding evaluation
            context.ProductModRequestEvaluations.Add(new ProductModRequestEvaluation
            {
                ProductModRequest = productRequest,
                EvaluatorUserId = Identity.GetUserIdentityId(httpContextAccessor.HttpContext.User),
                State = newState.Value,
                Timestamp = DateTime.Now,
            });

            productRequest.State = newState.Value;
            await productModReqRepository.Update(productRequest);

            return result;
        }

        [HttpGet("reject/{id}")]
        public async Task<IActionResult> RejectProductModRequest([FromRoute] string id)
        {
            logger.LogInformation($"RejectProductModRequest({id})");

            ProductModRequest productRequest = await productModReqRepository.Get(id);
            if (productRequest == null)
            {
                logger.LogWarning($"ProductModRequest not found");
                return NotFound();
            }

            productRequest.State = ProductModRequestState.Rejected;
            await productModReqRepository.Update(productRequest);
            context.ProductModRequestEvaluations.Add(new ProductModRequestEvaluation 
            {
                EvaluatorUserId = Identity.GetUserIdentityId(httpContextAccessor.HttpContext.User),
                ProductModRequest = productRequest,
                State = ProductModRequestState.Rejected,
                Timestamp = DateTime.Now,
            });

            return Ok(productRequest);
        }
    }
}