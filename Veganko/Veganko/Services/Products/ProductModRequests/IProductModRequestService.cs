using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Veganko.Common.Models.Products;
using Veganko.Models;

namespace Veganko.Services.Products.ProductModRequests
{
    public interface IProductModRequestService
    {
        Task<ProductModRequestDTO> AddAsync(ProductModRequestDTO item);
        Task<PagedList<ProductModRequestDTO>> AllAsync(int page = 1, int pageSize = 10, string userId = null, ProductModRequestState? state = null, bool forceRefresh = false);
        Task<ProductModRequestDTO> ApproveAsync(ProductModRequestDTO item);
        Task DeleteAsync(ProductModRequestDTO item);
        Task<ProductModRequestDTO> GetAsync(string id);
        Task<ProductModRequestDTO> RejectAsync(ProductModRequestDTO item);
        Task<ProductModRequestDTO> UpdateAsync(ProductModRequestDTO item);
        Task<ProductModRequestDTO> UpdateImagesAsync(ProductModRequestDTO product, byte[] detailImageData, byte[] thumbImageData);
    }
}
