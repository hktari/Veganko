using System;
using Veganko.Models;
using Veganko.Services.Http;

namespace Veganko.Services.Products
{
    public class ProductHelper : IProductHelper
    {
        private const string DetailImageEndpoint = "images/detail/";
        private const string ThumbImageEndpoint = "images/thumb/";

        /// <summary>
        /// Constructs the urls for the detail and thumb images from the image name and endpoint.
        /// </summary>
        public void AddImageUrls(Product product)
        {
            if (product.ImageName == null)
            {
                return;
            }

            // TODO: take only hostname; no relative paths
            string endpoint = RestService.Endpoint
                .Replace("https", "http")
                .Replace("5001", "5000")
                .Replace("api", string.Empty);

            if (!endpoint.EndsWith("/"))
            {
                endpoint += "/";
            }

            product.DetailImage =
                new Uri(
                    new Uri(
                        new Uri(endpoint),
                        DetailImageEndpoint),
                    product.ImageName)
                .AbsoluteUri;

            product.ThumbImage = new Uri(
                    new Uri(
                        new Uri(endpoint),
                        ThumbImageEndpoint),
                    product.ImageName)
                .AbsoluteUri;
        }
    }
}
