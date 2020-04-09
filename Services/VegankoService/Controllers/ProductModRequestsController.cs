using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Veganko.Common.Models.Products;
using VegankoService.Data;
using VegankoService.Data.ProductModRequests;
using VegankoService.Data.Users;
using VegankoService.Helpers;
using VegankoService.Models;
using VegankoService.Models.ErrorHandling;
using VegankoService.Models.Products;
using VegankoService.Models.User;
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
        private readonly IUsersRepository usersRepository;
        private readonly IProductModRequestsRepository productModReqRepository;

        public ProductModRequestsController(
            VegankoContext context,
            IHttpContextAccessor httpContextAccessor,
            IProductRepository productRepository,
            ILogger<ProductModRequestsController> logger,
            IImageService imageService,
            IUsersRepository usersRepository,
            IProductModRequestsRepository productModReqRepository)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.productRepository = productRepository;
            this.logger = logger;
            this.imageService = imageService;
            this.usersRepository = usersRepository;
            this.productModReqRepository = productModReqRepository;
        }

        // GET: api/ProductModRequests
        [HttpGet]
        public ActionResult<PagedList<ProductModRequestDTO>> GetProductModRequests(int page = 1, int pageSize = 10, string userId = null, ProductModRequestState? state = null)
        {
            logger.LogInformation($"GetProductModRequest({page}, {pageSize}, {userId}, {state})");

            int pageIdx = page - 1;
            if (pageIdx < 0)
            {
                logger.LogError($"Page index is less 1");
                return BadRequest();
            }

            var query = new ProductModReqQuery(state, null, page, pageSize);
            if (userId != null)
            {
                // Get IdentityID
                Customer customer = usersRepository.Get(userId);
                if (customer == null)
                {
                    logger.LogWarning($"Failed to find user identity id with customer id: {userId}");
                    return NotFound();
                }
                query.UserId = customer.IdentityId;
            }

            PagedList<ProductModRequest> result = productModReqRepository.GetAll(query);
            logger.LogDebug($"Total count: {result.TotalCount} {result.Items.Count()}, page: {result.Page}, pageSize: {result.PageSize}");
            return Ok(result);
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
        public async Task<IActionResult> PutProductModRequest([FromRoute] string id, [FromBody] ProductModRequestDTO input)
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

            ProductModRequest inputAsModel = input.MapToModel();

            ProductModRequest productModRequest = await productModReqRepository.Get(id);

            if (productModRequest == null)
            {
                logger.LogWarning("ProductModRequestDTO not found");
                return NotFound();
            }

            productModRequest.Update(inputAsModel);

            if (productModRequest.Action == ProductModRequestAction.Edit && string.IsNullOrEmpty(productModRequest.ChangedFields))
            {
                logger.LogWarning($"Change requests of action {ProductModRequestAction.Edit} require {nameof(ProductModRequestDTO.ChangedFields)}");
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

            return Ok(productModRequest);
        }

        // POST: api/ProductModRequests
        [HttpPost]
        public async Task<IActionResult> PostProductModRequest([FromBody] ProductModRequestDTO input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProductModRequest inputAsModel = input.MapToModel();
            inputAsModel.UnapprovedProduct.Id = null; // An existing entry is being posted here. Set Id to null to avoid conflicts.

            inputAsModel.Action = inputAsModel.ExistingProductId != null ? ProductModRequestAction.Edit : ProductModRequestAction.Add;

            Product product = new Product();

            if (inputAsModel.Action == ProductModRequestAction.Edit)
            {
                if (string.IsNullOrEmpty(inputAsModel.ChangedFields))
                {
                    logger.LogWarning($"Field {nameof(ProductModRequestDTO.ChangedFields)} is required when action is {nameof(ProductModRequestAction.Edit)}");
                    return BadRequest();
                }

                logger.LogInformation($"Retrieving product with id: {inputAsModel.ExistingProductId} which is being edited.");
                product = productRepository.Get(inputAsModel.ExistingProductId);

                if (product == null)
                {
                    logger.LogInformation($"Product with id: {inputAsModel.ExistingProductId} not found.");
                    return BadRequest();
                }
            }

            inputAsModel.UnapprovedProduct.MapToProduct(product);

            // Check if the auid already exists
            if (productRepository.Contains(product) is DuplicateProblemDetails err)
            {
                return new ConflictObjectResult(err);
            }

            await productRepository.CreateUnapproved(inputAsModel.UnapprovedProduct);

            inputAsModel.UserId = Identity.GetUserIdentityId(httpContextAccessor.HttpContext.User);
            inputAsModel.Timestamp = DateTime.Now;

            await productModReqRepository.Create(inputAsModel);

            logger.LogInformation($"Created product mod request for user: {inputAsModel.UserId}");

            return CreatedAtAction("GetProductModRequest", new { id = inputAsModel.Id }, inputAsModel);
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

            return Ok();
        }

        [HttpPost("{id}/image")]
        public async Task<ActionResult<ProductModRequestDTO>> PostImage(string id, [FromForm] ProductImageInput input)
        {
            logger.LogInformation($"PostImage({id})");

            ProductModRequest prodRequest = await productModReqRepository.Get(id);

            if (prodRequest == null)
            {
                logger.LogWarning($"ProductModRequestDTO not found");
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
                return new ProductModRequestDTO(prodRequest);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to save images for product with id: {id}");
                return BadRequest();
            }
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveProductModRequest([FromRoute] string id, [FromBody] ProductModRequestDTO input)
        {
            // TODO: move conflict handling to ProductRepository


            logger.LogInformation($"Executing ApproveProductModRequest({id}, productModRequest)");

            if (id != input.Id)
            {
                logger.LogWarning("Route id and model id don't match");
                return BadRequest();
            }

            ProductModRequest inputAsModel = input.MapToModel();
            ProductModRequest productRequest = await productModReqRepository.Get(id);
            if (productRequest == null)
            {
                logger.LogWarning($"ProductModRequestDTO not found");
                return NotFound();
            }

            // ProductModRequest was already approved
            if (productRequest.State == ProductModRequestState.Approved)
            {
                logger.LogInformation("ProductModRequest has already been approved");
                return Ok(productRequest);
            }

            Customer user = usersRepository.GetByIdentityId(Identity.GetUserIdentityId(httpContextAccessor.HttpContext.User));

            if (user == null)
            {
                logger.LogError("Failed to get user id");
                return BadRequest();
            }

            productRequest.UnapprovedProduct.Update(inputAsModel.UnapprovedProduct);

            ProductModRequestState? newState;
            Product product;
            IActionResult result;
            if (productRequest.Action == ProductModRequestAction.Add)
            {
                product = new Product();
                productRequest.UnapprovedProduct.MapToProduct(product, mapAllFields: true);

                if (productRepository.Contains(product) is DuplicateProblemDetails err)
                {
                    logger.LogInformation($"Add product conflicts: {string.Join(", ", err.ConflictingFields ?? new string[] { })}");
                    return Conflict(err);
                }
                else
                {
                    productRepository.Create(product);

                    newState = ProductModRequestState.Approved;
                    productRequest.NewlyCreatedProductId = product.Id;
                    result = Ok(productRequest);
                }
            }
            else if (productRequest.Action == ProductModRequestAction.Edit)
            {
                product = productRepository.Get(productRequest.ExistingProductId);
                
                if (product == null)
                {
                    logger.LogError($"Product with id: {productRequest.ExistingProductId} not found. Might've been deleted.");

                    newState = ProductModRequestState.Missing;
                    result = new RequestError(
                        nameof(ProductModRequestDTO.ExistingProductId), "Product to update was not found. It might've been deleted.")
                        .SetStatusCode((int)HttpStatusCode.BadRequest)
                        .ToActionResult();
                }
                else
                {
                    productRequest.UnapprovedProduct.MapToProduct(product);

                    if (productRepository.Contains(product) is DuplicateProblemDetails err)
                    {
                        logger.LogInformation($"Edit product conflicts: {string.Join(", ", err.ConflictingFields ?? new string[] { })}");
                        return Conflict(err);
                    }
                    else
                    {
                        // Delete old images before updating the reference to them
                        if (product.ImageName != null && product.ImageName != productRequest.UnapprovedProduct.ImageName)
                        {
                            imageService.DeleteImages(product.ImageName);
                        }

                        // ImageName is readonly and not being mapped by MapToProduct()
                        product.ImageName = productRequest.UnapprovedProduct.ImageName;

                        productRepository.Update(product);
                        newState = ProductModRequestState.Approved;
                        result = Ok(productRequest);
                    }
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
                EvaluatorUserId = user.Id,
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
                logger.LogWarning($"ProductModRequestDTO not found");
                return NotFound();
            }

            if (productRequest.State != ProductModRequestState.Pending)
            {
                logger.LogInformation($"ProductModRequest has state {productRequest.State}. Only pending requests can be rejected.");
                return Ok(productRequest);
            }

            Customer user = usersRepository.GetByIdentityId(Identity.GetUserIdentityId(httpContextAccessor.HttpContext.User));
            if (user == null)
            {
                logger.LogWarning("Failed to retrieve logged in customer data.");
                return Unauthorized();
            }

            productRequest.State = ProductModRequestState.Rejected;
            await productModReqRepository.Update(productRequest);
            context.ProductModRequestEvaluations.Add(new ProductModRequestEvaluation
            {
                EvaluatorUserId = user.Id,
                ProductModRequest = productRequest,
                State = ProductModRequestState.Rejected,
                Timestamp = DateTime.Now,
            });

            return Ok(productRequest);
        }
    }
}