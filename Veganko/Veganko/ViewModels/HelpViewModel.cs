using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using Veganko;

namespace Veganko.ViewModels
{
    public class HelpViewModel : BaseViewModel
    {
        public HelpViewModel()
        {
            VersionCode = AppInfo.BuildString;
        }

        public Command CloseCommand => new Command(
            async () => App.Navigation.PopModalAsync());

        public Command LinkToRedditCommand => new Command(
            async () => await Launcher.OpenAsync("https://www.reddit.com/r/veganko/"));

        public string ContactEmail { get; } = "vegankoapp@gmail.com";

        public string VersionCode { get; }
    }
}
