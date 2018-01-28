using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        Page currentTab;
        ObservableCollection<Page> tabs;

        public MainPageViewModel()
        {

        }

        public ObservableCollection<Page> Tabs
        {
            get
            {
                return tabs;
            }
            private set { SetProperty(ref tabs, value); }
        }

        public Page CurrentTab
        {
            get { return currentTab ?? (currentTab = Tabs[0]); }
            set { SetProperty(ref currentTab, value); }
        }
    }
}