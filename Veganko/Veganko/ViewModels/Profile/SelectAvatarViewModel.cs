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
    public class SelectAvatarViewModel : BaseViewModel
    {
        public class SelectableImageId : BaseViewModel
        {
            public string Id { get; set; }

            public string Image { get; set; }

            private bool isSelected;
            public bool IsSelected
            {
                get => isSelected;
                set => SetProperty(ref isSelected, value);
            }
        }

        public List<SelectableImageId> AvatarSource { get; }

        public const string SaveMsg = "ProfileAvatar_Save";

        public SelectableImageId Selected { get; set; }

        public Command SelectAvatarCommand { get; }

        public SelectAvatarViewModel(string avatarId)
        {
            AvatarSource = new List<SelectableImageId>();
            foreach (var ai in Images.AvatarImageSource)
            {
                AvatarSource.Add(
                    new SelectableImageId
                    {
                        Id = ai.Id,
                        Image = ai.Image,
                        IsSelected = ai.Id == avatarId
                    });
            }
            Selected = AvatarSource.FirstOrDefault(img => img.Id == avatarId) ?? AvatarSource.First();
            SelectAvatarCommand = new Command(OnAvatarSelected);
        }

        private void OnAvatarSelected(object obj)
        {
            Selected = (SelectableImageId)obj;
            foreach (var img in AvatarSource)
            {
                img.IsSelected = false;
            }
            Selected.IsSelected = true;
        }

        public Task Save()
        {
            MessagingCenter.Instance.Send(this, SaveMsg, Selected.Id);
            return Task.CompletedTask;
        }
    }
}
