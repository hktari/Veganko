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
        protected virtual Dictionary<ProductClassifier, string> ImageSource => new Dictionary<ProductClassifier, string>
        {
            { ProductClassifier.Vegeterijansko, "ico_vegetarian.png" },
            { ProductClassifier.Vegansko, "ico_vegan.png" },
            { ProductClassifier.Pesketarijansko, "ico_pescetarian.png" },
            { ProductClassifier.GlutenFree, "ico_gluten_free.png" },
            { ProductClassifier.RawVegan, "ico_raw_vegan.png" },
            { ProductClassifier.CrueltyFree, "ico_cruelty_free.png" },
        };

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(List<ProductClassifier>), typeof(ProductClassifierView), null, propertyChanged: OnSourceChanged);

        protected double viewSize = 22.0d;
        public double ViewSize
        {
            get { return viewSize; }
            set { viewSize = value; }
        }

        private LayoutOptions horizontalAlignment = LayoutOptions.Center;
        public LayoutOptions HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set { horizontalAlignment = value; }
        } 
        
        protected StackOrientation orientation = StackOrientation.Horizontal;
        public StackOrientation Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        protected List<ProductClassifier> source = new List<ProductClassifier>();
        public virtual List<ProductClassifier> Source
        {
            get
            {
                return (List<ProductClassifier>)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
                HandleSourceChanged(value);
            }
        }

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as ProductClassifierView;
            view.HandleSourceChanged(newValue as List<ProductClassifier>);
        }

        public virtual void HandleSourceChanged(List<ProductClassifier> newSource)
        {
            if (newSource == null)
                return;
            SetViewContent(CreateView(newSource));
        }

        protected void SetViewContent(IEnumerable<View> views)
        {
            var stackLayout = new StackLayout { Orientation = orientation, HorizontalOptions = horizontalAlignment};
            foreach (var item in views)
            {
                stackLayout.Children.Add(item);
            }
            Content = stackLayout;
        }
        
        protected string GetImageForClassifer(ProductClassifier classifier)
        {
            return ImageSource.Single((kv) => kv.Key == classifier).Value;
        }

        private List<View> CreateView(List<ProductClassifier> source)
        {
            List<View> views = new List<View>();
            foreach (var classifier in source)
            {
                var image = new Image();
                ProductClassifierItemViewModel vm;
                image.BindingContext = vm = new ProductClassifierItemViewModel(classifier, GetImageForClassifer(classifier));
                image.SetBinding(Image.SourceProperty, nameof(ProductClassifierItemViewModel.Image));
                image.WidthRequest = image.HeightRequest = ViewSize;
                image.HorizontalOptions = LayoutOptions.Center;
                image.Aspect = Aspect.AspectFill;
                views.Add(image);
            }
            return views;
        }

    }
}