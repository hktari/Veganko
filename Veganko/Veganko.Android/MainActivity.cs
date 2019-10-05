using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ImageCircle.Forms.Plugin.Droid;
using ZXing.Mobile;
using Microsoft.WindowsAzure.MobileServices;
using Veganko.Services;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using FFImageLoading.Forms.Platform;
using Android.Content;
using Android.Provider;
using Java.IO;
using Android.Support.V4.Content;
using Java.Text;
using Java.Util;
using Debug = System.Diagnostics.Debug;
using File = Java.IO.File;
using Stream = System.IO.Stream;
using Uri = Android.Net.Uri;
using Android.Graphics;
using Android.Util;
using System.IO;
using Java.Nio;
using System.Runtime.InteropServices;
using Android.Media;

[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
namespace Veganko.Droid
{
    [Activity(Label = "Veganko", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
    {
        public static MainActivity Context { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);

            Context = this;
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            ImageCircleRenderer.Init();
            CachedImageRenderer.Init(true);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            // Initialize the scanner first so it can track the current context
            MobileBarcodeScanner.Initialize(this.Application);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);
            CurrentPlatform.Init();
            // Initialize the authenticator before loading the app.
            App.Init((IAuthenticate)this);

            LoadApplication(new App());
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #region Taking Photos

        private int GetDps()
        {
            // (DPI / 160) * dps = pixels
            // dps = (pixels * 160) / DPI

            // pixels, dpi
            DisplayMetrics metrics = new DisplayMetrics();
            Log.Debug("VEGANKO", "Get metrics.");

            WindowManager.DefaultDisplay.GetMetrics(metrics);
            int heightPixels = metrics.HeightPixels;
            int widthPixels = metrics.WidthPixels;
            int densityDpi = (int)metrics.DensityDpi;
            float xdpi = metrics.Xdpi;
            float ydpi = metrics.Ydpi;
            int widthDps = ToDps(widthPixels, xdpi);
            Debug.WriteLine("widthDps  = " + widthDps);
            Debug.WriteLine("widthPixels  = " + widthPixels);
            Debug.WriteLine("heightPixels = " + heightPixels);
            Debug.WriteLine("densityDpi   = " + densityDpi);
            Debug.WriteLine("xdpi         = " + xdpi);
            Debug.WriteLine("ydpi         = " + ydpi);

            return widthDps;
        }

        private static int ToDps(int pixels, float dpi)
        {
            return (int)Math.Ceiling((pixels * 160.0d) / dpi);
        }

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == REQUEST_TAKE_PHOTO)
            {
                if (resultCode == Result.Canceled)
                {
                    Log.Debug("VEGANKO", "Picking photo failed. Result code is canceled.");
                    return;
                }


                if (takePictureTCS == null)
                {
                    throw new Exception("Take picture task is null ");
                }
                else if (takePictureTCS.Task.Status == TaskStatus.RanToCompletion)
                {
                    throw new Exception("The Take picture task has already completed");
                }


                Log.Debug("VEGANKO", "start processing take photo result.");

                // Get the dimensions of the bitmap
                BitmapFactory.Options bmOptions = new BitmapFactory.Options();
                bmOptions.InJustDecodeBounds = true;

                Log.Debug("VEGANKO", "Decode for image bounds.");

                BitmapFactory.DecodeFile(currentPhotoPath, bmOptions);

                DisplayMetrics metrics = new DisplayMetrics();
                
                WindowManager.DefaultDisplay.GetMetrics(metrics);

                Log.Debug("VEGANKO", $"Image width: {bmOptions.OutWidth}\theight: {bmOptions.OutHeight}");

                int maxWidth = 1080;
                // Get the dimensions of the View
                int targetW = ToDps(maxWidth, metrics.Xdpi);
                int targetH = 300;
                Log.Debug("VEGANKO", $"Image in DPS width: {targetW}\theight: {targetH}");


                int photoW = ToDps(bmOptions.OutWidth, metrics.Xdpi);
                int photoH = ToDps(bmOptions.OutHeight, metrics.Ydpi);

                Log.Debug("VEGANKO", $"Image in DPS width: {photoW}\theight: {photoH}");

                // Determine how much to scale down the image
                int scaleFactor = Math.Min(photoW / targetW, photoH / targetH);

                Log.Debug("VEGANKO", $"Scale factor: {scaleFactor}");

                // Decode the image file into a Bitmap sized to fill the View
                bmOptions.InJustDecodeBounds = false;
                bmOptions.InSampleSize = scaleFactor;
                //bmOptions.InPurgeable = true;

                Log.Debug("VEGANKO", "Decode image.");

                Bitmap bitmap = BitmapFactory.DecodeFile(currentPhotoPath, bmOptions);
                bitmap = RotateImageIfRequired(bitmap, currentPhotoUri);
                //ByteBuffer byteBuffer = ByteBuffer.AllocateDirect(bitmap.ByteCount);
                //byteBuffer.Position(0);
                //bitmap.CopyPixelsToBuffer(byteBuffer);

                //int count = byteBuffer.Capacity();
                //Log.Debug("VEGANKO", $"byte buffer capacity: {count}.");

                //byte[] buffer = new byte[count];
                //byteBuffer.Position(0);
                //Marshal.Copy(byteBuffer.GetDirectBufferAddress(), buffer, 0, count);

                //ByteArrayOutputStream stream = new ByteArrayOutputStream();
                MemoryStream stream = new MemoryStream();
                if (await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, stream))
                {
                    stream.Position = 0;
                    takePictureTCS.SetResult(stream);
                    //PhotoPicked?.Invoke(this, stream);
                }
                else
                {
                    Debug.Fail("Failed to compress bitmap to jpeg.");
                }

                //byte[] buffer = stream.ToArray();

                //Bitmap bitmap2 = BitmapFactory.DecodeByteArray(buffer, 0, buffer.Length);

                //byteBuffer.Clear();
                //byteBuffer.Dispose();
                //bitmap.Dispose();

                //MemoryStream stream = new MemoryStream(buffer);

                //takePictureTCS.SetResult(stream);
                //Log.Debug("VEGANKO", $"Successfuly decoded bitmap: width: {bitmap?.Width}\theight: {bitmap?.Height}");
            }
        }

        /**
         * Rotate an image if required.
         *
         * @param img           The image bitmap
         * @param selectedImage Image URI
         * @return The resulted Bitmap after manipulation
         */
        private Bitmap RotateImageIfRequired(Bitmap img, Uri selectedImage)
        {

            ExifInterface ei = new ExifInterface(currentPhotoPath);
            int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

            switch (orientation) {
            case (int)Android.Media.Orientation.Rotate90:
                return RotateImage(img, 90);
            case (int)Android.Media.Orientation.Rotate180:
                return RotateImage(img, 180);
            case (int)Android.Media.Orientation.Rotate270:
                return RotateImage(img, 270);
            default:
                return img;
            }
        }

        private Bitmap RotateImage(Bitmap img, int degree)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(degree);
            Bitmap rotatedImg = Bitmap.CreateBitmap(img, 0, 0, img.Width, img.Height, matrix, true);
            img.Recycle();
            return rotatedImg;
        }

        const int REQUEST_TAKE_PHOTO = 1;

        public EventHandler<byte[]> PhotoPicked;

        TaskCompletionSource<Stream> takePictureTCS;

        Uri currentPhotoUri;
        String currentPhotoPath;

        public Task<Stream> DispatchTakePictureIntent()
        {
            takePictureTCS = new TaskCompletionSource<Stream>();
            //if (context == null)
            //{
            //    throw new Exception("Not initialized.");
            //}

            Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
            // Ensure that there's a camera activity to handle the intent
            if (takePictureIntent.ResolveActivity(this.PackageManager) != null)
            {
                // Create the File where the photo should go
                File photoFile = null;
                try
                {
                    photoFile = CreateImageFile();
                }
                catch (Java.IO.IOException ex)
                {
                    throw new Exception("Failed to Create file for image.", ex);
                }

                // Continue only if the File was successfully created
                if (photoFile != null)
                {
                    currentPhotoUri = FileProvider.GetUriForFile(this,
                                                          "com.honeybunny.Veganko.fileprovider",
                                                          photoFile);
                    takePictureIntent.PutExtra(MediaStore.ExtraOutput, currentPhotoUri);
                    System.Diagnostics.Debug.WriteLine("Starting photo picking activity with path: " + photoFile.AbsolutePath);
                    base.StartActivityForResult(takePictureIntent, REQUEST_TAKE_PHOTO);
                }
            }

            return takePictureTCS.Task;
        }

        private File CreateImageFile()
        {
            // Create an image file name
            String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            String imageFileName = timeStamp;
            File storageDir = this.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures);
            storageDir.Mkdirs();

            File image = File.CreateTempFile(
                imageFileName,  /* prefix */
                ".jpg",         /* suffix */
                storageDir      /* directory */
            );

            // Save a file: path for use with ACTION_VIEW intents
            currentPhotoPath = image.AbsolutePath;
            return image;
        }
        #endregion

        #region FB_Auth
        // Define a authenticated user.
        private MobileServiceUser user;

        public async Task<bool> Authenticate()
        {
            var success = false;
            var message = string.Empty;
            try
            {
                // Sign in with Facebook login using a server-managed flow.
                user = await App.MobileService.LoginAsync(this,
                    MobileServiceAuthenticationProvider.Facebook, "facebook");
                if (user != null)
                {
                    message = string.Format("you are now signed-in as {0}.",
                        user.UserId);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Display the success or failure message.
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage(message);
            builder.SetTitle("Sign-in result");
            builder.Create().Show();

            return success;
        }
        #endregion
    }
}

