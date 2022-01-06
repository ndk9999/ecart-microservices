using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Mango.Web.Services
{
    public class CartService : GenericService, ICartService
    {
        private readonly ServiceUrls _serviceUrls;

        public CartService(IHttpClientFactory clientFactory, IOptions<ServiceUrls> urlOptions) : base(clientFactory)
        {
            _serviceUrls = urlOptions.Value;
        }

        public async Task<T> AddToCartAsync<T>(CartDto cartDto, string accessToken = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.POST,
                Payload = cartDto,
                Url = _serviceUrls.ShoppingCartApi + "/api/cart/addcart",
                AccessToken = accessToken
            });
        }

        public async Task<T> ApplyCouponAsync<T>(CartDto cartDto, string accessToken = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.POST,
                Payload = cartDto,
                Url = _serviceUrls.ShoppingCartApi + "/api/cart/applycoupon",
                AccessToken = accessToken
            });
        }

        public async Task<T> ClearCartAsync<T>(string userId, string accessToken = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.DELETE,
                Url = _serviceUrls.ShoppingCartApi + "/api/cart/clearcart/" + userId,
                AccessToken = accessToken
            });
        }

        public async Task<T> GetCartByUserIdAsync<T>(string userId, string accessToken = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.GET,
                Url = _serviceUrls.ShoppingCartApi + "/api/cart/getcart/" + userId,
                AccessToken = accessToken
            });
        }

        public async Task<T> RemoveCouponAsync<T>(string userId, string accessToken = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.POST,
                Payload = userId,
                Url = _serviceUrls.ShoppingCartApi + "/api/cart/removecoupon",
                AccessToken = accessToken
            });
        }

        public async Task<T> RemoveFromCartAsync<T>(int itemId, string accessToken = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.POST,
                Payload = itemId,
                Url = _serviceUrls.ShoppingCartApi + "/api/cart/removecart",
                AccessToken = accessToken
            });
        }

        public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string accessToken = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                Type = ApiType.POST,
                Payload = cartDto,
                Url = _serviceUrls.ShoppingCartApi + "/api/cart/updatecart",
                AccessToken = accessToken
            });
        }
    }
}
