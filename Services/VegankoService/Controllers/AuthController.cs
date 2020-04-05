

using System.Security.Claims;
using System.Threading.Tasks;
using VegankoService.Auth;
using VegankoService.Helpers;
using VegankoService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VegankoService.Models.User;
using System.Collections.Generic;
using VegankoService.Models.Auth;
using VegankoService.Data.Users;
using Microsoft.Extensions.Logging;
using VegankoService.Models.ErrorHandling;
using Veganko.Common.Models.Auth;
using Veganko.Common.Models.Users;

namespace VegankoService.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly IUsersRepository usersRepository;
        private readonly ILogger<AuthController> logger;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUsersRepository usersRepository,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            this.roleManager = roleManager;
            _jwtFactory = jwtFactory;
            this.usersRepository = usersRepository;
            this.logger = logger;
            _jwtOptions = jwtOptions.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]CredentialsInput credentials)
        {
            if (string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password))
            {
                return LoginFailed(AuthErrorCode.InvalidCredentials);
            }

            // get the user to verifty
            var userToVerify = await _userManager.FindByEmailAsync(credentials.Email);

            if (userToVerify == null)
            {
                return LoginFailed(AuthErrorCode.InvalidCredentials);
            }

            if (!await _userManager.CheckPasswordAsync(userToVerify, credentials.Password))
            {
                return LoginFailed(AuthErrorCode.InvalidCredentials);
            }

            // check the credentials
            if (!userToVerify.EmailConfirmed)
            {
                return LoginFailed(AuthErrorCode.UnconfirmedEmail);
            }

            IList<string> roles = await _userManager.GetRolesAsync(userToVerify);
            ClaimsIdentity identity = _jwtFactory.GenerateClaimsIdentity(credentials.Email, userToVerify.Id, roles);

            UserPublicInfo customerProfile = usersRepository.GetProfileByIdentityId(userToVerify.Id);
            if (customerProfile == null)
            {
                logger.LogError($"Customer profile not found in database: {userToVerify.Id}");
                return LoginFailed(AuthErrorCode.UserProfileNotFound);
            }

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.Email, _jwtOptions);
            return Ok(
                new LoginResponse
                {
                    Profile = customerProfile,
                    Token = jwt
                });
        }

        private BadRequestObjectResult LoginFailed(AuthErrorCode errCode)
        {
            var requestErr = new RequestError()
                .SetStatusCode((int)errCode);

            switch (errCode)
            {
                case AuthErrorCode.InvalidCredentials:
                    requestErr.Add(nameof(CredentialsInput.Email), "Invalid Credentials");
                    requestErr.Add(nameof(CredentialsInput.Password), "Invalid Credentials");
                    break;
                case AuthErrorCode.UnconfirmedEmail:
                    requestErr.Add(nameof(CredentialsInput.Email), "Unconfirmed Email");
                    break;
                default:
                    break;
            }

            return BadRequest(
                new LoginResponse
                {
                    RequestError = requestErr.ToValidProblemDetails()
                });
        }
    }
}
