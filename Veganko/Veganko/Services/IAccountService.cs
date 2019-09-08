using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.Models.User;

namespace Veganko.Services
{
    public interface IAccountService
    {
        UserPublicInfo User { get; }
        Task CreateAccount(UserPublicInfo user, string password);
        Task Login(string email, string password);
        Task ForgotPassword(string email);
        Task ResetPassword(string email, string token, string newPassword);
        Task<string> ValidateOTP(string email, int otp);
        Task<bool> LoginWithFacebook();
        void Logout();
    }
}
