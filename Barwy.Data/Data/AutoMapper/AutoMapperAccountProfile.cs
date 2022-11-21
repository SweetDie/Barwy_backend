using AutoMapper;
using Barwy.Data.Data.Models;
using Barwy.Data.Data.ViewModels.Account;

namespace Barwy.Data.Data.AutoMapper
{
    public class AutoMapperAccountProfile : Profile
    {
        public AutoMapperAccountProfile()
        {
            CreateMap<SignUpVM, AppUser>();
        }
    }
}
