namespace BankRUs.Application.Services.CustomerAccountService
{
    public record CreateBankAccountRequest(
        Guid CustomerId,
        string BankAccountName);
}
