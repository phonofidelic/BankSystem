namespace BankRUs.Infrastructure.Services.Identity;

public static class Roles
{
    public const string Customer = "Customer";
    public const string CustomerService = "CustomerService";

    public static readonly string[] All = [Customer, CustomerService];
}
