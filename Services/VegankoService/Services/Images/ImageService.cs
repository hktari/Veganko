using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VegankoService.Models.Products;

namespace VegankoService.Services.Images
{
    public class ImageService : IImageService
    {
        private const string DetailImagesFolder = "detail";
        private const string ThumbImagesFolder = "thumb";
        private readonly ILogger<ImageService> logger;

        public ImageService(ILogger<ImageService> logger)
        {
            this.logger = logger;
        }

        public void DeleteImages(string imageName)
        {
            logger.LogInformation("Deleting images with name: " + imageName);

            string[] images = new[]
            {
                Path.Combine(
                    Directory.GetCurrentDirectory(), "..", "images", ThumbImagesFolder, imageName),
                Path.Combine(
                    Directory.GetCurrentDirectory(), "..", "images", DetailImagesFolder, imageName)
            };

            foreach (var imagePath in images)
            {
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
                else
                {
                    logger.LogWarning($"Failed to find image at : " + imagePath);
                }
            }
        }

        public async Task<string> SaveImage(ProductImageInput input)
        {
            string trustedFileNameForStorage = Guid.NewGuid().ToString();

            if (!ValidateImage(input.DetailImage, DetailImagesFolder, trustedFileNameForStorage, out string detailImagePath)
                || !ValidateImage(input.ThumbImage, ThumbImagesFolder, trustedFileNameForStorage, out string thumbImagePath))
            {
                logger.LogError("Failed to validate images");
                throw new ArgumentException("invalid images");
            }

            await SaveImage(input.DetailImage, detailImagePath);
            await SaveImage(input.ThumbImage, thumbImagePath);

            return Path.GetFileName(detailImagePath);
        }

        private bool ValidateImage(IFormFile image, string folder, string trustedFileNameForStorage, out string targetFilePath)
        {
            targetFilePath = null;

            if (image.Length <= 0)
            {
                return false;
            }

            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(
                    image.FileName);
            double fileSize = image.Length / 1000000.0d;

            logger.LogDebug("Received image with name: {trustedFileNameForDisplay}, " +
                "size: {fileSize} MB.", trustedFileNameForDisplay, fileSize);

            string[] permittedExtensions = { ".jpg" };

            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                // The extension is invalid ... discontinue processing the file
                return false;
            }

            targetFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "images", folder, trustedFileNameForStorage + ext);
            return true;
        }

        private async Task SaveImage(IFormFile image, string targetFilePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

            using (var targetStream = System.IO.File.Create(targetFilePath))
            {
                await image.CopyToAsync(targetStream);

                logger.LogInformation("Uploaded file '{TrustedFileNameForDisplay}'", targetFilePath);
            }
        }
    }
}
