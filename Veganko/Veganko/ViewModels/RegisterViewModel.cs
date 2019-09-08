using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private IAccountService accountService;
        private static Random r = new Random();

        public RegisterViewModel()
        {
            accountService = App.IoC.Resolve<IAccountService>();

            ProfileBackgroundImage = Images.BackgroundImageSource[r.Next(Images.BackgroundImageSource.Count)];
            AvatarImage = Images.AvatarImageSource[r.Next(Images.AvatarImageSource.Count)];
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

        private string confirmPassword;
        public string ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }

        private string email;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        private string label;
        public string Label
        {
            get => label;
            set => SetProperty(ref label, value);
        }

        private ImageId avatarImage;
        public ImageId AvatarImage
        {
            get => avatarImage;
            set => SetProperty(ref avatarImage, value);
        }

        private ImageId profileBackgroundImage;
        public ImageId ProfileBackgroundImage
        {
            get => profileBackgroundImage;
            set => SetProperty(ref profileBackgroundImage, value);
        }
        public async Task RegisterUser()
        {
            UserPublicInfo user = new UserPublicInfo
            {
                Username = username,
                Email = email,
                Label = label,
                AvatarId = avatarImage.Id,
                ProfileBackgroundId = profileBackgroundImage.Id
            };

            await accountService.CreateAccount(user, Password);
        }
    }
}
