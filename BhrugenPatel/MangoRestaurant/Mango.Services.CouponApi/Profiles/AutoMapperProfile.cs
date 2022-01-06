using AutoMapper;
using Mango.Services.CouponApi.Models;
using Mango.Services.CouponApi.Models.Dto;

namespace Mango.Services.CouponApi.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}
