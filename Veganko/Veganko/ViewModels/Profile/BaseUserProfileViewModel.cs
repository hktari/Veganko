using Autofac;
using System.Diagnostics;
using Veganko.Common.Models.Users;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.Services;
using Veganko.Views.Profile;
using Xamarin.Forms;

namespace Veganko.ViewModels.Profile
{
    public class BaseUserProfileViewModel : BaseViewModel
    {
        public BaseUserProfileViewModel(bool isEditable)
        {
            IsEditable = isEditable;
            UserService = App.IoC.Resolve<IUserService>();
            Title = "Profile";
        }

        /// <summary>
        /// Whether the profile data can be changed.
        /// </summary>
        public bool IsEditable { get; }

        private string backgroundImage;
        public string BackgroundImage
        {
            get => backgroundImage;
            set => SetProperty(ref backgroundImage, value);
        }

        private string userDescription;
        public string UserDescription
        {
            get => userDescription;
            set => SetProperty(ref userDescription, value);
        }

        private string avatarImage;
        public string AvatarImage
        {
            get => avatarImage;
            set => SetProperty(ref avatarImage, value);
        }

        private UserPublicInfo user;
        public UserPublicInfo User
        {
            get => user;
            set => SetProperty(ref user, value);
        }

        private bool shouldShowDescriptionPlaceholder;
        public bool ShouldShowDescriptionPlaceholder
        {
            get => shouldShowDescriptionPlaceholder;
            set => SetProperty(ref shouldShowDescriptionPlaceholder, value);
        }

        private bool isEditingDescription;
        public bool IsEditingDescription
        {
            get => isEditingDescription;
            set => SetProperty(ref isEditingDescription, value);
        }

        public Command EditBackgroundImageCommand => new Command(
            async () =>
            {
                if (User == null)
                {
                    return;
                }

                await App.Navigation.PushModalAsync(
                    new SelectBackgroundPage(new BackgroundImageViewModel(User.ProfileBackgroundId)));
            });

        public Command EditAvatarImageCommand => new Command(
            async () =>
            {
                if (User == null)
                {
                    return;
                }

                await App.Navigation.PushModalAsync(
                    new SelectAvatarPage(new SelectAvatarViewModel(User.AvatarId)));
            });

        protected IUserService UserService { get; }

        protected virtual void HandleNewData()
        {
            Debug.Assert(User != null);
            UserDescription = User.Description;
            AvatarImage = Images.GetProfileAvatarById(User.AvatarId);
            BackgroundImage = Images.GetProfileBackgroundImageById(User.ProfileBackgroundId);
        }
    }
}
