using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Text;
using Java.Util;

namespace Veganko.Droid.Services.PhotoPicker
{
    public class PhotoPicker
    {
        private Context context;

        public void Init(Context context)
        {
            this.context = context;
        }
}
}