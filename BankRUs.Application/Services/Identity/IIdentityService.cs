using BankRUs.Application.Services.CustomerService;

namespace BankRUs.Application.Services.Identity;

public interface IIdentityService
{
    Task<CreateApplicationUserResult> CreateApplicationUserAsync(CreateApplicationUserRequest request);

    Task DeleteApplicationUserAsync(Guid ApplicationUserId);
}
