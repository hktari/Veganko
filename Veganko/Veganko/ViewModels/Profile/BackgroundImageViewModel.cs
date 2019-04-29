using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models.User;
using Veganko.Models.ViewModels.Images;
using Veganko.Other;
using Xamarin.Forms;

namespace Veganko.ViewModels.Profile
{
    public class BackgroundImageViewModel : BaseViewModel
    {
        public List<SelectableImageId> BackgroundImages { get; }

        public const string SaveMsg = "ProfileBackgroundImage_Save";
        
        public SelectableImageId Selected { get; set; }

        public BackgroundImageViewModel(string backgroundImageId)
        {
            BackgroundImages = Images.BackgroundImageSource.Select(imgId => new SelectableImageId(imgId)).ToList();
            SelectBackground(
                BackgroundImages.FirstOrDefault(img => img.Id == backgroundImageId) ?? BackgroundImages.First());
        }

        public void SelectBackground(SelectableImageId background)
        {
            foreach (var img in BackgroundImages)
            {
                img.IsSelected = false;
            }

            Selected = background;
            Selected.IsSelected = true;
        }

        public Task Save()
        {
            MessagingCenter.Instance.Send(this, SaveMsg, Selected.Id);
            return Task.CompletedTask;
        }
    }
}
