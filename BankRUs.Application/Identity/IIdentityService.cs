namespace BankRUs.Application.Identity;

public interface IIdentityService
{
    Task<CreateUserResult> CreateUserAsync(CreateUserRequest request);
    Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request);
}
