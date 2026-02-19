namespace BankRUs.Api.Dtos.CustomerAccounts;

public record PatchCustomerAccountRequestDto(
string? FirstName,
string? LastName,
string? Email,
string? Ssn);
