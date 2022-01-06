using AutoMapper;
using Mango.Services.CouponApi.DbContexts;
using Mango.Services.CouponApi.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponApi.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public CouponRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var coupon = await _dbContext.Coupons
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CouponCode == couponCode);

            return _mapper.Map<CouponDto>(coupon);
        }
    }
}
