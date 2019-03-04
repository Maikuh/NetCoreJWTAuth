using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtExample.Data
{
    public class UserSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = "usuario@example.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "usuario"
                };

                userManager.CreateAsync(user, "Asdf_1234");
            }
        }
    }
}
