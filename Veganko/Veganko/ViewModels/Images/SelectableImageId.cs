using System;
using System.Collections.Generic;
using System.Text;
using Veganko.Models.User;
using Veganko.ViewModels;

namespace Veganko.Models.ViewModels.Images
{
    public class SelectableImageId : BaseViewModel
    {
        public SelectableImageId()
        {
        }

        public SelectableImageId(ImageId imageId)
        {
            Id = imageId.Id;
            Image = imageId.Image;
        }

        public string Id { get; set; }

        public string Image { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }
    }
}
