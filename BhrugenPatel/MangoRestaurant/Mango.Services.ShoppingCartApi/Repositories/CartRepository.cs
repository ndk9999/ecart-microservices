using AutoMapper;
using Mango.Services.ShoppingCartApi.DbContexts;
using Mango.Services.ShoppingCartApi.Models;
using Mango.Services.ShoppingCartApi.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartApi.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public CartRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cartHeaderFromDb = await _dbContext.CartHeaders
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cartHeaderFromDb == null) return false;

            cartHeaderFromDb.CouponCode = couponCode;
            _dbContext.CartHeaders.Update(cartHeaderFromDb);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _dbContext.CartHeaders
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cartHeaderFromDb is null) return false;

            _dbContext.CartDetails.RemoveRange(_dbContext.CartDetails.Where(x => x.CartId == cartHeaderFromDb.Id));
            _dbContext.CartHeaders.Remove(cartHeaderFromDb);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            var cart = _mapper.Map<Cart>(cartDto);
            var cartDetail = cart.CartDetails.FirstOrDefault();
            var prodInDb = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == cartDetail.ProductId);

            if (prodInDb == null)
            {
                _dbContext.Products.Add(cartDetail.Product);
                await _dbContext.SaveChangesAsync();
            }

            var cartHeaderFromDb = await _dbContext.CartHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == cart.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                _dbContext.CartHeaders.Add(cart.CartHeader);
                await _dbContext.SaveChangesAsync();

                cartDetail.CartId = cart.CartHeader.Id;
                cartDetail.Product = null;
                _dbContext.CartDetails.Add(cartDetail);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var cartDetailFromDb = await _dbContext.CartDetails
                    .FirstOrDefaultAsync(x => x.ProductId == cartDetail.ProductId && x.CartId == cartHeaderFromDb.Id);
                
                if (cartDetailFromDb == null)
                {
                    cartDetail.CartId = cartHeaderFromDb.Id;
                    cartDetail.Product = null;
                    _dbContext.CartDetails.Add(cartDetail);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    cartDetailFromDb.Count += cartDetail.Count;
                    _dbContext.CartDetails.Update(cartDetailFromDb);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            var cart = new Cart()
            {
                CartHeader = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId)
            };

            cart.CartDetails = await _dbContext.CartDetails
                .Include(x => x.Product)
                .Where(x => x.CartId == cart.CartHeader.Id)
                .ToListAsync();

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartHeaderFromDb = await _dbContext.CartHeaders
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cartHeaderFromDb == null) return false;

            cartHeaderFromDb.CouponCode = null;
            _dbContext.CartHeaders.Update(cartHeaderFromDb);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            try
            {
                var cartDetail = await _dbContext.CartDetails
                .FirstOrDefaultAsync(x => x.Id == cartDetailId);

                int totalCountOfCartItems = await _dbContext.CartDetails
                    .CountAsync(x => x.CartId == cartDetail.CartId);

                _dbContext.CartDetails.Remove(cartDetail);

                if (totalCountOfCartItems == 1)
                {
                    var cartHeader = await _dbContext.CartHeaders.FindAsync(cartDetail.CartId);
                    _dbContext.CartHeaders.Remove(cartHeader);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
