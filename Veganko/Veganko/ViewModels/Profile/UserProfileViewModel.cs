using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Extensions;
using Veganko.Services.Http;

namespace Veganko.ViewModels.Profile
{
    public class UserProfileViewModel : BaseUserProfileViewModel
    {
        private readonly string userId;

        public UserProfileViewModel(string userId)
            : base(false)
        {
            ShouldShowDescriptionPlaceholder = false;
            IsEditingDescription = false;
            this.userId = userId;
        }

        public async override void OnPageAppearing()
        {
            base.OnPageAppearing();

            try
            {
                IsBusy = true;
                User = await UserService.Get(userId);
                HandleNewData();
            }
            catch (ServiceException sex)
            {
                await App.CurrentPage.Err("Napaka pri nalaganju profilne.", sex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
