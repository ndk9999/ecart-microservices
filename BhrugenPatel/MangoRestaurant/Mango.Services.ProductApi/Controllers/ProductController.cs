using Mango.Services.ProductApi.Models.Dto;
using Mango.Services.ProductApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers
{
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var products = await _productRepository.GetProducts();
                return ResponseDto.Ok(products);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            try
            {
                var product = await _productRepository.GetProductById(id);
                return ResponseDto.Ok(product);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<object> Post([FromBody] ProductDto productDto)
        {
            try
            {
                var product = await _productRepository.CreateUpdateProduct(productDto);
                return ResponseDto.Ok(product);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<object> Put([FromBody] ProductDto productDto)
        {
            try
            {
                var product = await _productRepository.CreateUpdateProduct(productDto);
                return ResponseDto.Ok(product);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<object> Delete(int id)
        {
            try
            {
                var success = await _productRepository.DeleteProduct(id);
                return ResponseDto.Ok(success);
            }
            catch (Exception ex)
            {
                return ResponseDto.Error(ex);
            }
        }
    }
}
