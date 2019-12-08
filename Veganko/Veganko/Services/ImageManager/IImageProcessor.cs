using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Veganko.Services.ImageManager
{
    public interface IImageProcessor
    {
        Task<byte[]> GenerateThumbnail(byte[] imageData, int height, int width);
    }
}
