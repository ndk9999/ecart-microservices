using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Mango.Web.Services
{
    public class CouponService : GenericService, ICouponService
    {
        private readonly ServiceUrls _serviceUrls;

        public CouponService(IHttpClientFactory httpClient, IOptions<ServiceUrls> urlOptions) : base(httpClient)
        {
            _serviceUrls = urlOptions.Value;
        }

        public async Task<T> GetCouponAsync<T>(string couponCode, string accessToken = null)
        {
            return await this.SendAsync<T>(new ApiRequest
            {
                Type= ApiType.GET,
                Url = _serviceUrls.CouponApi + "/api/coupon/" + couponCode,
                AccessToken = accessToken
            });
        }
    }
}
