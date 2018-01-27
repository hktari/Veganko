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
	public class SelectableProductClassifierView : ProductClassifierView, INotifyPropertyChanged
	{
        public static readonly BindableProperty SelectedProperty =
            BindableProperty.Create(nameof(Selected), typeof(List<ProductClassifier>), typeof(SelectableProductClassifierView), new List<ProductClassifier>(), BindingMode.OneWayToSource);

        public List<ProductClassifier> Selected
		{
			get
            {
                return (List<ProductClassifier>)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
                //OnPropertyChanged(nameof(Selected));
            }
		}
        
		Command HandleButtonClickCommand => new Command(
			(param) =>
			{
				var vm = param as ProductClassifierItemViewModel;
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
	    
        public override void HandleSourceChanged(List<ProductClassifier> newSource)
        {
            // re-initialize the buttons and state of the viewmodel
            Selected.Clear();
            if (newSource == null)
                return;
            SetViewContent(CreateView(newSource));
        }

        List<View> CreateView(List<ProductClassifier> source)
        {
            List<View> views = new List<View>();
            foreach (var classifier in source)
            {
                var image = new Image();
                ProductClassifierItemViewModel vm;
                image.BindingContext = vm = new ProductClassifierItemViewModel(classifier, GetImageForClassifer(classifier), HandleButtonClickCommand);
                image.SetBinding(Image.SourceProperty, nameof(ProductClassifierItemViewModel.Image));
                image.SetBinding(Image.OpacityProperty, nameof(ProductClassifierItemViewModel.Opacity));
                image.WidthRequest = image.HeightRequest = ViewSize;
                image.HorizontalOptions = LayoutOptions.Center;
                image.Aspect = Aspect.AspectFill;
                TapGestureRecognizer gestureRecognizer = new TapGestureRecognizer() { BindingContext = vm };
                gestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(ProductClassifierItemViewModel.ClickedCommand));

                image.GestureRecognizers.Add(gestureRecognizer);
                views.Add(image);
            }
            return views;
        }
    }
}
