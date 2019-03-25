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
        public async static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();

            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("Default"));
            }

            if (!context.Users.Any())
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = "admin@example.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "admin"
                };

                await userManager.CreateAsync(user, "Asdf_1234");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
