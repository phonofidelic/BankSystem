namespace BankRUs.Infrastructure.Services.Identity;

public static class Roles
{
    public const string Customer = "Customer";
    public const string CustomerServiceRepresentative = "CustomerServiceRepresentative";

    public static readonly string[] All = [Customer, CustomerServiceRepresentative];
}
