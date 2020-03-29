using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VegankoService.Data;
using VegankoService.Data.Stores;
using VegankoService.Helpers;
using VegankoService.Models.Stores;

namespace VegankoService.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        // Every authorized user can add stores. Only moderators can delete.
        private const string RestrictedAccessRoles = Constants.Strings.Roles.Admin + ", " + Constants.Strings.Roles.Manager + ", " + Constants.Strings.Roles.Moderator;
        private readonly IStoresRepository storesRepository;
        private readonly IProductRepository productRepository;

        public StoresController(IStoresRepository storesRepository, IProductRepository productRepository)
        {
            this.storesRepository = storesRepository;
            this.productRepository = productRepository;
        }

        // GET: api/Stores
        [HttpGet]
        public IEnumerable<Store> GetAllStores([FromQuery] string productId)
        {
            return storesRepository.GetAll(productId);
        }

        // GET: api/Stores/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStore([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var store = await storesRepository.Get(id);

            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        // PUT: api/Stores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore([FromRoute] string id, [FromBody] Store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != store.Id)
            {
                return BadRequest();
            }

            try
            {
                await storesRepository.Update(store);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(id))
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

        // POST: api/Stores
        [HttpPost]
        public async Task<IActionResult> PostStore([FromBody] Store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (productRepository.Get(store.ProductId) == null && productRepository.GetUnapproved(store.ProductId) == null)
            {
                return BadRequest("Product not found.");
            }

            await storesRepository.Create(store);
            
            return CreatedAtAction("GetStore", new { id = store.Id }, store);
        }

        // DELETE: api/Stores/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RestrictedAccessRoles)]
        public async Task<IActionResult> DeleteStore([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var store = await storesRepository.Get(id);
            if (store == null)
            {
                return NotFound();
            }

            await storesRepository.Delete(store);
            
            return Ok();
        }

        private bool StoreExists(string id)
        {
            return storesRepository.Get(id) != null;
        }
    }
}