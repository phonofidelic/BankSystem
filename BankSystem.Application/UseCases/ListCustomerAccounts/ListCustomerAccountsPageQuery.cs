using BankSystem.Application.Services.PaginationService;

namespace BankSystem.Application.UseCases.ListCustomerAccounts;

public record ListCustomerAccountsPageQuery(
    string? Search,
    string? FirstName,
    string? LastName,
    string? Email,
    string? Ssn) : BasePageQuery;