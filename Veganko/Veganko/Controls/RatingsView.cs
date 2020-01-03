using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veganko.Converters;
using Xamarin.Forms;

namespace Veganko.Controls
{
	public class RatingsView : ContentView
	{
        public static readonly BindableProperty RatingProperty = 
            BindableProperty.Create(nameof(Rating), typeof(int), typeof(RatingsView), 0, propertyChanged: OnRatingPropertyChanged, coerceValue: CoerceRatingPropertyValue);

        public int Rating
        {
            get
            {
                return (int)GetValue(RatingProperty);
            }
            set
            {
                SetValue(RatingProperty, value);
            }
        }

        List<Label> stars;

		public RatingsView ()
		{
            CreateStars();
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal, Spacing = 2.5
            };
            foreach (var star in stars)
                stackLayout.Children.Add(star);
		    UpdateView(Rating);
            Content = stackLayout;
        }


        private static object CoerceRatingPropertyValue(BindableObject bindable, object value)
        {
            var val = (int)value;
            return Math.Max(0, Math.Min(val, 5)); // clamp value
        }

        private static void OnRatingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ratingsView = bindable as RatingsView;
            ratingsView.UpdateView((int)newValue);
        }
        
        private void CreateStars()
        {
            stars = new List<Label>();
            int count = 6;
            for (int i = 0; i < count; i++)
            {
                var label = new Label
                {
                    Text = i == 0 ? " " : "" + IntToStarConverter.StarUnicode,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                };
                label.GestureRecognizers.Add(
                    new TapGestureRecognizer((obj) => OnButtonClicked(label, null)));
                stars.Add(label);
            }
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            var idx = stars.IndexOf(sender as Label);
            Rating = idx;
        }

        public void UpdateView(int rating)
        {
            for (int i = 0; i < stars.Count; i++)
            {
                if (i <= rating)
                    stars[i].Opacity = 1.0d;
                else
                    stars[i].Opacity = 0.5d;
            }
        }
    }
}