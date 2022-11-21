using Barwy.Data.Data.Context;
using Barwy.Data.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Barwy.Data.Initializer
{
    public class AppDbInitializer
    {
        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                if (userManager.FindByNameAsync("admin").Result == null)
                {
                    AppUser admin = new AppUser()
                    {
                        UserName = "admin",
                        Email = "admin@email.com",
                        EmailConfirmed = true,
                        Name = "Administrator"
                    };

                    AppUser user = new AppUser()
                    {
                        UserName = "user",
                        Email = "user@email.com",
                        EmailConfirmed = true,
                        Name = "User"
                    };

                    context.Roles.AddRange(
                        new IdentityRole()
                        {
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR",
                        },
                        new IdentityRole()
                        {
                            Name = "User",
                            NormalizedName = "USER"
                        }
                    );

                    await context.SaveChangesAsync();

                    var resultAdmin = userManager.CreateAsync(admin, "Qwerty-1").Result;
                    var resultUser = userManager.CreateAsync(user, "Qwerty-1").Result;

                    if (resultAdmin.Succeeded)
                    {
                        userManager.AddToRoleAsync(admin, "Administrator").Wait();
                    }
                    if (resultUser.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "User").Wait();
                    }
                }
            }

        }
    }
}
