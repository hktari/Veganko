using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Veganko.Models;
using Veganko.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Veganko.Controls
{
    public class SelectableEnumImageView<T> : EnumImageView<T>
    {
        public static readonly BindableProperty SelectedProperty =
            BindableProperty.Create(nameof(Selected), typeof(ObservableCollection<T>), typeof(SelectableEnumImageView<T>), new ObservableCollection<T>(), propertyChanged: OnSelectedChanged);

        public ObservableCollection<T> Selected
        {
            get
            {
                return (ObservableCollection<T>)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }

        Command HandleButtonClickCommand => new Command(OnButtonClick);

        List<EnumImageItemViewModel<T>> itemViewModels;

        public SelectableEnumImageView()
        {
            itemViewModels = new List<EnumImageItemViewModel<T>>();
        }

        private static void OnSelectedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as SelectableEnumImageView<T>;

            if (oldValue is ObservableCollection<T> oldSelected)
            {
                oldSelected.CollectionChanged -= view.HandleSelectedCollectionChanged;
            }

            if (newValue is ObservableCollection<T> newSelected)
            {
                view.HandleSelectedCollectionChanged(newSelected, null);
                newSelected.CollectionChanged += view.HandleSelectedCollectionChanged;
            }
        }

        private void HandleSelectedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyChildren(sender as ObservableCollection<T>);
        }
        
        public override void HandleSourceChanged(ObservableCollection<T> newSource)
        {
            // re-initialize the buttons and state of the viewmodel
            Selected?.Clear();
            itemViewModels.Clear();

            if (newSource != null)
                SetViewContent(CreateView(newSource));

            if (Selected != null)
                NotifyChildren(Selected);
        }

        /// <summary>
        /// Handle when an item has been clicked
        /// </summary>
        /// <param name="param">the EnumImageItemViewModel of the item view</param>
        void OnButtonClick(object param)
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
        }
        
        /// <summary>
        /// Notify the ItemViewModels when the Selected collection is changed from outside the view
        /// </summary>
        public void NotifyChildren(ObservableCollection<T> selected)
        {
            foreach (var itemVm in itemViewModels)
            {
                itemVm.IsSelected = selected.Contains(itemVm.Classifier);
            }
        }

        List<View> CreateView(ObservableCollection<T> source)
        {
            List<View> views = new List<View>();
            foreach (var classifier in source)
            {
                var image = new Image();
                EnumImageItemViewModel<T> vm;
                image.BindingContext = vm = new EnumImageItemViewModel<T>(classifier, GetImageForClassifer(classifier), HandleButtonClickCommand);
                image.SetBinding(Image.SourceProperty, nameof(EnumImageItemViewModel<T>.Image));
                image.SetBinding(Image.OpacityProperty, nameof(EnumImageItemViewModel<T>.Opacity));
                //image.WidthRequest = image.HeightRequest = ViewSize;
                image.HorizontalOptions = LayoutOptions.Center;
                image.SetBinding(VisualElement.WidthRequestProperty, new Binding(nameof(ViewSize), BindingMode.Default, null, null, null, this));
                image.SetBinding(VisualElement.HeightRequestProperty, new Binding(nameof(ViewSize), BindingMode.Default, null, null, null, this));
                image.Aspect = Aspect.AspectFit;

                TapGestureRecognizer gestureRecognizer = new TapGestureRecognizer() { BindingContext = vm };
                gestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, nameof(EnumImageItemViewModel<T>.ClickedCommand));
                
                image.GestureRecognizers.Add(gestureRecognizer);
                views.Add(image);
                itemViewModels.Add(vm);
            }
            return views;
        }
    }
}
