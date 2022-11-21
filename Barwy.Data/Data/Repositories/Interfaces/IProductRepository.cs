using Barwy.Data.Data.Managers;
using Barwy.Data.Data.Models;

namespace Barwy.Data.Data.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductManagerResult> CreateProductAsync(Product model);
        Task<ProductManagerResult> UpdateProductAsync(Product product);
        Task<Product> GetProductByIdAsync(string id);
        Task<ProductManagerResult> CreateCategoryAsync(string categoryName);
    }
}
