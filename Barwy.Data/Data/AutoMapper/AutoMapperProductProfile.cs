using AutoMapper;
using Barwy.Data.Data.Models;
using Barwy.Data.Data.ViewModels.Product;

namespace Barwy.Data.Data.AutoMapper
{
    public class AutoMapperProductProfile : Profile
    {
        public AutoMapperProductProfile()
        {
            CreateMap<CreateProductVM, Product>();
            CreateMap<UpdateProductVM, Product>();
        }
    }
}
