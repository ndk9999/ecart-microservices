using Mango.Services.CouponApi.Models.Dto;
using Mango.Services.CouponApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponApi.Controllers
{
    [ApiController]
    [Route("api/coupon")]
    public class CouponController : Controller
    {
        private readonly ICouponRepository _couponRepository;

        public CouponController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        [Authorize]
        [HttpGet("{code}")]
        public async Task<object> GetDiscountForCode(string code)
        {
            try
            {
                var couponDto = await _couponRepository.GetCouponByCode(code);
                return ResponseDto.Ok(couponDto);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }
    }
}
