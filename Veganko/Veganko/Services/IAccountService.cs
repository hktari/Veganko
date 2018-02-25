using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models;


namespace Veganko.Services
{
    interface IAccountService
    {
        User User { get; }
        bool CreateAccount(string username, string password, string profileImage);
        bool Login(string username, string password);
        bool Logout();
    }
}
