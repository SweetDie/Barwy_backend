using Barwy.Data.Data.Context;
using Barwy.Data.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Barwy.Data.Data.Managers
{
    public class ProductManager
    {
        private readonly AppDbContext _context;

        public ProductManager(AppDbContext context)
        {
            _context = context;
        }

        // Product
        public async Task<ProductManagerResult> CreateProductAsync(Product product)
        {
            var result = new ProductManagerResult();

            product.Id ??= Guid.NewGuid().ToString();
            try
            {
                await _context.AddAsync(product);
                await _context.SaveChangesAsync();

                result.Succeeded = true;
            }
            catch (Exception e)
            {
                result.Errors.Add(e.Message);
                result.Succeeded = false;
            }

            return result;
        }

        public async Task<ProductManagerResult> UpdateProductAsync(Product product)
        {
            var result = new ProductManagerResult();

            product.Id ??= Guid.NewGuid().ToString();
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();

                result.Succeeded = true;
            }
            catch (Exception e)
            {
                result.Errors.Add(e.Message);
                result.Succeeded = false;
            }

            return result;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            return product;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        //Category
        public async Task<ProductManagerResult> CreateCategoryAsync(string categoryName)
        {
            var result = new ProductManagerResult();

            if(categoryName.Length == 0)
            {
                result.Succeeded = false;
                result.Errors.Add("Name cannot be empty");
                return result;
            }
            else if(categoryName.Length == 1)
            {
                categoryName = char.ToUpper(categoryName[0]).ToString();
            }
            else
            {
                categoryName = char.ToUpper(categoryName[0]) + categoryName.Substring(1).ToLower();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());

            if(category != null)
            {
                result.Succeeded = false;
                result.Errors.Add("Category already exists");
                return result;
            }

            var newCategory = new Category { Id = Guid.NewGuid().ToString(), Name = categoryName };

            try
            {
                await _context.AddAsync(newCategory);
                await _context.SaveChangesAsync();

                result.Succeeded = true;
            }
            catch (Exception e)
            {
                result.Errors.Add(e.Message);
                result.Succeeded = false;
            }

            return result;
        }
    }
}
