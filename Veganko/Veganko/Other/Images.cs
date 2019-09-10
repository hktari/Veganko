using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veganko.Models.User;

namespace Veganko.Other
{
    public static class Images
    {
        public static List<ImageId> BackgroundImageSource => new List<ImageId>
        {
            new ImageId
            {
                Id = "0",
                Image = "pinkbackground1.png"
            },
            new ImageId
            {
                Id = "1",
                Image = "greenbackground2.png"
            },
            new ImageId
            {
                Id = "2",
                Image = "orangebackground3.png"
            },
            new ImageId
            {
                Id = "3",
                Image = "yellowbackground4.png"
            },
            new ImageId
            {
                Id = "4",
                Image = "greybackground5.png"
            },
            new ImageId
            {
                Id = "5",
                Image = "violetbackground6.png"
            },
        };

        public static List<ImageId> AvatarImageSource => new List<ImageId>
        {
            new ImageId
            {
                Id = "0",
                Image = "avatar_fox.png"
            },
            new ImageId
            {
                Id = "1",
                Image = "avatar_bear.png"
            },
            new ImageId
            {
                Id = "2",
                Image = "avatar_cat.png"
            },
        };

        public static string GetProfileBackgroundImageId(string image)
        {
            return BackgroundImageSource.FirstOrDefault(img => img.Image == image)?.Id
                ?? throw new ArgumentException("Invalid image for profile background!");
        }

        public static string GetProfileAvatarId(string image)
        {
            return AvatarImageSource.FirstOrDefault(img => img.Image == image)?.Id
                ?? throw new ArgumentException("Invalid image for avatar image!");
        }

        public static string GetProfileBackgroundImageById(string id)
        {
            return BackgroundImageSource.FirstOrDefault(img => img.Id == id)?.Image 
                ?? throw new ArgumentException("Invalid id for profile background!");
        }

        public static string GetProfileAvatarById(string avatarId)
        {
            return AvatarImageSource.FirstOrDefault(img => img.Id == avatarId)?.Image
                ?? throw new ArgumentException("Invalid id for avatar image!");
        }
    }
}
