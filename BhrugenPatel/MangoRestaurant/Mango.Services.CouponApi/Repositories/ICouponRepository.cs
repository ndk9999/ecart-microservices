﻿using Mango.Services.CouponApi.Models.Dto;

namespace Mango.Services.CouponApi.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
