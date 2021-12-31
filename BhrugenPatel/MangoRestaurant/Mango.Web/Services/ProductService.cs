using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Mango.Web.Services
{
    public class ProductService : GenericService, IProductService
    {
        private readonly ServiceUrls _serviceUrls;

        public ProductService(IHttpClientFactory clientFactory, IOptions<ServiceUrls> urlOptions) : base(clientFactory)
        {
            _serviceUrls = urlOptions.Value;
        }

        public async Task<T> CreateProductAsync<T>(ProductDto productDto, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.POST,
                Payload = productDto,
                Url = _serviceUrls.ProductApi + "/api/products",
                AccessToken = accessToken
            });
        }

        public async Task<T> DeleteProductAsync<T>(int id, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.DELETE,
                Url = _serviceUrls.ProductApi + "/api/products/" + id,
                AccessToken = accessToken
            });
        }

        public async Task<T> GetAllProductsAsync<T>(string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.GET,
                Url = _serviceUrls.ProductApi + "/api/products/",
                AccessToken = accessToken
            });
        }

        public async Task<T> GetProductByIdAsync<T>(int id, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.GET,
                Url = _serviceUrls.ProductApi + "/api/products/" + id,
                AccessToken = accessToken
            });
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto productDto, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.PUT,
                Payload = productDto,
                Url = _serviceUrls.ProductApi + "/api/products",
                AccessToken = accessToken
            });
        }
    }
}
