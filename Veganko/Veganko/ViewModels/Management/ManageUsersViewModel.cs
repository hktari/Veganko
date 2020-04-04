using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veganko.Common.Models.Users;
using Veganko.Extensions;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Services;
using Veganko.Services.Http;
using Veganko.ViewModels.Users;
using Veganko.Views.Profile;
using Xamarin.Forms;

namespace Veganko.ViewModels.Management
{
    public class ManageUsersViewModel : BaseViewModel
    {
        public ManageUsersViewModel()
        {
            Roles = Enum.GetNames(typeof(UserRole))
                        .Where(n => n != UserRole.Admin.ToString())
                        .ToList();
            Roles.Insert(0, "Brez filtra");
        }

        private List<UserProfileViewModel> users = new List<UserProfileViewModel>();
        public List<UserProfileViewModel> Users
        {
            get => users;
            set => SetProperty(ref users, value);
        }

        public Command<UserProfileViewModel> UserSelectedCommand => new Command<UserProfileViewModel>(
            async selectedUser =>
            {
                await App.Navigation.PushAsync(
                    new UserProfileDetailPage(
                        new UserProfileDetailViewModel(selectedUser)));
            });

        public Command RefreshUsersCommand => new Command(
            async _ => await RefreshUsers());

        public List<string> Roles { get; }

        private string selectedFilter;
        public string SelectedFilter
        {
            get => selectedFilter;
            set => SetProperty(ref selectedFilter, value);
        }

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        public Command FilterChangedCommand => new Command(
            async _ => await RefreshUsers());

        private IUserService UserService => App.IoC.Resolve<IUserService>();

        public override async void OnPageAppearing()
        {
            base.OnPageAppearing();

            if (Users.Count == 0)
            {
                await RefreshUsers();
            }
        }

        public async Task RefreshUsers()
        {
            try
            {
                UserQuery query = new UserQuery();
                if (Enum.TryParse(SelectedFilter, out UserRole role))
                {
                    query.Role = role;
                }

                if (!string.IsNullOrEmpty(SearchText))
                {
                    query.Name = SearchText.Trim();
                }

                IsBusy = true;
                PagedList<UserPublicInfo> page = await UserService.GetAll(query);
                Users = page.Items
                    .Where(u => u.Role != UserRole.Admin) // Don't show admin account
                    .Select(u => new UserProfileViewModel(u))
                    .ToList();
            }
            catch (ServiceException ex)
            {
                await App.CurrentPage.Err("Napaka pri nalaganju", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
