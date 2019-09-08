using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models;
using Veganko.Models.User;
using Veganko.Other;

namespace Veganko.ViewModels
{
    public class CommentViewModel : BaseViewModel
    {
        public CommentViewModel(UserPublicInfo user)
        {
            Username = user.Username;
            UserAvatar = Images.GetProfileAvatarById(user.AvatarId);
        }

        public CommentViewModel(Comment comment, UserPublicInfo userInfo)
        {
            Id = comment.Id;
            Username = userInfo.Username;
            UserAvatar = Images.GetProfileAvatarById(userInfo.AvatarId);
            Text = comment.Text;
            DatePosted = comment.DatePosted;
            Rating = comment.Rating;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string UserAvatar { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
