using BankRUs.Application.Services.Identity;
using BankRUs.Application.Exceptions;
using BankRUs.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace BankRUs.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public IdentityService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<CreateApplicationUserResult> CreateApplicationUserAsync(CreateApplicationUserRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email.Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim()
        };

        string password = "Secret#1";

        // TODO: Skapa användaren i databasen (ASP.NET Core Identity)
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            if (error != null)
                throw new DuplicateCustomerException(error.Description, error.Code);
            throw new Exception("Unable to create user");
        }

        await _userManager.AddToRoleAsync(user, Roles.Customer);

        return new CreateApplicationUserResult(UserId: Guid.Parse(user.Id));
    }
}
