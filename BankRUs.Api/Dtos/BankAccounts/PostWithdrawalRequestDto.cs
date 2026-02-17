namespace BankRUs.Api.Dtos.BankAccounts;

public record PostWithdrawalRequestDto(
        decimal Amount,
        string IsoCurrencySymbol,
        string? Reference);
