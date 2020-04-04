using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Users;
using Veganko.Models.User;
using Veganko.Other;
using Xamarin.Forms;

namespace Veganko.ViewModels.Users
{
    public class UserProfileViewModel : UserPublicInfo
    {
        public UserProfileViewModel(UserPublicInfo user) : base(user)
        {
        }

        public ImageSource AvatarImage => AvatarId != null ? Images.GetProfileAvatarById(AvatarId) : null;
        public ImageSource BackgroundImage => ProfileBackgroundId != null ? Images.GetProfileBackgroundImageById(ProfileBackgroundId) : null;
    }
}
