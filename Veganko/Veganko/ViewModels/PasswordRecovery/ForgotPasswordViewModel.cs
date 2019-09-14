using Autofac;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Veganko.Services;

namespace Veganko.ViewModels.PasswordRecovery
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly IAccountService accountService;

        private string passwordResetToken;

        public ForgotPasswordViewModel()
        {
            accountService = App.IoC.Resolve<IAccountService>();
        }

        private string email;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        private string otp;
        public string OTP
        {
            get => otp;
            set => SetProperty(ref otp, value);
        }

        private string newPassword;
        public string NewPassword
        {
            get => newPassword;
            set => SetProperty(ref newPassword, value);
        }

        private string confirmNewPassword;
        public string ConfirmNewPassword
        {
            get => confirmNewPassword;
            set => SetProperty(ref confirmNewPassword, value);
        }

        public Task SendForgotPasswordRequest()
        {
            return accountService.ForgotPassword(email);
        }

        public async Task ValidateOTP()
        {
            bool validOTP = int.TryParse(otp, out int otpParsed);
            Debug.Assert(validOTP);

            passwordResetToken = await accountService.ValidateOTP(email, otpParsed);
        }

        public Task ResetPassword()
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(newPassword));
            Debug.Assert(!string.IsNullOrWhiteSpace(confirmNewPassword));
            Debug.Assert(!string.IsNullOrWhiteSpace(email));
            Debug.Assert(!string.IsNullOrWhiteSpace(passwordResetToken));

            // TODO: handle password errors
            return accountService.ResetPassword(email, passwordResetToken, newPassword);
        }
    }
}
