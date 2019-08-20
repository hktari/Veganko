using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VegankoService.Models.User;

namespace VegankoService.Controllers
{
    //TODO: https://fullstackmark.com/post/13/jwt-authentication-with-aspnet-core-2-web-api-angular-5-net-core-identity-and-facebook-login
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IConfiguration configuration;

        public AccountController(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserInput model)
        {
            //ViewData["ReturnUrl"] = returnUrl;
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Description = model.Description,
                Label = model.Label,
                AvatarId = model.AvatarId,
                ProfileBackgroundId = model.ProfileBackgroundId,
                AccessRights = int.MaxValue, // puno right
            };

            IdentityResult result = await userManager.CreateAsync(user, model.PasswordHash);
            if (result.Succeeded)
            {
                logger.LogInformation("User created a new account with password.");

                //var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                await signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }

            // If we got this far, something failed, redisplay form
            return BadRequest(result.Errors);
        }

        //public async Task<IActionResult> SignIn()
        //{
        //        // This doesn't count login failures towards account lockout
        //        // To enable password failures to trigger account lockout, 
        //        // set lockoutOnFailure: true
        //        var result = await signInManager.PasswordSignInAsync(Input.Email,
        //            Input.Password, Input.RememberMe, lockoutOnFailure: true);
        //        if (result.Succeeded)
        //        {
        //            logger.LogInformation("User logged in.");
        //            return Ok();  // bearer Token ?
        //        }
        //        if (result.IsLockedOut)
        //        {
        //            logger.LogWarning("User account locked out.");
        //            return BadRequest();
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //            return BadRequest("Login failed");
        //        }
        
        //    // If we got this far, something failed, redisplay form
        //    return BadRequest();
        //}

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GenerateToken(
            string name = "aspnetcore-api-demo", bool admin = false)
        {
            var jwt = JwtTokenGenerator.Generate(
                name, admin,
                configuration["Tokens:Issuer"],
                configuration["Tokens:Key"]);

            return Ok(jwt);
        }
    }
}
