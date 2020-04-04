using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Users;
using Veganko.Extensions;
using Veganko.Services.Http;
using Veganko.Services.Logging;
using Veganko.ViewModels.Profile;
using Xamarin.Forms;

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

        public bool CanUserBeModerated => UserService.CurrentUser.Role > Common.Models.Users.UserRole.Moderator;

        public List<string> AllRoles => Enum.GetNames(typeof(UserRole))
            .Where(n => n != UserRole.Admin.ToString())
            .ToList();

        private async Task ChangeRole(string newRole)
        {
            if (!Enum.TryParse<UserRole>(newRole, out var role))
            {
                Logger.WriteLine<UserProfileDetailViewModel>("Failed to parse role");
                return;
            }

            try
            {
                IsBusy = true;
                await UserService.SetRole(User, role);
                User.Role = role;
                Role = newRole;
            }
            catch (ServiceException ex)
            {
                await App.CurrentPage.Err("Nisem uspel spreminiti vloge.", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string role;
        public string Role
        {
            get => role;
            set => SetProperty(ref role, value);
        }

        private bool isDirty;
        public bool IsDirty
        {
            get => isDirty;
            set => SetProperty(ref isDirty, value);
        }

        private string formattedUserEmail;
        public string FormattedUserEmail
        {
            get => formattedUserEmail;
            set => SetProperty(ref formattedUserEmail, value);
        }

        public Command ApplyChangesCommand => new Command(
            async _ =>
            {
                IsBusy = true;
                await ChangeRole(Role);
                RefreshIsDirty();
            });

        public Command RefreshIsDirtyCommand => new Command(
            _ =>
            {
                RefreshIsDirty();
            });

        private void RefreshIsDirty()
        {
            IsDirty = Role != User.Role.ToString();
        }

        private ILogger Logger => App.IoC.Resolve<ILogger>();

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

        protected override void HandleNewData()
        {
            base.HandleNewData();
            Role = User.Role.ToString();
            FormattedUserEmail = User.Email;
            if (!User.IsEmailConfirmed)
            {
                FormattedUserEmail += " (nepotrjen)";
            }

            RefreshIsDirty();
        }
    }
}
