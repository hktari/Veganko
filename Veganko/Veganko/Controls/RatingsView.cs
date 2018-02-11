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
            BindableProperty.Create(nameof(Rating), typeof(int), typeof(RatingsView), 1, propertyChanged: OnRatingPropertyChanged, coerceValue: CoerceRatingPropertyValue);

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

        List<Button> stars;

		public RatingsView ()
		{
            CreateStars();
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal, Spacing = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            foreach (var star in stars)
                stackLayout.Children.Add(star);
		    UpdateView(Rating);
            Content = stackLayout;
        }


        private static object CoerceRatingPropertyValue(BindableObject bindable, object value)
        {
            var val = (int)value;
            return Math.Max(1, Math.Min(val, 5)); // clamp value
        }

        private static void OnRatingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ratingsView = bindable as RatingsView;
            ratingsView.UpdateView((int)newValue);
        }
        
        private void CreateStars()
        {
            stars = new List<Button>();
            int count = 5;
            for (int i = 0; i < count; i++)
            {
                var button = new Button
                {
                    Text = "" + IntToStarConverter.StarUnicode,
                    BackgroundColor = Color.Transparent,
                    HeightRequest = 42,
                    WidthRequest = 42,
                };
                button.Clicked += OnButtonClicked;
                stars.Add(button); 
            }
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            var idx = stars.IndexOf(sender as Button);
            Rating = idx + 1;
        }

        public void UpdateView(int rating)
        {
            var idx = rating - 1;
            for (int i = 0; i < stars.Count; i++)
            {
                if (i <= idx)
                    stars[i].Opacity = 1.0d;
                else
                    stars[i].Opacity = 0.5d;
            }
        }
    }
}