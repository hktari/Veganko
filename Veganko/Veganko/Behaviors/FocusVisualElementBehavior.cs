using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Behaviors
{
    public class FocusVisualElementBehavior : BehaviorBase<Editor>
    {
        public static readonly BindableProperty FocusCommandProperty =
            BindableProperty.Create(nameof(FocusCommand), typeof(Command), typeof(FocusVisualElementBehavior), default(Command), BindingMode.OneWayToSource);

        public Command FocusCommand
        {
            get
            {
                return (Command)GetValue(FocusCommandProperty);
            }
            set
            {
                SetValue(FocusCommandProperty, value);
            }
        }

        public FocusVisualElementBehavior()
        {
            FocusCommand = new Command(_ => AssociatedObject?.Focus());
        }
    }
}
