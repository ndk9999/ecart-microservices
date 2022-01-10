using Mango.Services.ShoppingCartApi.Models.Dto;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartApi.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _httpClient;

        public CouponRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var responseMsg = await _httpClient.GetAsync($"/api/coupon/{couponCode}");
            var apiContent = await responseMsg.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            return response.IsSuccess 
                ? response.GetResultAs<CouponDto>()
                : new CouponDto();
        }
    }
}
