using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BankRUs.Intrastructure.Services.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        // Seeda data applikation behöver för att fungera
        // däribland roller (Customer, CustomerService, Admin, ...)
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedRolesAsync(roleManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // "Customer", "CustomerService"
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create role '{role}': {errors}");
                }
            }
        }
    }
}
