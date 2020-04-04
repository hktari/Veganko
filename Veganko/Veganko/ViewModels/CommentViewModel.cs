using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Common.Models.Users;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Other;
using Veganko.ViewModels.Profile;
using Veganko.ViewModels.Users;
using Veganko.Views.Profile;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class CommentViewModel : BaseViewModel
    {
        public CommentViewModel(UserPublicInfo user)
        {
            Username = user.Username;
            UserAvatar = Images.GetProfileAvatarById(user.AvatarId);
        }

        public CommentViewModel(Comment comment)
        {
            Id = comment.Id;
            UserId = comment.UserId;
            Username = comment.Username;
            UserAvatar = Images.GetProfileAvatarById(comment.UserAvatarId);
            Text = comment.Text;
            DatePosted = comment.UtcDatePosted.ToLocalTime();
            Rating = comment.Rating.GetValueOrDefault();
            HasRating = comment.Rating != null;
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string UserAvatar { get; set; }
        public int Rating { get; set; }
        public bool HasRating { get; set; }
        public string Text { get; set; }
        public DateTime DatePosted { get; set; }

        private Command profileTappedCommand;
        public Command ProfileTappedCommand => profileTappedCommand 
            ?? (profileTappedCommand = new Command(
                async () => await App.Navigation.PushAsync(
                    new UserProfileDetailPage(new UserProfileDetailViewModel(UserId)))));
    }
}
