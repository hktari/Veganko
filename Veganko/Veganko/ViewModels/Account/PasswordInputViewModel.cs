using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Validations;

namespace Veganko.ViewModels.Account
{
    public class PasswordInputViewModel : BaseViewModel
    {
        public PasswordInputViewModel()
        {
            password.Validations.Add(new MinLengthRule(6));
            confirmPassword.Validations.Add(new ValueMatchesRule(password)
            {
                ValidationMessage = "Se ne ujema z geslom."
            });
        }

        private ValidatableObject<string> password = new ValidatableObject<string>();
        public ValidatableObject<string> Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private ValidatableObject<string> confirmPassword = new ValidatableObject<string>();
        public ValidatableObject<string> ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }
    }
}
