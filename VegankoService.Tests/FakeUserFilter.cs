using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VegankoService.Tests
{
    internal class FakeUserFilter : IAsyncActionFilter
    {
        public FakeUserFilter(string role)
        {
            Role = role;
        }

        public string Role { get; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, "Test user"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, Role),
            new Claim("id", "user_identity_id"),
        }));

            await next();
        }
    }
}
