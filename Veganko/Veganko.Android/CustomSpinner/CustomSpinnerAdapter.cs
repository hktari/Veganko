using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Veganko.Droid.CustomSpinner
{
    public class CustomSpinnerAdapter : BaseAdapter, AdapterView.IOnItemSelectedListener
    {
        int[] images;
        List<string> fruit;
        LayoutInflater inflter;

        public CustomSpinnerAdapter(Context applicationContext, int[] flags, List<string> fruit) 
        {
            this.images = flags;
            this.fruit = fruit;
            inflter = (LayoutInflater.From(applicationContext));
        }

        public IReadOnlyCollection<string> Items => fruit;

        public EventHandler<string> ItemSelected { get; set; }

        public override int Count => images.Length;

        public override Java.Lang.Object GetItem(int position)
        {
            return fruit.ElementAtOrDefault(position);
        }

        public override long GetItemId(int i)
        {
            return 0;
        }

        public override View GetView(int i, View view, ViewGroup viewGroup)
        {
            view = inflter.Inflate(Resource.Layout.spinner_custom_layout, null);
            ImageView icon = (ImageView)view.FindViewById(Resource.Id.imageView);
            TextView names = (TextView)view.FindViewById(Resource.Id.textView);
            icon.SetImageResource(images[i]);
            names.SetText(fruit[i], TextView.BufferType.Normal);
            return view;
        }

        void AdapterView.IOnItemSelectedListener.OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            ItemSelected?.Invoke(this, fruit[position]);
        }

        void AdapterView.IOnItemSelectedListener.OnNothingSelected(AdapterView parent)
        {
            return;
        }
    }
}