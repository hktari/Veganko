using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Veganko.ViewModels
{
    // TODO: evaluating usernames ? link to profile detail ?
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
    }
}