using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence
{
    public class SeedRoles
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (await roleManager.RoleExistsAsync("Author") &&
                await roleManager.RoleExistsAsync("Editor") &&
                await roleManager.RoleExistsAsync("Moderator"))
            {
                return;
            }


            await roleManager.CreateAsync(new IdentityRole("Author"));
            await roleManager.CreateAsync(new IdentityRole("Editor"));
            await roleManager.CreateAsync(new IdentityRole("Moderator"));
        }
    }
}
