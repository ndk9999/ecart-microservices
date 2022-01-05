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

        public CartController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
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
                cart.CartHeader.OrderTotal = cart.CartDetails.Sum(x => x.Product.Price * x.Count);
            }

            return cart;
        }
    }
}
