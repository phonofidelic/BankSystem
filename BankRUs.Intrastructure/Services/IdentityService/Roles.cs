namespace BankRUs.Infrastructure.Services.Identity;

public static class Roles
{
    public const string Customer = "Customer";
    public const string CustomerServiceRepresentative = "CustomerServiceRepresentative";
    public const string SystemAdmin = "SystemAdministrator";

    public static readonly string[] All = [SystemAdmin, Customer, CustomerServiceRepresentative];
}
