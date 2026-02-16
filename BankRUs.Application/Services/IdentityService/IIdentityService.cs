namespace BankRUs.Application.Services.Identity;

public interface IIdentityService
{
    Task<CreateApplicationUserResult> CreateApplicationUserAsync(CreateApplicationUserRequest request);
    Task AssignCustomerServiceRepresentativeRoleToUser(Guid applicationUserId);
    Task DeleteApplicationUserAsync(Guid applicationUserId);
}
