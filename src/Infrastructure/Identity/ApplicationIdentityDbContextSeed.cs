using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NoteIt.Infrastructure.Identity
{
    public class ApplicationIdentityDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {         
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("DbSeed");

                var env = scope.ServiceProvider.GetService<IHostingEnvironment>();

                if(env.IsDevelopment() == false)
                    return;

                var roleManager = scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                var adminRole = new ApplicationRole("Admin");
                var userRole = new ApplicationRole("User");

                var admin = new ApplicationUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };

                var user = new ApplicationUser
                {
                    UserName = "user@test.com",
                    Email = "user@test.com"
                };

                await userManager.CreateAsync(admin, "Passw0rd!");
                await userManager.CreateAsync(user, "Passw0rd!");

                logger.LogInformation("Created user@test.com and admin@test.com.");

                await roleManager.CreateAsync(adminRole);
                await roleManager.CreateAsync(userRole);

                logger.LogInformation("Created User and Admin roles.");

                await userManager.AddToRoleAsync(admin, "Admin");
                await userManager.AddToRoleAsync(user, "User");

                logger.LogInformation("Added User role to user@test.com and role Admin to admin@test.com.");

                await Task.CompletedTask;
            }
        }
    }
}