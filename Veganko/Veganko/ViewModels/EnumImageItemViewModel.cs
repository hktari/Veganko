using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.ViewModels
{
    class EnumImageItemViewModel<T> : INotifyPropertyChanged
    {
        public double ActiveOpacity = 1.0d;

        public double InactiveOpacity = 0.25d;

        private bool isSelected = false;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected == value)
                    return;
                isSelected = value;

                Opacity = value ? ActiveOpacity : InactiveOpacity;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }
        private double opacity;
        public double Opacity
        {
            get
            {
                return opacity;
            }
            private set
            {
                if (opacity == value)
                    return;
                opacity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Opacity)));
            }
        }

        public FileImageSource Image { get; set; }

        public T Classifier { get; set; }

        public Command ClickedCommand => new Command(
            () =>
            {
                IsSelected = !IsSelected;
                onButtonClickCallback?.Execute(this);
            });

    public event PropertyChangedEventHandler PropertyChanged;

        Command onButtonClickCallback;

        public EnumImageItemViewModel(T classifier, FileImageSource image, Command onButtonClickCallback)
        {
            Classifier = classifier;
            Image = image;
            this.onButtonClickCallback = onButtonClickCallback;

            opacity = InactiveOpacity;
        }
        public EnumImageItemViewModel(T classifier, FileImageSource image)
        {
            Classifier = classifier;
            Image = image;
        }
    }
}
