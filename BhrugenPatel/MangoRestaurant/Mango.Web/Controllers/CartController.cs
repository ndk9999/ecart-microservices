using Mango.Web.Models.Dto;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await LoadCartBasedOnLoggedInUser();
            return View(cart);
        }

        public async Task<IActionResult> Remove(int itemId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveFromCartAsync<ResponseDto>(itemId, accessToken);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCouponAsync<ResponseDto>(cartDto, accessToken);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCouponAsync<ResponseDto>(cartDto.CartHeader.UserId, accessToken);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = await LoadCartBasedOnLoggedInUser();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.CheckoutAsync<ResponseDto>(cartDto.CartHeader, accessToken);

                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(Confirmation));
                }

                TempData["Error"] = response.DisplayMessage;
                return RedirectToAction(nameof(Checkout));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Checkout));
            }
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }


        private async Task<CartDto> LoadCartBasedOnLoggedInUser()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);
            var cart = new CartDto();

            if (response is { IsSuccess: true })
            {
                cart = response.GetResultAs<CartDto>();
            }

            if (cart.CartHeader != null)
            {
                var orderTotal = cart.CartDetails.Sum(x => x.Product.Price * x.Count);

                cart.CartHeader.DiscountTotal = await ComputeDiscountAmount(cart, accessToken);
                cart.CartHeader.OrderTotal = orderTotal - cart.CartHeader.DiscountTotal;
            }

            return cart;
        }

        private async Task<decimal> ComputeDiscountAmount(CartDto cartDto, string accessToken)
        {
            if (string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                return 0;

            var response = await _couponService.GetCouponAsync<ResponseDto>(cartDto.CartHeader.CouponCode, accessToken);
            if (response == null) return 0;

            var coupon = response.GetResultAs<CouponDto>();
            return coupon?.DiscountAmount ?? 0;
        }
    }
}
