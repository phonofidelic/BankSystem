namespace BankRUs.Application.Services.Identity;

public interface IIdentityService
{
    Task<CreateUserResult> CreateUserAsync(CreateUserRequest request);
    Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request);
}
