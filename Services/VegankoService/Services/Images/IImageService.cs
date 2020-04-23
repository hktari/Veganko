using System.Threading.Tasks;
using VegankoService.Models.Products;

namespace VegankoService.Services.Images
{
    public interface IImageService
    {
        void DeleteImages(string imageName);
        Task<string> SaveImage(ProductImageInput input);
    }
}
