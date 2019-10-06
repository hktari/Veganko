using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Text;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using Debug = System.Diagnostics.Debug;
using File = Java.IO.File;
using Stream = System.IO.Stream;
using Uri = Android.Net.Uri;

[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
namespace Veganko.Droid
{
    public partial class MainActivity
    {
        const int REQUEST_TAKE_PHOTO = 1;
        private int maxPhotoWidthInPix;
        private int maxPhotoHeightInDips;
        private string currentPhotoPath;
        private TaskCompletionSource<byte[]> takePictureTCS;

        public Task<byte[]> DispatchTakePictureIntent(int maxPhotoHeightInDips, int maxPhotoWidthInPix)
        {
            this.maxPhotoWidthInPix = maxPhotoWidthInPix;
            this.maxPhotoHeightInDips = maxPhotoHeightInDips;
            takePictureTCS = new TaskCompletionSource<byte[]>();

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
                    Uri photoUri = FileProvider.GetUriForFile(this, "com.honeybunny.Veganko.fileprovider", photoFile);
                    takePictureIntent.PutExtra(MediaStore.ExtraOutput, photoUri);
                    Log.Info("VEGANKO", "Starting photo picking activity with path: " + photoFile.AbsolutePath);
                    base.StartActivityForResult(takePictureIntent, REQUEST_TAKE_PHOTO);
                }
            }
            else
            {
                throw new Exception("No camera app available.");
            }

            return takePictureTCS.Task;
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
                BitmapFactory.DecodeFile(currentPhotoPath, bmOptions);

                DisplayMetrics metrics = new DisplayMetrics();
                WindowManager.DefaultDisplay.GetMetrics(metrics);

                // Get the dimensions of the View
                int targetW = ToDps(maxPhotoWidthInPix, metrics.Xdpi);
                int targetH = maxPhotoHeightInDips;

                int photoW = ToDps(bmOptions.OutWidth, metrics.Xdpi);
                int photoH = ToDps(bmOptions.OutHeight, metrics.Ydpi);

                // Determine how much to scale down the image
                int scaleFactor = Math.Min(photoW / targetW, photoH / targetH);

                Log.Debug("VEGANKO", $"Scale factor: {scaleFactor}");

                // Decode the image file into a Bitmap sized to fill the View
                bmOptions.InJustDecodeBounds = false;
                bmOptions.InSampleSize = scaleFactor;

                Log.Debug("VEGANKO", "Decode image.");

                Bitmap bitmap = BitmapFactory.DecodeFile(currentPhotoPath, bmOptions);
                bitmap = RotateImageIfRequired(bitmap, currentPhotoPath);

                using (MemoryStream stream = new MemoryStream())
                {
                    if (await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 100, stream))
                    {
                        stream.Position = 0;
                        takePictureTCS.SetResult(stream.ToArray());
                    }
                    else
                    {
                        Debug.Fail("Failed to compress bitmap to jpeg.");
                    }
                }

                bitmap.Dispose();
            }
        }

        /**
         * Rotate an image if required.
         *
         * @param img           The image bitmap
         * @param selectedImage Image URI
         * @return The resulted Bitmap after manipulation
         */
        private Bitmap RotateImageIfRequired(Bitmap img, string photoPath)
        {

            ExifInterface ei = new ExifInterface(photoPath);
            int orientation = ei.GetAttributeInt(ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

            switch (orientation)
            {
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

        private int ToDps(int pixels, float dpi)
        {
            return (int)Math.Ceiling((pixels * 160.0d) / dpi);
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
    }
}