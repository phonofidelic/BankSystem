using BankRUs.Application.Configuration;
using BankRUs.Application.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BankRUs.Infrastructure.Services.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        // Seeda data applikation behöver för att fungera
        // däribland roller (Customer, CustomerService, Admin, ...)
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedRolesAsync(roleManager);

        // Seed default admin account if none exists
        var adminConfig = services.GetRequiredService<IOptions<DefaultAdmin>>().Value;
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var defaultAdmin = await userManager.FindByNameAsync(adminConfig.Username);
        if (defaultAdmin == null)
        {
            await SeedDefaultAdmin(userManager, adminConfig);
        }
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

    public static async Task SeedTestUser(UserManager<ApplicationUser> userManager, CreateApplicationUserRequest request, Guid id, string role)
    {
        var user = new ApplicationUser
        {
            Id = id.ToString(),
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var password = request.Password.Trim();
        
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, role);
    }

    private static async Task SeedDefaultAdmin(UserManager<ApplicationUser> userManager, DefaultAdmin adminConfig)
    {
        var admin = new ApplicationUser
        {
            UserName = adminConfig.Username.Trim(),
            Email = adminConfig.Email.Trim(),
            FirstName = adminConfig.FirstName.Trim(),
            LastName = adminConfig.LastName.Trim()
        };

        var password = adminConfig.Password.Trim();

        await userManager.CreateAsync(admin, password);
        await userManager.AddToRoleAsync(admin, Roles.SystemAdmin);
    }
}
