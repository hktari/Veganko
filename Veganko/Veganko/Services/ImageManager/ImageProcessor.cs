using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FFImageLoading;
using System.Threading.Tasks;

namespace Veganko.Services.ImageManager
{
    public class ImageProcessor : IImageProcessor
    {
        public async Task<byte[]> GenerateThumbnail(byte[] imageData, int height, int width)
        {
            Stream downSampleImgStream = await ImageService.Instance
                .LoadStream(_ => Task.FromResult<Stream>(new MemoryStream(imageData)))
                .DownSample(width, height)
                .AsJPGStreamAsync();

            using (downSampleImgStream)
            using (MemoryStream ms = new MemoryStream())
            {
                downSampleImgStream.CopyTo(ms);
                return ms.ToArray();
            }

            // Check out if in doubt https://forums.xamarin.com/discussion/67531/how-to-resize-image-file-on-xamarin-forms-writeablebitmap-package-cant-be-added
        }
    }
}
