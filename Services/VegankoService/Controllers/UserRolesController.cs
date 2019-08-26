using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Data;
using VegankoService.Helpers;
using VegankoService.Models.User;

namespace VegankoService.Controllers
{
    [Authorize(Roles = Constants.Strings.Roles.Admin + ", " + Constants.Strings.Roles.Manager)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly VegankoContext context;

        public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, VegankoContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody]EditAccountRoleInput input, [FromQuery]EditUserRoleQueryParams queryParams)
        {
            if (!await roleManager.RoleExistsAsync(input.Role))
            {
                return BadRequest($"Role '{input.Role}' doesn't exist !");
            }

            // Prevent users from giving themselves admin rights
            if (input.Role == Constants.Strings.Roles.Admin)
            {
                return BadRequest();
            }

            Customer customer = context.Customer.FirstOrDefault(c => c.Id == queryParams.UserId);
            if (customer == null)
            {
                return NotFound("User not found!");
            }

            ApplicationUser userId = await userManager.FindByIdAsync(customer.IdentityId);
            IdentityResult clearRolesResult = await userManager.RemoveFromRolesAsync(userId, await userManager.GetRolesAsync(userId));
            if (!clearRolesResult.Succeeded)
            {
                return StatusCode(500, "Failed to clear user roles. " + GetErrorsAsString(clearRolesResult));
            }

            IdentityResult addRoleResult = await userManager.AddToRoleAsync(userId, input.Role);
            if (!addRoleResult.Succeeded)
            {
                return StatusCode(500, "Failed to add user to role. " + GetErrorsAsString(addRoleResult));
            }

            return Ok(input);
        }

        private static string GetErrorsAsString(IdentityResult idResult)
        {
            return idResult.Errors.Aggregate(string.Empty, (seed, str) => seed += str + ", ");
        }

        public class EditUserRoleQueryParams
        {
            [Required]
            public string UserId { get; set; }
        }

        public class EditAccountRoleInput
        {
            [Required]
            public string Role { get; set; }
        }

    }
}
