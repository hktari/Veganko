using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Behaviors
{
    public class ScrollToTopBehavior : Behavior<ScrollView>
    {
        public static readonly BindableProperty ScrollCommandProperty =
            BindableProperty.Create(nameof(ScrollCommand), typeof(Command), typeof(ScrollToTopBehavior),
                                    default(Command), BindingMode.OneWayToSource,
                                    defaultValueCreator: CreateDefaultScrollCommand);

        private ScrollView scrollView;

        public Command ScrollCommand
        {
            get
            {
                return (Command)GetValue(ScrollCommandProperty);
            }
            set
            {
                SetValue(ScrollCommandProperty, value);
            }
        }

        protected override void OnAttachedTo(ScrollView bindable)
        {
            base.OnAttachedTo(bindable);
            BindingContext = scrollView = bindable;
            scrollView.BindingContextChanged += OnScrollViewBCChanged;
        }

        private void OnScrollViewBCChanged(object sender, EventArgs args)
        {
            BindingContext = scrollView.BindingContext;
        }

        protected override void OnDetachingFrom(ScrollView bindable)
        {
            base.OnDetachingFrom(bindable);
            scrollView.BindingContextChanged -= OnScrollViewBCChanged;
            BindingContext = scrollView = null;
        }

        private static object CreateDefaultScrollCommand(BindableObject bindable)
        {
            ScrollToTopBehavior sttb = (ScrollToTopBehavior)bindable;
            return new Command(
                async () =>
                {
                    if (sttb.scrollView != null)
                    {
                        await sttb.scrollView.ScrollToAsync(0, 0, false);
                    }
                });
        }
    }
}
