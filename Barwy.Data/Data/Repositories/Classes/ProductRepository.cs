using Barwy.Data.Data.Managers;
using Barwy.Data.Data.Models;
using Barwy.Data.Data.Repositories.Interfaces;

namespace Barwy.Data.Data.Repositories.Classes
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductManager _productManager;

        public ProductRepository(ProductManager productManager)
        {
            _productManager = productManager;
        }

        public async Task<ProductManagerResult> CreateCategoryAsync(string categoryName)
        {
            var result = await _productManager.CreateCategoryAsync(categoryName);
            return result;
        }

        public async Task<ProductManagerResult> CreateProductAsync(Product model)
        {
            var result = await _productManager.CreateProductAsync(model);
            return result;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            var result = await _productManager.GetProductByIdAsync(id);
            return result;
        }

        public async Task<ProductManagerResult> UpdateProductAsync(Product model)
        {
            var result = await _productManager.UpdateProductAsync(model);
            return result;
        }
    }
}
