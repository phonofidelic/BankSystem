namespace BankRUs.Application.Services.CustomerService
{
    public class CustomersSearchQuery(string? search)
    {
        public string FirstName { get; set; } = search ?? string.Empty;
        public string LastName { get; set; } = search ?? string.Empty;
        public string Email { get; set; } = search ?? string.Empty;
        public string SocialSecurityNumber { get; set; } = search ?? string.Empty;
    }
}