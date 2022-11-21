using Barwy.Data.Data.Repositories.Classes;
using Barwy.Data.Data.Repositories.Interfaces;

namespace Barwy.API.Infrastructure.Repositories
{
    public class RepositoriesConfiguration
    {
        public static void Config(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}
