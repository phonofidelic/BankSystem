using System;
using BankRUs.Infrastructure.Services.Identity;

namespace BankRUs.Infrastructure.Services.IdentityService;

// ToDo: Implement custom policies with IAuthorizationService
//       See: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0
public static class Policies
{
    public const string REQUIRE_AUTHENTICATION = "policy.authentication";
    public const string REQUIRE_ROLE_CUSTOMER_SERVICE = "policy.role." + Roles.CustomerServiceRepresentative;
    public const string REQUIRE_ROLE_SYSTEM_ADMIN = "policy.role." + Roles.SystemAdmin;
}
