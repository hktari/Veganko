using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VegankoService.Models.User;

namespace VegankoService.Tests.Services
{
    public class MockUserManager : UserManager<ApplicationUser>
    {
        private static Dictionary<string, ApplicationUser> users = new Dictionary<string, ApplicationUser>();
        private static Dictionary<string, string> passwords = new Dictionary<string, string>();

        public MockUserManager() : this(null, null, null, null, null, null, null, null, null)
        {
            SeedWithTestUsers();
        }

        public MockUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            SeedWithTestUsers();
        }

        private void SeedWithTestUsers()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "confirmedUser", Email = "confirmed@email.com", PasswordHash = "Test123.", EmailConfirmed = true },
            };

            foreach (var user in users)
            {
                CreateAsync(user, user.PasswordHash).GetAwaiter().GetResult();
                //userManager.CreateAsync(user, "Test123.").GetAwaiter().GetResult();
                //userManager.AddToRoleAsync(user, VegankoService.Helpers.Constants.Strings.Roles.Admin).GetAwaiter().GetResult();

                //var tmp = db.Users.Find(user.Id);
                //tmp.EmailConfirmed = true;
                //db.Users.Update(tmp);

                //db.Customer.Add(
                //    new Customer
                //    {
                //        IdentityId = user.Id,
                //    });
            }
        }

        public override Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            //if (passwords.TryGetValue(user.Id, out string pwd))
            //{ 
            //    return password 
            //}
            //return base.CheckPasswordAsync(user, password);
            throw new NotImplementedException();
        }

        public override Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var user = users.FirstOrDefault(kvp => kvp.Value.Email == email).Value;
            return Task.FromResult(user);
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            IdentityResult result = IdentityResult.Success;

            List<IdentityError> errors = new List<IdentityError>();
            if (users.Any(kvp => kvp.Value.Email == user.Email))
            {
                errors.Add(new IdentityError { Code = nameof(IdentityErrorDescriber.DuplicateEmail) });
            }

            if (users.Any(kvp => kvp.Value.UserName == user.UserName))
            {
                errors.Add(new IdentityError { Code = nameof(IdentityErrorDescriber.DuplicateUserName) });
            }

            if (errors.Count == 0)
            {
                user.Id = Guid.NewGuid().ToString();
                users[user.Id] = user;
                passwords[user.Id] = password;
            }
            else
            {
                result = IdentityResult.Failed(errors.ToArray());
            }

            return Task.FromResult(result);
        }

        public override Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            IdentityResult result = null;
            if (!users.ContainsKey(user.Id))
            {
                result = IdentityResult.Failed();
            }
            else
            {
                users.Remove(user.Id);
                passwords.Remove(user.Id);
                result = IdentityResult.Success;
            }

            return Task.FromResult(result);
        }

        public override Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            IdentityResult result = IdentityResult.Success;
            var localUser = users.FirstOrDefault(kvp => kvp.Value.Email == user.Email).Value;
            
            if (localUser == null)
            {
                result = IdentityResult.Failed();
            }

            localUser.EmailConfirmed = user.EmailConfirmed = true;
            return Task.FromResult(result);
        }

        public override Task<ApplicationUser> FindByIdAsync(string userId)
        {
            users.TryGetValue(userId, out ApplicationUser user);
            return Task.FromResult(user);
        }

        public override Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            throw new NotImplementedException();
        }

        public override Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return Task.FromResult<IList<string>>(
                new[] { VegankoService.Helpers.Constants.Strings.Roles.Admin });
        }

        public override Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }
    }
}
