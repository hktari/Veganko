using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Other;

[assembly: Xamarin.Forms.Dependency(typeof(Veganko.Services.MockAccountService))]
namespace Veganko.Services
{
    class MockAccountService : IAccountService
    {
        private static int IdCounter;
        public User User { get; private set; }

        private List<User> userDatabase = new List<User>();

        public Task<bool> CreateAccount(string username, string password)
        {
            // check if username exists
            if (userDatabase.Exists(u => u.Username == username))
                return Task.FromResult(false);
            var hashedPassword = Helper.CalculateBase64Sha256Hash(password);
            var curId = IdCounter.ToString();
            IdCounter++;
            var user = new User
            {
                Id = curId,
                Username = username,
                Password = hashedPassword,
                AvatarId = Images.AvatarImageSource.First().Id,
                ProfileBackgroundId = Images.BackgroundImageSource.First().Id,
                AccessRights = UserAccessRights.All
            };
            userDatabase.Add(user);
            return Task.FromResult(true);
        }

        public bool Login(string username, string password)
        {
            var user = userDatabase.Find(u => u.Username == username);
            if (user != null && Helper.CalculateBase64Sha256Hash(password) == user.Password)
            {
                User = user;
                return true;
            }
            return false;            
        }

        public bool Logout()
        {
            User = null;
            return true;
        }

        public Task<bool> LoginWithFacebook()
        {
            throw new NotImplementedException();
        }
    }
}
