using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Common.Models.Users;
using Veganko.Models;
using Veganko.Models.User;

namespace Veganko.Services
{
    public interface IAccountService
    {
        Task CreateAccount(UserPublicInfo user, string password);
        Task ForgotPassword(string email);
        Task ResendConfirmationEmail(string email);
        Task ResetPassword(string email, string token, string newPassword);
        Task<string> ValidateOTP(string email, int otp);
    }
}
