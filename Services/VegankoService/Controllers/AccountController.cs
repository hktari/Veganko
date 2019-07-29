using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VegankoService.Models.User;

namespace VegankoService.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
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
    }
}
