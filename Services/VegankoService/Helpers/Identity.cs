using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VegankoService.Data;
using VegankoService.Models.User;

namespace VegankoService.Helpers
{
    public static class Identity
    {
        public static string GetUserIdentityId(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.Single(c => c.Type == "id").Value;
        }

        public static string GetRole(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.Single(c => c.Type == ClaimTypes.Role).Value;
        }

        public static Task<ApplicationUser> GetUserIdentity(ClaimsPrincipal claimsPrincipal, UserManager<ApplicationUser> userManager)
        {
            return userManager.FindByIdAsync(
                GetUserIdentityId(claimsPrincipal));
        }

        public static Task<Customer> CurrentCustomer(ClaimsPrincipal claimsPrincipal, VegankoContext context)
        {
            var userId = GetUserIdentityId(claimsPrincipal);
            return context.Customer.Include(c => c.Identity).SingleAsync(c => c.Identity.Id == userId);
        }

    }
}
