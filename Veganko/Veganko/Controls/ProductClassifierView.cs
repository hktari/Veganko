using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veganko.Models;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductClassifierView : ContentView
    {
        private double buttonSize = 22.0d;
        public double ButtonSize
        {
            get { return buttonSize; }
            set { buttonSize = value; }
        }

        private StackOrientation orientation = StackOrientation.Horizontal;
        public StackOrientation Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        private List<ProductClassifier> selected = new List<ProductClassifier>();
        public IEnumerable<ProductClassifier> Selected
        {
            get { return selected; }
        }

        private Dictionary<ProductClassifier, string> source = new Dictionary<ProductClassifier, string>();
        public Dictionary<ProductClassifier, string> Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                // re-initialize the buttons and state of the viewmodel
                selected.Clear();
                var stackLayout = new StackLayout { Orientation = orientation, HorizontalOptions = LayoutOptions.Center };
                foreach (var item in source)
                {
                    var image = new Image();
                    ProductClassifierItemViewModel vm;
                    image.BindingContext = vm = new ProductClassifierItemViewModel(item.Key, item.Value, HandleButtonClickCommand);
                    image.SetBinding(Image.SourceProperty, nameof(ProductClassifierItemViewModel.Image));
                    image.SetBinding(Image.OpacityProperty, nameof(ProductClassifierItemViewModel.Opacity));
                    image.WidthRequest = image.HeightRequest = ButtonSize;
                    image.HorizontalOptions = LayoutOptions.Center;
                    image.Aspect = Aspect.AspectFill;
                    TapGestureRecognizer gestureRecognizer = new TapGestureRecognizer() { BindingContext = vm };
                    gestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(ProductClassifierItemViewModel.ClickedCommand));
                    image.GestureRecognizers.Add(gestureRecognizer);
                    stackLayout.Children.Add(image);
                }
                Content = stackLayout;
            }
        }

        Command HandleButtonClickCommand => new Command(
            (param) =>
            {
                var vm = param as ProductClassifierItemViewModel;
                if (vm.IsSelected)
                {
                    Debug.Assert(!Selected.Contains(vm.Classifier));
                    selected.Add(vm.Classifier);
                }
                else
                {
                    Debug.Assert(Selected.Contains(vm.Classifier));
                    selected.Remove(vm.Classifier);
                }
                string tmp = "";
                selected.ForEach(item => tmp += $"{item}, ");
                Console.WriteLine(tmp);
            });
    }
}