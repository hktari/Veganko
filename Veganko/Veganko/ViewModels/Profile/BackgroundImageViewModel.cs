using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;
using Xamarin.Forms;

namespace Veganko.ViewModels.Profile
{
    public class BackgroundImageViewModel : BaseViewModel
    {
        public List<ProfileBackgroundImage> BackgroundImageSource => new List<ProfileBackgroundImage>
        {
            new ProfileBackgroundImage
            {
                Id = "0",
                Image = "pbg_1.png"
            },
            new ProfileBackgroundImage
            {
                Id = "1",
                Image = "pbg_2.png"
            },
            new ProfileBackgroundImage
            {
                Id = "2",
                Image = "pbg_3.png"
            },
            new ProfileBackgroundImage
            {
                Id = "3",
                Image = "pbg_4.png"
            },
            new ProfileBackgroundImage
            {
                Id = "4",
                Image = "pbg_5.png"
            },
            new ProfileBackgroundImage
            {
                Id = "5",
                Image = "pbg_6.png"
            },
        };

        public const string SaveMsg = "ProfileBackgroundImage_Save";

        public Task Save()
        {
            MessagingCenter.Send(this, SaveMsg, Selected);
            return Task.CompletedTask;
        }

        public ProfileBackgroundImage Selected { get; set; }

        public BackgroundImageViewModel(ProfileBackgroundImage profileBackground)
        {
            Selected = profileBackground;
        }
    }
}
