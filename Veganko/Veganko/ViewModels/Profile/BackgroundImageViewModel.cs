using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;
using Veganko.Other;
using Xamarin.Forms;

namespace Veganko.ViewModels.Profile
{
    public class BackgroundImageViewModel : BaseViewModel
    {
        public List<ImageId> BackgroundImages { get; }

        public const string SaveMsg = "ProfileBackgroundImage_Save";
        
        public ImageId Selected { get; set; }

        public BackgroundImageViewModel(string backgroundImageId)
        {
            BackgroundImages = Images.BackgroundImageSource;
            Selected = BackgroundImages.FirstOrDefault(img => img.Id == backgroundImageId) ?? BackgroundImages.First();
        }

        public Task Save()
        {
            MessagingCenter.Instance.Send(this, SaveMsg, Selected);
            return Task.CompletedTask;
        }
    }
}
