using Barwy.Data.Data.ViewModels.Product;
using Barwy.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Barwy.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpPost("createProduct")]
        public async Task<IActionResult> CreateProductAsync(CreateProductVM model)
        {
            var result = await _productService.CreateProductAsync(model);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("updateProduct")]
        public async Task<IActionResult> UpdateProductAsync(UpdateProductVM model)
        {
            var result = await _productService.UpdateProductAsync(model);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateCategoryAsync(string  categoryName)
        {
            var result = await _productService.CreateCategoryAsync(categoryName);
            return Ok(result);
        }
    }
}
