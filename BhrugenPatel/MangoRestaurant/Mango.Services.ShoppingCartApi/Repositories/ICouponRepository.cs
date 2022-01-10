using Mango.Services.ShoppingCartApi.Models.Dto;

namespace Mango.Services.ShoppingCartApi.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
