﻿using System;
using System.Linq;
using Veganko.Models;
using Veganko.Services;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{
        public MainPage(bool isManager = false)
        {
            InitializeComponent();

            if (isManager)
            {
                Children.Add(
                    new NavigationPage(new ProductPage(new ManageProductsViewModel()))
                    {
                        Title = Device.RuntimePlatform == Device.UWP ? "Manage" : string.Empty,
                        Icon = Device.RuntimePlatform == Device.UWP ? null : "icon.png"
                    });
            }
        }

        private NavigationPage lastPage;

        protected async override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            Title = CurrentPage?.Title;

            if (lastPage != null)
            {
                await lastPage.PopToRootAsync();
            }

            lastPage = CurrentPage as NavigationPage;
        }
    }
}