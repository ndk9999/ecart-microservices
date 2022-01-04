using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var products = new List<ProductDto>();
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");

            if (response is not null && response.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }

            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var product = new ProductDto();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id, "");

            if (response is not null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }

            return View(product);
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Details(ProductDto productDto)
        {
            var cartDto = new CartDto
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value
                }
            };

            var cartDetails = new CartDetailDto
            {
                Count = productDto.Count,
                ProductId = productDto.Id
            };

            var response = await _productService.GetProductByIdAsync<ResponseDto>(productDto.Id, "");
            if (response is { IsSuccess: true })
            {
                var jsonData = Convert.ToString(response.Result);
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>(jsonData);
            }

            cartDto.CartDetails = new List<CartDetailDto>()
            {
                cartDetails
            };

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addToCartResponse = await _cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken);
            
            if (addToCartResponse is { IsSuccess: true})
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productDto);
        }

        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}