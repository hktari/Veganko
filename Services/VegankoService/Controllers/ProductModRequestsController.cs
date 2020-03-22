using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veganko.Common.Models.Products;
using VegankoService.Data;
using VegankoService.Models.ErrorHandling;

namespace VegankoService.Controllers
{
    [Route("api/product_requests")]
    [ApiController]
    public class ProductModRequestsController : ControllerBase
    {
        private readonly VegankoContext _context;
        private readonly IProductRepository productRepository;

        public ProductModRequestsController(VegankoContext context, IProductRepository productRepository)
        {
            _context = context;
            this.productRepository = productRepository;
        }

        // GET: api/ProductModRequests
        [HttpGet]
        public IEnumerable<ProductModRequest> GetProductModRequests()
        {
            return _context.ProductModRequests;
        }

        // GET: api/ProductModRequests/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductModRequest([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: product 
            var productModRequest = await _context.ProductModRequests.FindAsync(id);

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

            _context.Entry(productModRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            // Check if the auid already exists
            if (productRepository.Contains(productModRequest.Product) is DuplicateProblemDetails err)
            {
                return new ConflictObjectResult(err);
            }

            await productRepository.CreateUnapproved(productModRequest.Product);
            productModRequest.ProductId = productModRequest.Product.Id;

            _context.ProductModRequests.Add(productModRequest);
            await _context.SaveChangesAsync();

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

            var productModRequest = await _context.ProductModRequests. FindAsync(id);
            if (productModRequest == null)
            {
                return NotFound();
            }

            _context.ProductModRequests.Remove(productModRequest);
            await _context.SaveChangesAsync();

            return Ok(productModRequest);
        }

        private bool ProductModRequestExists(string id)
        {
            return _context.ProductModRequests.Any(e => e.Id == id);
        }
    }
}