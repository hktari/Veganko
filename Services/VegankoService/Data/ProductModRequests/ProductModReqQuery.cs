using Veganko.Common.Models.Products;

namespace VegankoService.Data.ProductModRequests
{
    public struct ProductModReqQuery 
    {
        public ProductModReqQuery(ProductModRequestState? state = null, string userId = null, int page = 1, int pageSize = 10)
        {
            State = state;
            UserId = userId;
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public string UserId { get; set; }
        public ProductModRequestState? State { get; set; }
    }
}
