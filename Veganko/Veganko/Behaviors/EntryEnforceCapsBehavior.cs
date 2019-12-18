using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Veganko.Behaviors
{
    public class EntryEnforceCapsBehavior : Behavior<Entry>
    {
        private Entry entry;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            entry = bindable;
            bindable.TextChanged += OnTextChanged;
        }
        
        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= OnTextChanged;
            entry = null;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            entry.Text = e.NewTextValue?.ToUpperInvariant();
        }
    }
}
