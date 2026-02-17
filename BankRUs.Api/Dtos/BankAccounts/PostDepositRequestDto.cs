namespace BankRUs.Api.Dtos.BankAccounts
{
    public record PostDepositRequestDto(
        decimal Amount,
        string IsoCurrencySymbol,
        string? Reference);
}
