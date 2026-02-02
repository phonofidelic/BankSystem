using BankRUs.Application.Services.Identity;
using BankRUs.Application.UseCases.OpenAccount.Exceptions;
using BankRUs.Domain.Entities;
using BankRUs.Infrastructure.Persistance;
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

    public async Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request)
    {
        try
        {
            var newCustomer = new Customer
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = request.ApplicationUserId,
                Email = request.Email,
                SocialSecurityNumber = request.SocialSecurityNumber
            };

            // ToDo: Move to OpenBankAccount use case
            BankAccount bankAccount = new()
            {
                Id = Guid.NewGuid(),
                CustomerId = newCustomer.Id
            };

            newCustomer.BankAccounts.Add(bankAccount);
            await _context.Customers.AddAsync(newCustomer);
            await _context.SaveChangesAsync();

            return new CreateCustomerResult(newCustomer.Id);
        }
        catch (Exception ex) {
            throw new Exception("Could not create Customer");
        }
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
