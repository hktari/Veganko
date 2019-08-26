using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Data;
using VegankoService.Helpers;
using VegankoService.Models;
using VegankoService.Models.User;

namespace VegankoService.Controllers
{
    [Authorize(Policy = "ApiUser")]
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
            input.Email = input.Username = input.PasswordHash = null;

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

        // POST api/accounts
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AccountInput model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
            };

            var result = await userManager.CreateAsync(user, model.PasswordHash);

            if (!result.Succeeded)
                return new BadRequestObjectResult(result);

            var idResult = await userManager.AddToRoleAsync(user, Constants.Strings.Roles.Member);
            if (!idResult.Succeeded)
            {
                return new BadRequestObjectResult(idResult);
            }
            var customer = new Customer
            {
                IdentityId = user.Id,
                Description = model.Description,
                Label = model.Label,
                AvatarId = model.AvatarId,
                ProfileBackgroundId = model.ProfileBackgroundId,
                // AccessRights = int.MaxValue, // puno right 
            };

            await context.Customer.AddAsync(customer);
            await context.SaveChangesAsync();

            logger.LogDebug($"\nCreated user: " +
                $"\nId:\t{customer.IdentityId}" +
                $"\nUsername:\t{user.UserName}");
            return new OkObjectResult("Account created");
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

            // Paged customers
            IQueryable<Customer> customers = context.Customer
                .Skip(page * pageSize)
                .Take(pageSize);

            var customerProfiles =
               from customer in customers
               join appUser in context.Users on customer.IdentityId equals appUser.Id
               join userRole in context.UserRoles on customer.IdentityId equals userRole.UserId
               join role in context.Roles on userRole.RoleId equals role.Id
               select new CustomerProfile
               {
                   Id = customer.Id,
                   Username = appUser.UserName,
                   Email = appUser.Email,
                   AvatarId = customer.AvatarId,
                   Description = customer.Description,
                   Label = customer.Label,
                   ProfileBackgroundId = customer.ProfileBackgroundId,
                   Role = role.Name
               };

            return new PagedList<CustomerProfile>
            {
                Items = customerProfiles.ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = context.Customer.Count()
            };
        }

       
        //  TODO: change  role

        //[HttpGet("{id}")]
        //[Authorize]
        //public ActionResult<CustomerProfile> Get(string id)
        //{
        //    Customer customer = context.Customer.FirstOrDefault(c => c.Id == id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }
        //    userManager.GEtROl
        //    var customerProfile = new CustomerProfile(customer);
        //    customerProfile.Role =   context.UserRoles.Join()
        //        .First(idr => idr.UserId == Identity.GetUserIdentityId(HttpContext.User))
        //        .
        //}
    }
}
