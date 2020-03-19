using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using VegankoService.Data;
using VegankoService.Helpers;
using VegankoService.Models.Account;
using VegankoService.Models.ErrorHandling;
using VegankoService.Models.User;
using VegankoService.Services;

namespace VegankoService.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private static Random r = new Random();

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<AccountController> logger;
        private readonly VegankoContext context;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;

        public AccountController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AccountController> logger,
            VegankoContext context,
            IEmailService emailService)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.context = context;
            this.emailService = emailService;
        }

        // POST api/accounts
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody]AccountInput input)
        {
            // UserManager takes   care  of  trimming,  but  we want  a  valid  email   address for  sending  email confirmation
            input.Email = input.Email.Trim();
            input.Username = input.Username.Trim();

            MailAddress parsedEmail = ParseEmail(input.Email);

            if (parsedEmail == null)
            {
                return new RequestError("InvalidEmail", "Email ni pravilen").ToActionResult();
            }

            if (!emailService.IsEmailProviderSupported(input.Email))
            {
                string errorMsg = $"Nepodprt email ponudnik. Trenutno so podprti: " +
                    EmailService.KnownEmailProviders.Select(emp => emp.Host)
                                                    .Aggregate(string.Empty, (str, email) => str += $"{email}, ");
                return new RequestError(nameof(input.Email), errorMsg)
                    .ToActionResult();
            }

            ApplicationUser user = await userManager.FindByEmailAsync(input.Email);

            // Delete user if another registration is made with the same email (probably confirmation mail not received)
            if (user != null && !user.EmailConfirmed)
            {
                var delResult = await userManager.DeleteAsync(user);
                if (!delResult.Succeeded)
                {
                    logger.LogError($"Failed to delete user with unconfirmed email. UserId: {user.Id}, email: {user.Email}.");
                    return HandleIdentityErr(delResult);
                }

                logger.LogInformation($"Deleted user with unconfirmed email on account creation. UserId: {user.Id}, email: {user.Email}.");
            }

            user = new ApplicationUser
            {
                UserName = input.Username,
                Email = input.Email,
            };

            var result = await userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
            {
                return HandleIdentityErr(result);
            }

#if REGISTER_AS_MODERATORS
            var idResult = await userManager.AddToRoleAsync(user, Constants.Strings.Roles.Moderator);
#else
            var idResult = await userManager.AddToRoleAsync(user, Constants.Strings.Roles.Member);
#endif
            if (!idResult.Succeeded)
            {
                return new BadRequestObjectResult(idResult);
            }
            var customer = new Customer
            {
                IdentityId = user.Id,
                Description = input.Description,
                Label = input.Label,
                AvatarId = input.AvatarId,
                ProfileBackgroundId = input.ProfileBackgroundId,
                // AccessRights = int.MaxValue, // puno right 
            };

            await context.Customer.AddAsync(customer);
            await context.SaveChangesAsync();

            logger.LogDebug($"\nCreated user: " +
                $"\nId:\t{customer.IdentityId}" +
                $"\nUsername:\t{user.UserName}");

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

            await SendConfirmationEmail(parsedEmail, user, code);

            return new OkObjectResult("Account created");
        }

        [AllowAnonymous]
        [HttpGet("resend_confirmation_email")]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            MailAddress parsedEmail = ParseEmail(email);
            if (parsedEmail == null || !emailService.IsEmailProviderSupported(parsedEmail.Address))
            {
                return BadRequest();
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await userManager.UpdateSecurityStampAsync(user);
            if (result == null)
            {
                logger.LogError($"Failed to update security stamp for user with email: {user.Email}");
                return BadRequest();
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

            await SendConfirmationEmail(parsedEmail, user, code);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("confirm_email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);
            string error = null;
            string message = "<html><body><h2>Email je bil uspesno potrjen ! Lahko se prijavis v aplikacijo.</h2></body></html>";

            if (user == null)
            {
                logger.LogError($"User {userId} not found. Confirmation email: {code}");
                error = "Neznana napaka.";
            }
            else
            {
                if (!user.EmailConfirmed)
                {
                    IdentityResult confirmEmailResult = await userManager.ConfirmEmailAsync(user, code);
                    if (!confirmEmailResult.Succeeded)
                    {
                        error = "\n" + confirmEmailResult.Errors?.Aggregate(string.Empty, (str, err) => str += $"{err.Code}: {err.Description}\n");
                    }
                }
            }

            if (error != null)
            {
                logger.LogError($"Errors when confirming email for user {userId}: " + error);
                message = $"<html><body><h2>Emaila ni bilo mogoce potrditi.</h2></br>{error}</body></html>";
            }

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = message,
            };
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await Identity.GetUserIdentity(httpContextAccessor.HttpContext.User, userManager);

            IdentityResult result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok();
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordInput input)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(input.Email);
                //If user has to activate his email to confirm his account, the use code listing below
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    logger.LogDebug("Forgot password: user not found or account is unconfirmed: " + input.Email);

                    return Ok();
                }

                string code = string.Empty;
                for (int i = 0; i < 6; i++)
                {
                    code += r.Next(0, 10);
                }

                OTP otp = context.OTPs.FirstOrDefault(o => o.IdentityId == user.Id);
                if (otp == null)
                {
                    context.OTPs.Add(otp = new OTP
                    {
                        Code = int.Parse(code),
                        IdentityId = user.Id,
                        Timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    otp.Code = int.Parse(code);
                    otp.Timestamp = DateTime.UtcNow;
                    otp.LoginCount = 0;
                    context.OTPs.Update(otp);
                }

                await context.SaveChangesAsync();

                //string code = await userManager.GeneratePasswordResetTokenAsync(user);

                await emailService.SendEmail(user.Email, "Reset Password", $"Please reset your password by using this {code}");
                return Ok();
            }

            // If we got this far, something failed, redisplay form
            return BadRequest(ModelState);
        }

        [HttpPost("otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateOTP(ValidateOTPInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                logger.LogDebug("Validate OTP: user doesn't exist: " + input.Email);
                return Ok(new ValidateOTPResponse());
            }

            OTP otp = context.OTPs.FirstOrDefault(o => o.IdentityId == user.Id);
            if (otp == null)
            {
                logger.LogDebug("Validate OTP: not otp found for user: " + user.Email);
                return BadRequest();
            }

            IActionResult result = null;

            if (otp.Timestamp.Add(TimeSpan.FromMinutes(30)) < DateTime.UtcNow)
            {
                context.OTPs.Remove(otp);
                result = BadRequest("OTP expired.");
            }
            else if (otp.Code != input.OTP)
            {
                if (otp.LoginCount > 5)
                {
                    context.OTPs.Remove(otp);
                    result = Forbid("Too many invalid requests.");
                }
                else
                {
                    otp.LoginCount++;
                    context.OTPs.Update(otp);
                    result = BadRequest("Wrong password.");
                }
            }
            else
            {
                string token = await userManager.GeneratePasswordResetTokenAsync(user);
                result = Ok(
                    new ValidateOTPResponse
                    {
                        PwdResetToken = token
                    });
                context.OTPs.Remove(otp);
                logger.LogDebug("OTP validation successful.");
            }

            await context.SaveChangesAsync();
            return result;
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }
            var result = await userManager.ResetPasswordAsync(user, input.Token, input.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            return Ok();
        }

        private IActionResult HandleIdentityErr(IdentityResult result)
        {
            RequestError err = new RequestError();
            err.Add(
                result.Errors.Select(idErr => ProcessIdentityError(idErr)));
            return err.ToActionResult();
        }

        private Task SendConfirmationEmail(MailAddress receiverEmail, ApplicationUser user, string code)
        {
            var callbackUrl = Url.Action("confirm_email", "account",
                   new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            string confirmEmailBody = $"<h2>Potrebna je potrditev</h2></br> <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Prosim potrdi svoj email tukaj.</a>";
            
            return emailService.SendEmail(receiverEmail, "Potrdi email.", confirmEmailBody);
        }

        private MailAddress ParseEmail(string email)
        {
            try
            {
                return new MailAddress(email);
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, $"Failed to parse email: {email}");
            }

            return null;
        }

        private KeyValuePair<string, string> ProcessIdentityError(IdentityError idErr)
        {
            string key = "unknown";
            switch (idErr.Code)
            {
                case nameof(IdentityErrorDescriber.DuplicateUserName):
                case nameof(IdentityErrorDescriber.InvalidUserName):
                    key = nameof(AccountInput.Username);
                    break;
                case nameof(IdentityErrorDescriber.DuplicateEmail):
                case nameof(IdentityErrorDescriber.InvalidEmail):
                    key = nameof(AccountInput.Email);
                    break;
                case nameof(IdentityErrorDescriber.PasswordMismatch):
                case nameof(IdentityErrorDescriber.PasswordRequiresDigit):
                case nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric):
                case nameof(IdentityErrorDescriber.PasswordRequiresUpper):
                case nameof(IdentityErrorDescriber.PasswordTooShort):
                    key = nameof(AccountInput.Password);
                    break;
                default:
                    break;
            }

            return new KeyValuePair<string, string>(key, idErr.Description);
        }
    }
}
