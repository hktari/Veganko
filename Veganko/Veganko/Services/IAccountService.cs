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
        Task<bool> LoginWithFacebook();
        bool Logout();
    }
}
