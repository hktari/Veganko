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
        User User { get; }
        Task CreateAccount(User user, string password);
        Task Login(string username, string password);
        Task ForgotPassword(string email);
        Task ResetPassword(string email, string token, string newPassword);
        Task<string> ValidateOTP(string email, int otp);
        Task<bool> LoginWithFacebook();
        bool Logout();
    }
}
