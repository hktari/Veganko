using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.Services.Logging;
using Veganko.Validations;
using Veganko.ViewModels.Account;
using Veganko.ViewModels.PasswordRecovery;
using Veganko.Views.Account;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class RegisterViewModel : EditAccountViewModel
    {
        private IAccountService accountService;
        private ILogger logger;
        private static Random r = new Random();

        public RegisterViewModel()
        {
            accountService = App.IoC.Resolve<IAccountService>();
            logger = App.IoC.Resolve<ILogger>();

            ProfileBackgroundImage = Images.BackgroundImageSource[r.Next(Images.BackgroundImageSource.Count)];
            AvatarImage = Images.AvatarImageSource[r.Next(Images.AvatarImageSource.Count)];

            username.Validations.Add(new IsNotNullOrEmptyRule<string> 
            { 
                ValidationMessage = "Zahtevano polje."
            });
          
            email.Validations.Add(new IsValidEmailRule()
            {
                ValidationMessage = "Nepravilen email."
            });
        }

        public PasswordInputViewModel PasswordInput { get; } = new PasswordInputViewModel();

        private ValidatableObject<string> username = new ValidatableObject<string>();
        public ValidatableObject<string> Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private ValidatableObject<string> email = new ValidatableObject<string>();
        public ValidatableObject<string> Email
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

        public override Command SubmitCommand => new Command(
            async () =>
            {
                bool validInput = true;
                validInput &= username.Validate();
                validInput &= PasswordInput.Password.Validate();
                validInput &= PasswordInput.ConfirmPassword.Validate();
                validInput &= email.Validate();

                if (!validInput)
                {
                    return;
                }

                try
                {
                    UserPublicInfo user = new UserPublicInfo
                    {
                        Username = username.Value,
                        Email = email.Value,
                        Label = label,
                        AvatarId = avatarImage.Id,
                        ProfileBackgroundId = profileBackgroundImage.Id
                    };

                    await accountService.CreateAccount(user, PasswordInput.Password.Value);
                    await App.Navigation.PopToRootAsync();
                    await App.Navigation.PushAsync(new FinishRegistrationInstructPage());
                }
                catch (ServiceException ex)
                {
                    logger.LogException(ex);
                    // TODO: duplicate user ?
                    await App.CurrentPage.Err("Neznana napaka pri registraciji: " + ex.StatusDescription);
                }
            });

        public override string SubmitBtnText => "PRIJAVI ME";
    }
}
