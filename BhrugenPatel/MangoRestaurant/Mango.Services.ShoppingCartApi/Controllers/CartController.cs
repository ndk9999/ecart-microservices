using Mango.Services.ShoppingCartApi.Models.Dto;
using Mango.Services.ShoppingCartApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartApi.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet("getcart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                var cartDto = await _cartRepository.GetCartByUserId(userId);
                return ResponseDto.Ok(cartDto);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [HttpPost("addcart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                var cart = await _cartRepository.CreateUpdateCart(cartDto);
                return ResponseDto.Ok(cart);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [HttpPost("updatecart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            try
            {
                var cart = await _cartRepository.CreateUpdateCart(cartDto);
                return ResponseDto.Ok(cart);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [HttpPost("removecart/{itemId}")]
        public async Task<object> RemoveCart([FromBody] int itemId)
        {
            try
            {
                var success = await _cartRepository.RemoveFromCart(itemId);
                return ResponseDto.Ok(success);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [HttpDelete("clearcart/{userId}")]
        public async Task<object> ClearCart(string userId)
        {
            try
            {
                var success = await _cartRepository.ClearCart(userId);
                return ResponseDto.Ok(success);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }
    }
}
