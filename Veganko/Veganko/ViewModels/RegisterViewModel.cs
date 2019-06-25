using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Services;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private IAccountService accountService;

        public RegisterViewModel()
        {
            accountService = DependencyService.Get<IAccountService>();
        }

        private string username;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private string password;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private string email;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public Task<bool> RegisterUser()
        {
            return accountService.CreateAccount(Username, Password);
        }
    }
}
