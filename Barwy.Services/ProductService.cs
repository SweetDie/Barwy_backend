using AutoMapper;
using Barwy.Data.Data.Models;
using Barwy.Data.Data.Repositories.Interfaces;
using Barwy.Data.Data.ViewModels.Product;

namespace Barwy.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse> CreateProductAsync(CreateProductVM productVM)
        {
            var model = _mapper.Map<Product>(productVM);
            var result = await _productRepository.CreateProductAsync(model);

            if (result.Succeeded)
            {
                return new ServiceResponse
                {
                    IsSuccess = true,
                    Message = "Product create success"
                };
            }

            return new ServiceResponse
            {
                IsSuccess = false,
                Message = result.Errors.First(),
                Errors = result.Errors
            };
        }

        public async Task<ServiceResponse> UpdateProductAsync(UpdateProductVM productVM)
        {
            var model = _mapper.Map<Product>(productVM);
            var result = await _productRepository.UpdateProductAsync(model);

            if (result.Succeeded)
            {
                return new ServiceResponse
                {
                    IsSuccess = true,
                    Message = "Product update success"
                };
            }

            return new ServiceResponse
            {
                IsSuccess = false,
                Message = result.Errors.First(),
                Errors = result.Errors
            };
        }

        public async Task<ServiceResponse> CreateCategoryAsync(string categoryName)
        {
            var result = await _productRepository.CreateCategoryAsync(categoryName);

            if (result.Succeeded)
            {
                return new ServiceResponse
                {
                    IsSuccess = true,
                    Message = "Category created"
                };
            }

            return new ServiceResponse
            {
                IsSuccess = false,
                Message = result.Errors.First(),
                Errors = result.Errors
            };
        }
    }
}
