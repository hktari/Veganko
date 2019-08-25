using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VegankoService.Data;
using VegankoService.Helpers;
using VegankoService.Models.User;

namespace VegankoService.Controllers
{
    //TODO: https://fullstackmark.com/post/13/jwt-authentication-with-aspnet-core-2-web-api-angular-5-net-core-identity-and-facebook-login
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<AccountController> logger;
        private readonly VegankoContext context;
        private readonly IConfiguration configuration;

        public AccountController(
            IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, ILogger<AccountController> logger, VegankoContext context)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.context = context;
        }

        [HttpPut("edit")]
        public async Task<ActionResult<CustomerProfile>> Edit([FromBody]AccountInput input)
        {
            // Can't be edited here
            input.Email = input.Username = input.PasswordHash = null;
            var customer = await Identity.CurrentCustomer(httpContextAccessor.HttpContext.User, context);

            customer.AvatarId = input.AvatarId;
            customer.ProfileBackgroundId = input.ProfileBackgroundId;
            customer.Label = input.Label;
            customer.Description = input.Description;

            context.Customer.Update(customer);
            context.SaveChanges();

            var customerProfile = new CustomerProfile(customer);
            var userID = await Identity.GetUserIdentity(httpContextAccessor.HttpContext.User, userManager);
            customerProfile.Role = (await userManager.GetRolesAsync(userID)).First();
            return customerProfile;
        }

        // POST api/accounts
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AccountInput model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
            };

            var result = await userManager.CreateAsync(user, model.PasswordHash );

            if (!result.Succeeded)
                return new BadRequestObjectResult(result);

            var idResult = await userManager.AddToRoleAsync(user, Constants.Strings.Roles.Member);
            if (!idResult.Succeeded)
            {
                return new BadRequestObjectResult(idResult);
            }

            await context.Customer.AddAsync(
                new Customer
                {
                    IdentityId = user.Id,
                    Description = model.Description,
                    Label = model.Label,
                    AvatarId = model.AvatarId,
                    ProfileBackgroundId = model.ProfileBackgroundId,
                    // AccessRights = int.MaxValue, // puno right 
                });
            await context.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }

    }
}
