using AutoMapper;
using Barwy.Data.Data.Models;
using Barwy.Data.Data.ViewModels.User;

namespace Barwy.Data.Data.AutoMapper
{
    public class AutoMapperUserProfile : Profile
    {
        public AutoMapperUserProfile()
        {
            CreateMap<AppUser, UserVM>();
        }
    }
}
