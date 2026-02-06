namespace BankRUs.Api.Dtos.BankAccounts;

public record PostWithdrawalRequestDto(
        decimal Amount,
        string ISO_Currency_Symbol,
        string? Reference);
