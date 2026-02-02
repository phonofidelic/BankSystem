namespace BankRUs.Application.Services.Identity;

public interface IIdentityService
{
    Task<CreateApplicationUserResult> CreateApplicationUserAsync(CreateApplicationUserRequest request);
    Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request);
}
