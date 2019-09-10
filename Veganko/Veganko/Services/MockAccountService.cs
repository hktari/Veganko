using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Other;

namespace Veganko.Services
{
    class MockAccountService : IAccountService
    {
        private static int IdCounter;
        public UserPublicInfo User { get; set; }

        private List<UserPublicInfo> userDatabase = new List<UserPublicInfo>();
        private Dictionary<string, string> userPasswords = new Dictionary<string, string>();

        public Task CreateAccount(UserPublicInfo user, string password)
        {
            // check if username exists
            if (userDatabase.Exists(u => u.Username == user.Username))
                throw new Exception("Exists !");

            var hashedPassword = Helper.CalculateBase64Sha256Hash(password);
            var curId = IdCounter.ToString();
            IdCounter++;
            
            userDatabase.Add(user);
            userPasswords.Add(user.Id, hashedPassword);

            return Task.CompletedTask;
        }

        public Task Login(string username, string password)
        {
            var user = userDatabase.Find(u => u.Username == username);
            if (user == null || Helper.CalculateBase64Sha256Hash(password) != userPasswords[user.Id])
            {
                throw new Exception("Invalid credentials.");
            }

            User = user;

            return Task.CompletedTask;
        }

        public void Logout()
        {
            User = null;
        }

        public Task<bool> LoginWithFacebook()
        {
            throw new NotImplementedException();
        }

        public Task ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        public Task ResetPassword(string email, string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<string> ValidateOTP(string email, int otp)
        {
            throw new NotImplementedException();
        }
    }
}
