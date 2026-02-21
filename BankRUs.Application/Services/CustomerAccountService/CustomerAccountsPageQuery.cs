using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Application.Services.CustomerAccountService;

public record CustomerAccountsPageQuery(
    string? Search,
    string? FirstName,
    string? LastName,
    string? Email,
    string? Ssn) : BasePageQuery;