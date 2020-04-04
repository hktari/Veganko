using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Users;
using VegankoService.Models;
using VegankoService.Models.User;

namespace VegankoService.Data.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly VegankoContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<UsersRepository> logger;

        public UsersRepository(VegankoContext context, UserManager<ApplicationUser> userManager, ILogger<UsersRepository> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
        }

        public UserPublicInfo GetProfile(string identityId)
        {
            IQueryable<UserPublicInfo> query =
               from customer in context.Customer
               where customer.IdentityId == identityId
               join appUser in context.Users on customer.IdentityId equals appUser.Id
               join userRole in context.UserRoles on customer.IdentityId equals userRole.UserId
               join role in context.Roles on userRole.RoleId equals role.Id
               select CreateCustomerProfile(customer, appUser, role);

            return query.FirstOrDefault();
        }

        public PagedList<UserPublicInfo> GetAll(UserQuery query)
        {
            IQueryable<Customer> customers = context.Customer;

            if (query.PageSize > 0)
            {
                customers = customers
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize);
            }

            IQueryable<UserPublicInfo> userProfiles =
                from customer in customers
                join appUser in context.Users on customer.IdentityId equals appUser.Id
                join userRole in context.UserRoles on customer.IdentityId equals userRole.UserId
                join role in context.Roles on userRole.RoleId equals role.Id
                select CreateCustomerProfile(customer, appUser, role);

            if (query.Role != null)
            {
                userProfiles = userProfiles.Where(cp => cp.Role == query.Role);
            }

            if (!string.IsNullOrEmpty(query.Name))
            {
                userProfiles = userProfiles.Where(cp => cp.Username.Contains(query.Name) || cp.Email.Contains(query.Name));
            }

            return new PagedList<UserPublicInfo>
            {
                Items = userProfiles.ToList(),
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = userProfiles.Count()
            };
        }

        public void Update(UserPublicInfo customerProfile)
        {
            Customer customer = context.Customer.FirstOrDefault(c => c.Id == customerProfile.Id);
            customerProfile.MapToCustomer(customer);
            context.Customer.Update(customer);
            context.SaveChanges();
        }

        public Customer GetByIdentityId(string identityId)
        {
            return context.Customer.FirstOrDefault(c => c.IdentityId == identityId);
        }

        public Customer Get(string id)
        {
            return context.Customer.FirstOrDefault(c => c.Id == id);
        }

        public async Task Delete(string identityId)
        {
            var customer = context.Customer.FirstOrDefault(c => c.IdentityId == identityId);
            if (customer != null)
            {
                context.Customer.Remove(customer);
            }

            var user = await userManager.FindByIdAsync(identityId);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to delete user."
                        + result.Errors?.Aggregate("", (str, err) => str += $"{err.Code}: {err.Description}, "));
                }
            }

            await context.SaveChangesAsync();
        }

        private UserPublicInfo CreateCustomerProfile(Customer customer, ApplicationUser appUser, IdentityRole role)
        {
            return new UserPublicInfo
            {
                Id = customer.Id,
                Username = appUser.UserName,
                Email = appUser.Email,
                AvatarId = customer.AvatarId,
                Description = customer.Description,
                Label = customer.Label,
                ProfileBackgroundId = customer.ProfileBackgroundId,
                Role = ParseRoleName(role.Name),
                IsEmailConfirmed = appUser.EmailConfirmed
            };
        }

        private UserRole ParseRoleName(string roleName)
        {
            if (!Enum.TryParse<UserRole>(roleName, out UserRole userRole))
            {
                logger.LogWarning($"Failed to parse user role name: {roleName} to {typeof(UserRole)}. Using default value.");
            }

            return userRole;
        }
    }
}
