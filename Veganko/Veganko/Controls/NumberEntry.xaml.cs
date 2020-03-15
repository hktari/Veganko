using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NumberEntry : ContentView
    {
        public static readonly BindableProperty NumberProperty =
            BindableProperty.Create(nameof(Number), typeof(double), typeof(NumberEntry), default(double));
        private bool formattingText;

        public double Number
        {
            get => (double)GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }

        public NumberEntry()
        {
            InitializeComponent();
            entry.TextChanged += Entry_TextChanged;
            entry.InputTransparent = true;
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += Tap_Tapped;
            GestureRecognizers.Add(tap);

            entry.Keyboard = Keyboard.Numeric;
            FormatText(Number);
        }

        private void Tap_Tapped(object sender, EventArgs e)
        {
            entry.Focus();
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnTextChanged(e.OldTextValue, e.NewTextValue);
        }

        protected void OnTextChanged(string oldValue, string newValue)
        {
            // IF DIGIT
            // MOVE TO FRONT

            // UPDATE TEXT

            if (formattingText)
            {
                return;
            }

            //if (newValue == null || newValue.Length == 0)
            //{
            //    // Default when input is not a 
            //    FormatText("");
            //    return;
            //}

            newValue = Regex.Replace(newValue, "[^0-9]", string.Empty);
            while (newValue.StartsWith("0"))
            {
                newValue = newValue.Remove(0, 1);
            }

            double num = 0;
            for (int i = newValue.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(newValue[i].ToString());

                if (digit == 0)
                {
                    continue;
                }

                num += digit * Math.Pow(10, -2 + (newValue.Length - 1 - i));
            }

            Number = num;

            FormatText(num);
        }

        private void FormatText(double text)
        {
            formattingText = true;
            entry.Text = string.Format("{0:0.00}", text);
            formattingText = false;
        }
    }
}