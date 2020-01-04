using Autofac;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Veganko.Extensions;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.Views.PasswordRecovery;
using Xamarin.Forms;

namespace Veganko.ViewModels.PasswordRecovery
{
    public class PasswordRecoveryViewModel : EditAccountViewModel
    {
        private const int requiredOTPLength = 6;
        private readonly IAccountService accountService;

        private string passwordResetToken;

        public PasswordRecoveryViewModel()
        {
            accountService = App.IoC.Resolve<IAccountService>();
        }

        public PasswordRecoveryStage CurrentStage { get; private set; }

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

        public override Command SubmitCommand => new Command(
            async () =>
            {
                switch (CurrentStage)
                {
                    case PasswordRecoveryStage.EnterEmail:
                        await SendForgotPasswordRequest();
                        break;
                    case PasswordRecoveryStage.ValidateOTP:
                        await ValidateOTP();
                        break;
                    case PasswordRecoveryStage.EnterNewPassword:
                        await ResetPassword();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            });

        public override string SubmitBtnText
        {
            get
            {
                switch (CurrentStage)
                {
                    case PasswordRecoveryStage.EnterEmail:
                    case PasswordRecoveryStage.ValidateOTP:
                        return "POŠLJI";
                    case PasswordRecoveryStage.EnterNewPassword:
                        return "RESETIRAJ";
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public async Task SendForgotPasswordRequest()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await App.CurrentPage.Err("Prosim vnesi email");
                return;
            }

            try
            {
                IsBusy = true;
                await accountService.ForgotPassword(email);
                CurrentStage = PasswordRecoveryStage.ValidateOTP;
                await App.Navigation.PushAsync(new ValidateOTPPage(this));
            }
            catch (ServiceException ex)
            {
                await App.CurrentPage.Err(ex.StatusCodeDescription);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ValidateOTP()
        {
            if (string.IsNullOrWhiteSpace(OTP))
            {
                await App.CurrentPage.Err("Prosim izpolni polje.");
                return;
            }

            if (OTP.Length != requiredOTPLength)
            {
                await App.CurrentPage.Err("Pričakujem 6-mestno številko.");
                return;
            }

            try
            {
                IsBusy = true;

                bool validOTP = int.TryParse(otp, out int otpParsed);
                Debug.Assert(validOTP);

                passwordResetToken = await accountService.ValidateOTP(email, otpParsed);
                CurrentStage = PasswordRecoveryStage.EnterNewPassword;
                
                await App.Navigation.PushAsync(new PasswordResetPage(this));
            }
            catch (ServiceException ex)
            {
                await App.CurrentPage.Err(ex.StatusCodeDescription);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ResetPassword()
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                await App.CurrentPage.Err("Prosim izpolni polja.");
                return;
            }
            else if (NewPassword != ConfirmNewPassword)
            {
                await App.CurrentPage.Err("Gesla se ne ujemata.");
                return;
            }

            try
            {
                IsBusy = true;
                
                // TODO: handle password errors
                await accountService.ResetPassword(email, passwordResetToken, newPassword);

                await App.CurrentPage.Inform("Geslo je spremenjeno.");
                await App.Navigation.PopToRootAsync();
            }
            catch (ServiceException ex)
            {
                await App.CurrentPage.Err(ex.Response);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public enum PasswordRecoveryStage
        {
            EnterEmail,
            ValidateOTP,
            EnterNewPassword,
        }
    }
}
