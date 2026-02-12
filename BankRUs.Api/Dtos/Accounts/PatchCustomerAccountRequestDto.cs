namespace BankRUs.Api.Dtos.Accounts;

public record PatchCustomerAccountRequestDto(
string? FirstName,
string? LastName,
string? Email,
string? Ssn);
