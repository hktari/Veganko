using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Veganko.ViewModels.PasswordRecovery
{
    public abstract class EditAccountViewModel : BaseViewModel
    {
        public abstract Command SubmitCommand { get; }

        public abstract string SubmitBtnText { get; }
    }
}
