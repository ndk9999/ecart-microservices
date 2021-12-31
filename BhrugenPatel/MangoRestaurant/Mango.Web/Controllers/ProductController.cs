using Mango.Web.Models.Dto;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = new List<ProductDto>();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetAllProductsAsync<ResponseDto>(accessToken);

            if (response.IsSuccess && response.Result is not null)
            { 
                var resultAsJson = Convert.ToString(response.Result);
                products = JsonConvert.DeserializeObject<List<ProductDto>>(resultAsJson);
            }

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.CreateProductAsync<ResponseDto>(model, accessToken);
            
            if (response is not null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id, accessToken);
            
            if (response is null || !response.IsSuccess)
            {
                return NotFound();
            }

            var resultAsJson = Convert.ToString(response.Result);
            var productDto = JsonConvert.DeserializeObject<ProductDto>(resultAsJson);

            return View(productDto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.UpdateProductAsync<ResponseDto>(model, accessToken);
            
            if (response is not null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(id, accessToken);
            
            if (response is null || !response.IsSuccess)
            {
                return NotFound();
            }

            var resultAsJson = Convert.ToString(response.Result);
            var productDto = JsonConvert.DeserializeObject<ProductDto>(resultAsJson);

            return View(productDto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.DeleteProductAsync<ResponseDto>(model.Id, accessToken);
            
            if (response is not null && response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
