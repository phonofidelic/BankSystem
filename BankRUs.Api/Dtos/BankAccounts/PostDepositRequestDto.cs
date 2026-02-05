namespace BankRUs.Api.Dtos.BankAccounts
{
    public record PostDepositRequestDto (
        decimal Amount,
        string ISO_Currency_Symbol,
        string? Reference);
}
