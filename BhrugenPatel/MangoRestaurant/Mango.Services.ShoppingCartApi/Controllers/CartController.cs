using Mango.MessageBus;
using Mango.Services.ShoppingCartApi.Messages;
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
        private readonly ICouponRepository _couponRepository;
        private readonly IMessageBus _messageBus;

        public CartController(ICartRepository cartRepository, ICouponRepository couponRepository, IMessageBus messageBus)
        {
            _cartRepository = cartRepository;
            _couponRepository = couponRepository;
            _messageBus = messageBus;
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

        [HttpPost("removecart")]
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

        [HttpPost("applycoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var success = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                return ResponseDto.Ok(success);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [HttpPost("removecoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                var success = await _cartRepository.RemoveCoupon(userId);
                return ResponseDto.Ok(success);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [HttpPost("checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutModel)
        {
            try
            {
                var cartDto = await _cartRepository.GetCartByUserId(checkoutModel.UserId);
                if (cartDto == null) return BadRequest();

                if (!string.IsNullOrEmpty(checkoutModel.CouponCode))
                {
                    var coupon = await _couponRepository.GetCoupon(checkoutModel.CouponCode);
                    if (checkoutModel.DiscountTotal != coupon.DiscountAmount)
                    {
                        return ResponseDto.Error(
                            "Coupon Price has changed, please confirm",
                            "Coupon Price has changed, please confirm");
                    }
                }

                checkoutModel.CartDetails = cartDto.CartDetails;

                // TODO: Put the logic to add message to process order here
                await _messageBus.PublishMessageAsync(checkoutModel, "checkout-message-topic");
                await _cartRepository.ClearCart(checkoutModel.UserId);

                return ResponseDto.Ok(true);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }
    }
}
