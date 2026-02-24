using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Application.UseCases.ListCustomerAccounts;

public record CustomerAccountsPageQuery(
    string? Search,
    string? FirstName,
    string? LastName,
    string? Email,
    string? Ssn) : BasePageQuery;