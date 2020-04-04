using Autofac;
using Veganko.Extensions;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.ViewModels.Profile;

namespace Veganko.ViewModels.Users
{
    public class UserProfileDetailViewModel : BaseUserProfileViewModel
    {
        private readonly string userId;

        public UserProfileDetailViewModel(UserProfileViewModel user)
            : base(false)
        {
            User = user;
            HandleNewData();
        }

        public UserProfileDetailViewModel(string userId)
            : base(false)
        {
            this.userId = userId;
        }

        public override async void OnPageAppearing()
        {
            base.OnPageAppearing();
            try
            {
                if (userId != null)
                {
                    IsBusy = true;
                    User = new UserProfileViewModel(await UserService.Get(userId));
                    HandleNewData();
                }
            }
            catch (ServiceException ex)
            {
                await App.CurrentPage.Err("Napaka pri nalaganju", ex);
                await App.Navigation.PopAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
