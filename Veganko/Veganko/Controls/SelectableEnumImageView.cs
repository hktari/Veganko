using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Veganko.Models;
using Veganko.ViewModels;
using Xamarin.Forms;

namespace Veganko.Controls
{
	public class SelectableEnumImageView<T> : EnumImageView<T>, INotifyPropertyChanged
	{
        public static readonly BindableProperty SelectedProperty =
            BindableProperty.Create(nameof(Selected), typeof(List<T>), typeof(SelectableEnumImageView<T>), new List<T>(), BindingMode.OneWayToSource);

        public List<T> Selected
		{
			get
            {
                return (List<T>)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
            }
		}
        
		Command HandleButtonClickCommand => new Command(
			(param) =>
			{
				var vm = param as EnumImageItemViewModel<T>;
				if (vm.IsSelected)
				{
					Debug.Assert(!Selected.Contains(vm.Classifier));
					Selected.Add(vm.Classifier);
				}
				else
				{
					Debug.Assert(Selected.Contains(vm.Classifier));
					Selected.Remove(vm.Classifier);
				}
				string tmp = "";
				Selected.ForEach(item => tmp += $"{item}, ");
				Console.WriteLine(tmp);
			});
	    
        public override void HandleSourceChanged(List<T> newSource)
        {
            // re-initialize the buttons and state of the viewmodel
            Selected.Clear();
            if (newSource == null)
                return;
            SetViewContent(CreateView(newSource));
        }

        List<View> CreateView(List<T> source)
        {
            List<View> views = new List<View>();
            foreach (var classifier in source)
            {
                var image = new Image();
                EnumImageItemViewModel<T> vm;
                image.BindingContext = vm = new EnumImageItemViewModel<T>(classifier, GetImageForClassifer(classifier), HandleButtonClickCommand);
                image.SetBinding(Image.SourceProperty, nameof(EnumImageItemViewModel<T>.Image));
                image.SetBinding(Image.OpacityProperty, nameof(EnumImageItemViewModel<T>.Opacity));
                image.WidthRequest = image.HeightRequest = ViewSize;
                image.HorizontalOptions = LayoutOptions.Center;
                image.Aspect = Aspect.AspectFill;
                TapGestureRecognizer gestureRecognizer = new TapGestureRecognizer() { BindingContext = vm };
                gestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(EnumImageItemViewModel<T>.ClickedCommand));

                image.GestureRecognizers.Add(gestureRecognizer);
                views.Add(image);
            }
            return views;
        }
    }
}
