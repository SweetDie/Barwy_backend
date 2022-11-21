using Barwy.Data.Data.AutoMapper;

namespace Barwy.API.Infrastructure.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public static void Config(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperUserProfile));
            services.AddAutoMapper(typeof(AutoMapperAccountProfile));
            services.AddAutoMapper(typeof(AutoMapperProductProfile));
        }
    }
}
