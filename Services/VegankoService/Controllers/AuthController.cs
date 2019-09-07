
 
using System.Security.Claims;
using System.Threading.Tasks;
using VegankoService.Auth;
using VegankoService.Helpers;
using VegankoService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VegankoService.Models.User;
using System.Collections.Generic;

namespace VegankoService.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            this.roleManager = roleManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]CredentialsInput credentials)
        {
            if (string.IsNullOrEmpty(credentials.UserName) || string.IsNullOrEmpty(credentials.Password))
                return LoginFailed();

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(credentials.UserName);

            if (userToVerify == null)
                return LoginFailed();

            if (!await _userManager.CheckPasswordAsync(userToVerify, credentials.Password))
            {
                return LoginFailed();
            }

            // check the credentials
            if (!userToVerify.EmailConfirmed)
                return BadRequest("Email not confirmed.");

            IList<string> roles = await _userManager.GetRolesAsync(userToVerify);
            ClaimsIdentity identity = _jwtFactory.GenerateClaimsIdentity(credentials.UserName, userToVerify.Id, roles);

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return new OkObjectResult(jwt);
        }

        private BadRequestObjectResult LoginFailed()
        {
            return BadRequest("Invalid username or password.");
        }
    }
}
