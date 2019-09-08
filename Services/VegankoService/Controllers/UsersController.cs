using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Data;
using VegankoService.Data.Users;
using VegankoService.Helpers;
using VegankoService.Models;
using VegankoService.Models.User;

namespace VegankoService.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [Route("api/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<AccountController> logger;
        private readonly VegankoContext context;
        private readonly IConfiguration configuration;
        private readonly IUsersRepository usersRepository;

        public UsersController(
            IConfiguration configuration,
            IUsersRepository usersRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AccountController> logger,
            VegankoContext context)
        {
            this.configuration = configuration;
            this.usersRepository = usersRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.context = context;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerProfile>> Edit(string id, [FromBody]AccountInput input)
        {
            var customer = context.Customer.FirstOrDefault(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            if (customer.IdentityId != Identity.GetUserIdentityId(httpContextAccessor.HttpContext.User))
            {
                return Forbid();
            }

            // Can't be edited here
            input.Email = input.Username = input.Password = null;

            customer.AvatarId = input.AvatarId;
            customer.ProfileBackgroundId = input.ProfileBackgroundId;
            customer.Label = input.Label;
            customer.Description = input.Description;

            context.Customer.Update(customer);
            context.SaveChanges();

            var customerProfile = new CustomerProfile(customer);
            var userID = await userManager.FindByIdAsync(customer.IdentityId);
            customerProfile.Role = (await userManager.GetRolesAsync(userID)).First();
            return customerProfile;
        }

        [Authorize(Roles = Constants.Strings.Roles.Admin + ", " + Constants.Strings.Roles.Manager)]
        [HttpGet]
        public ActionResult<PagedList<CustomerProfile>> GetAll(int page = 1, int pageSize = 20)
        {
            page--;
            if (page < 0)
            {
                return BadRequest("Pages start with index 1");
            }

            return usersRepository.GetAll(page, pageSize);
        }
    }
}
