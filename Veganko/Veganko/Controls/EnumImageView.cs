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
    public partial class EnumImageView<T> : ContentView
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(ObservableCollection<T>), typeof(EnumImageView<T>), new ObservableCollection<T>(), propertyChanged: OnSourceChanged, coerceValue: SourceCoerceValue);

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(nameof(ImageSource), typeof(Dictionary<T, string>), typeof(EnumImageView<T>), new Dictionary<T,string>());

        public Dictionary<T, string> ImageSource
        {
            get
            {
                return (Dictionary<T, string>)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

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

        public virtual ObservableCollection<T> Source
        {
            get
            {
                return (ObservableCollection<T>)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
                HandleSourceChanged(value); // called twice ?
            }
        }


        private static object SourceCoerceValue(BindableObject bindable, object value)
        {
            var selected = value as ObservableCollection<T>;
            if (selected == null)
            {
                return new ObservableCollection<T>();
            }
            return selected;
        }

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as EnumImageView<T>;
            view.HandleSourceChanged(newValue as ObservableCollection<T>);
        }

        public virtual void HandleSourceChanged(ObservableCollection<T> newSource)
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
        
        protected string GetImageForClassifer(T classifier)
        {
            return ImageSource.Single((kv) => kv.Key.Equals(classifier)).Value;
        }

        private List<View> CreateView(ObservableCollection<T> source)
        {
            List<View> views = new List<View>();
            foreach (var classifier in source)
            {
                var image = new Image();
                EnumImageItemViewModel<T> vm;
                image.BindingContext = vm = new EnumImageItemViewModel<T>(classifier, GetImageForClassifer(classifier));
                image.SetBinding(Image.SourceProperty, nameof(EnumImageItemViewModel<T>.Image));
                image.WidthRequest = image.HeightRequest = ViewSize;
                image.HorizontalOptions = LayoutOptions.Center;
                image.Aspect = Aspect.AspectFill;
                views.Add(image);
            }
            return views;
        }
    }
}